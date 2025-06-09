#if !GUI
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BepInEx.SplashScreen
{
    public static class SplashScreenController
    {
        internal static readonly ManualLogSource Logger = Logging.Logger.CreateLogSource("Splash");
        private static readonly Queue _StatusQueue = Queue.Synchronized(new Queue(10, 2));
        private static LoadingLogListener _logListener;
        private static Process _guiProcess;

        public static void SpawnSplash(ConfigFile config)
        {
            try
            {
                if (config == null) throw new ArgumentNullException(nameof(config));

                var isEnabled = config.Bind("SplashScreen", "Enabled", true, "Display a splash screen with information about game load progress on game start-up.").Value;
#if DEBUG
                const bool onlyNoConsoleDefault = false;
#else
                const bool onlyNoConsoleDefault = true;
#endif
                var consoleNotAllowed = config.Bind("SplashScreen", "OnlyNoConsole", onlyNoConsoleDefault, "Only display the splash screen if the logging console is turned off.").Value;

                if (!isEnabled)
                {
                    Logger.LogDebug("Not showing splash because the Enabled setting is off");
                    return;
                }

                if (consoleNotAllowed)
                {
                    if (config.TryGetEntry("Logging.Console", "Enabled", out ConfigEntry<bool> entry) && entry.Value)
                    {
                        Logger.LogDebug("Not showing splash because the console is enabled");
                        return;
                    }
                }

                var currentProcess = Process.GetCurrentProcess();

                var renameConf = config.Bind("SplashScreen", "RenameExe", true, "Automatically rename the splash .exe file to GameName.SplashScreen.GUI.dll to prevent Discord from mistaking the game as a different game.");
                var exeNameProc = currentProcess.ProcessName + ".SplashScreen.GUI.exe";
                var exeNameOrig = "BepInEx.SplashScreen.GUI.exe";

                var assemblyLocation = typeof(SplashScreenController).Assembly.Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? Path.Combine(Paths.PatcherPluginPath, "BepInEx.SplashScreen");
                var guiExecutablePath = Path.Combine(assemblyDirectory, renameConf.Value ? exeNameProc : exeNameOrig);

                if (!File.Exists(guiExecutablePath))
                {
                    var otherExePath = Path.Combine(assemblyDirectory, !renameConf.Value ? exeNameProc : exeNameOrig);

                    if (File.Exists(otherExePath))
                        File.Move(otherExePath, guiExecutablePath);
                    else
                        throw new FileNotFoundException("Executable not found or inaccessible at " + guiExecutablePath);
                }

                Logger.Log(LogLevel.Debug, "Starting GUI process: " + guiExecutablePath);

                var psi = new ProcessStartInfo(guiExecutablePath, currentProcess.Id.ToString())
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };
                _guiProcess = Process.Start(psi);

                new Thread(CommunicationThread) { IsBackground = true }.Start(_guiProcess);

                _logListener = LoadingLogListener.StartListening();
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to start GUI: " + e);
                KillSplash();
            }
        }

        internal static void SendMessage(string message)
        {
            _StatusQueue.Enqueue(message);
        }

        private static void CommunicationThread(object processArg)
        {
            try
            {
                Thread.Sleep(100);

                var guiProcess = (Process)processArg;

                guiProcess.Exited += (sender, args) => KillSplash();

                guiProcess.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null) Logger.Log(LogLevel.Debug, "[GUI] " + args.Data.Replace('\t', '\n').TrimEnd('\n'));
                };
                guiProcess.BeginOutputReadLine();

                guiProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null) Logger.Log(LogLevel.Error, "[GUI] " + args.Data.Replace('\t', '\n').TrimEnd('\n'));
                };
                guiProcess.BeginErrorReadLine();

                guiProcess.StandardInput.AutoFlush = false;

                try
                {
                    Logger.LogDebug("Connected to the GUI process");
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(50);
                    Logger.LogDebug("Connected to the GUI process");
                }

                var any = false;
                while (!guiProcess.HasExited)
                {
                    while (_StatusQueue.Count > 0 && guiProcess.StandardInput.BaseStream.CanWrite)
                    {
                        guiProcess.StandardInput.WriteLine(_StatusQueue.Dequeue());
                        any = true;
                    }

                    if (any)
                    {
                        any = false;
                        guiProcess.StandardInput.Flush();
                    }

                    Thread.Sleep(150);
                }
            }
            catch (ThreadAbortException)
            {
                // I am die, thank you forever
            }
            catch (Exception e)
            {
                Logger.LogError((object)$"Crash in {nameof(CommunicationThread)}, aborting. Exception: {e}");
            }
            finally
            {
                KillSplash();
            }
        }

        internal static void KillSplash()
        {
            try
            {
                _logListener?.Dispose();

                _StatusQueue.Clear();
                _StatusQueue.TrimToSize();

                try
                {
                    if (_guiProcess != null && !_guiProcess.HasExited)
                    {
                        Logger.LogDebug("Closing GUI process");
                        _guiProcess.Kill();
                    }
                }
                catch (Exception)
                {
                    // _guiProcess already quit so Kill threw
                }

                Logger.Dispose();
                // todo not thread safe
                // Logging.Logger.Sources.Remove(Logger);
            }
            catch (Exception e)
            {
                // Welp, no Logger left to use. This shouldn't ever happen annyways.
                Console.WriteLine(e);
            }
        }
    }
}
#endif