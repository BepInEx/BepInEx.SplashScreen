using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

[assembly: AssemblyTitle("BepInEx.SplashScreen.GUI")]

namespace BepInEx.SplashScreen
{
    public static class Program
    {
        private static SplashScreen _mainForm;

        private static readonly System.Timers.Timer _AliveTimer = new System.Timers.Timer(60000);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(params string[] args)
        {
            try
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();

                if (args.Length == 0)
                {
                    if (MessageBox.Show("This is a splash screen that shows loading progress when a game patched with BepInEx is loading. It is automatically started and then updated by \"BepInEx.SplashScreen.Patcher.dll\" and can't be opened manually.\n\n" +
                                        "If you can't see a splash screen when the game is starting:\n" +
                                        "1 - Make sure that \"BepInEx.SplashScreen.GUI.exe\" and \"BepInEx.SplashScreen.Patcher.dll\" are both present inside the \"BepInEx\\patchers\" folder.\n" +
                                        "2 - Check if the splash screen isn't disabled in \"BepInEx\\config\\BepInEx.cfg\".\n" +
                                        "3 - Update BepInEx5 to latest version and make sure that it is running.\n" +
                                        "4 - If the splash screen still does not appear, check the game log for any errors or exceptions. You can report issues on GitHub.\n\n" +
                                        "Do you want to open the GitHub repository page of BepInEx.SplashScreen?",
                                        "BepInEx Loading Progress Splash Screen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        Process.Start("https://github.com/BepInEx/BepInEx.SplashScreen");
                    return;
                }

                _mainForm = new SplashScreen();
                _mainForm.Show();

                var pid = int.Parse(args.Last());
                var gameProcess = Process.GetProcessById(pid);

                BeginReadingInput(gameProcess);

                try
                {
                    // Get game name
                    _mainForm.Text = $@"{gameProcess.ProcessName} is loading...";

                    if (gameProcess.MainModule == null)
                        throw new FileNotFoundException("gameProcess.MainModule is null");

                    // Get game location and icon
                    var gameExecutable = gameProcess.MainModule.FileName;
                    _mainForm.SetGameLocation(Path.GetDirectoryName(gameExecutable));
                    _mainForm.SetIcon(IconManager.GetLargeIcon(gameExecutable, true, true).ToBitmap());

                    BeginSnapPositionToGameWindow(gameProcess);

                    // If log messages stop coming, preloader/chainloader has crashed or is stuck
                    _AliveTimer.AutoReset = false;
                    _AliveTimer.Elapsed += (_, __) =>
                    {
                        try
                        {
                            Log("Stopped receiving log messages from the game, assuming preloader/chainloader has crashed or is stuck", true);
                        }
                        catch (Exception e)
                        {
                            // ¯\_(ツ)_/¯
                            Debug.Fail(e.ToString());
                        }
                        Environment.Exit(3);
                    };
                    _AliveTimer.Start();
                }
                catch (Exception e)
                {
                    _mainForm.SetGameLocation(null);
                    _mainForm.SetIcon(null);
                    Log("Failed to get some info about the game process: " + e, true);
                    Debug.Fail(e.ToString());
                }

                Log("Splash screen window started successfully", false);

                Application.Run(_mainForm);
            }
            catch (Exception e)
            {
                Log("Failed to create window: " + e, true);
                Debug.Fail(e.ToString());
            }
        }

        public static void Log(string message, bool error)
        {
            // Patcher reads standard output and error for return info
            // Replace newlines with tabs to send multiline messages as a single line
            (error ? Console.Error : Console.Out).WriteLine(message.Replace("\t", "    ").Replace('\n', '\t').Replace("\r", ""));
        }

        #region Input processing

        private static void BeginReadingInput(Process gameProcess)
        {
            new Thread(InputReadingThread) { IsBackground = true }.Start(gameProcess);
        }
        private static void InputReadingThread(object processArg)
        {
            try
            {
                var gameProcess = (Process)processArg;

                using (var inStream = Console.OpenStandardInput())
                using (var inReader = new StreamReader(inStream))
                {
                    while (inStream.CanRead && !gameProcess.HasExited)
                    {
                        // Still receiving log messages, so preloader/chainloader is still alive and loading
                        _AliveTimer.Stop();
                        _AliveTimer.Start();

                        var line = inReader.ReadLine();

                        if (line == null) // If no data, wait a bit
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        ProcessInputMessage(line);
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.ToString(), true);
                Debug.Fail(e.ToString());
            }

            Environment.Exit(0);
        }

        // Use 10 as a failsafe in case the "x plugins to load" log message is not received for whatever reason
        private static int _pluginCount = 10;
        private static int _pluginProcessedCount = 0;
        private static LoadEvent _lastLoadEvent = LoadEvent.None;

        private static void RunEventsUpTo(LoadEvent targetEvent)
        {
            for (var i = _lastLoadEvent; i < targetEvent; i++)
            {
                _mainForm.ProcessEvent(i + 1);
                _lastLoadEvent = targetEvent;
            }
        }

        private static void ProcessInputMessage(string message)
        {
            try
            {
                switch (message)
                {
                    case "Preloader started": //bep5
                        RunEventsUpTo(LoadEvent.PreloaderStart);
                        break;
                    // For some reason "Preloader finished" is unreliable, it can
                    // be called before "Preloader started", and then again after.
                    case "Preloader finished": //bep5
                        //if (_lastLoadEvent == LoadEvent.PreloaderStart)
                        RunEventsUpTo(LoadEvent.PreloaderFinish);
                        break;
                    case "Chainloader started": //bep5
                    case "Chainloader initialized": //bep6
                        RunEventsUpTo(LoadEvent.ChainloaderStart);
                        break;
                    case "Chainloader startup complete": //bep5 and bep6
                        RunEventsUpTo(LoadEvent.ChainloaderFinish);
                        _AliveTimer.Stop();
                        _AliveTimer.Interval = 5000;
                        _AliveTimer.Start();
                        break;

                    default:
                        if (message.EndsWith(" patcher plugins loaded", StringComparison.Ordinal)) //bep6
                        {
                            RunEventsUpTo(LoadEvent.PreloaderStart);
                        }
                        else if (message.StartsWith("Patching ", StringComparison.Ordinal) || // bep5
                                 message.StartsWith("Executing ", StringComparison.Ordinal) && message.EndsWith(" patch(es)", StringComparison.Ordinal)) //bep6
                        {
                            RunEventsUpTo(LoadEvent.PreloaderStart);

                            _mainForm.SetStatusDetail(message);
                        }
                        else if (message.StartsWith("Loading ", StringComparison.Ordinal)) //bep5 and bep6
                        {
                            RunEventsUpTo(LoadEvent.ChainloaderStart);

                            _mainForm.SetStatusDetail(message);

                            _pluginProcessedCount++;
                            _mainForm.SetPluginProgress((int)Math.Round(100f * (_pluginProcessedCount / (float)_pluginCount)));
                        }
                        else if (message.StartsWith("Skipping ", StringComparison.Ordinal)) //bep5 and bep6?
                        {
                            RunEventsUpTo(LoadEvent.ChainloaderStart);

                            _pluginProcessedCount++;
                            _mainForm.SetPluginProgress((int)Math.Round(100f * (_pluginProcessedCount / (float)_pluginCount)));
                        }
                        else if (message.EndsWith(" plugins to load", StringComparison.Ordinal)) //bep5 and bep6
                        {
                            RunEventsUpTo(LoadEvent.ChainloaderStart);

                            _pluginCount = Math.Max(1, int.Parse(new string(message.TakeWhile(char.IsDigit).ToArray())));
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Log($"Failed to process message \"{message}\": {e}", true);
            }
        }

        #endregion

        #region Window poisition snap

        private static void BeginSnapPositionToGameWindow(Process gameProcess)
        {
            new Thread(SnapPositionToGameWindowThread) { IsBackground = true }.Start(gameProcess);
        }
        private static void SnapPositionToGameWindowThread(object processArg)
        {
            try
            {
                var temporarilyHidden = false;
                var gameProcess = (Process)processArg;
                while (!_mainForm.IsDisposed)
                {
                    Thread.Sleep(100);

                    if (!_mainForm.Visible && !temporarilyHidden)
                        continue;

                    var gameWindowHandle = gameProcess.MainWindowHandle;
                    var gameWindowTitle = gameProcess.MainWindowTitle;

                    if (gameWindowHandle == IntPtr.Zero ||
                        // Ignore console window
                        gameWindowTitle.StartsWith("BepInEx") ||
                        gameWindowTitle.StartsWith("Select BepInEx"))
                    {
                        // Need to refresh the process if the window handle is not yet valid or it will keep grabbing the old one
                        _mainForm.TopMost = false;
                        gameProcess.Refresh();
                        continue;
                    }

                    // Detect Unity's pre-launch resoultion and hotkey configuration window and hide the splash until it is closed
                    // It seems like it's not possible to localize this window so the title check should be fine? Hopefully?
                    if (gameWindowTitle.EndsWith(" Configuration"))
                    {
                        _mainForm.Visible = false;
                        _mainForm.TopMost = false;
                        temporarilyHidden = true;
                        gameProcess.Refresh();
                        continue;
                    }

                    if (temporarilyHidden)
                    {
                        temporarilyHidden = false;
                        _mainForm.Visible = true;
                    }

                    if (!NativeMethods.GetWindowRect(new HandleRef(_mainForm, gameWindowHandle), out var rct))
                        throw new InvalidOperationException("GetWindowRect failed :(");

                    var foregroundWindow = NativeMethods.GetForegroundWindow();
                    // The main game window is not responding most of the time, which prevents it from being recognized as the foreground window
                    // To work around this, check if the currently focused window is the splash window, as it will most likely be the last focused window after user clicks on the game window
                    _mainForm.TopMost = gameWindowHandle == foregroundWindow ||  NativeMethods.IsBorderless(gameWindowHandle);

                    // Just in case, don't want to mangle the splash
                    if (default(NativeMethods.RECT).Equals(rct))
                        continue;

                    var x = rct.Left + (rct.Right - rct.Left) / 2 - _mainForm.Width / 2;
                    var y = rct.Top + (rct.Bottom - rct.Top) / 2 - _mainForm.Height / 2;
                    var newLocation = new Point(x, y);

                    if (_mainForm.Location != newLocation)
                        _mainForm.Location = newLocation;

                    if (_mainForm.FormBorderStyle != FormBorderStyle.None)
                    {
                        // At this point the form is snapped to the main game window so prevent user from trying to drag it
                        _mainForm.FormBorderStyle = FormBorderStyle.None;
                        //_mainForm.BackColor = Color.White;
                        _mainForm.PerformLayout();
                    }
                }
            }
            catch (Exception)
            {
                // Not much we can do here, it's not critical either way
                Environment.Exit(1);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;        // x position of upper-left corner
                public int Top;         // y position of upper-left corner
                public int Right;       // x position of lower-right corner
                public int Bottom;      // y position of lower-right corner
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            private static int GWL_STYLE = -16;
            private static int WS_BORDER = 0x00800000; //window with border
            private static int WS_DLGFRAME = 0x00400000; //window with double border but no title
            private static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar 
            private static int WS_SYSMENU = 0x00080000; //window menu  

            public static bool IsBorderless(IntPtr windowPtr)
            {
                int style = GetWindowLong(windowPtr, GWL_STYLE);

                return (style & WS_CAPTION) == 0 && (style & WS_SYSMENU) == 0;
            }
        }

        #endregion
    }
}
