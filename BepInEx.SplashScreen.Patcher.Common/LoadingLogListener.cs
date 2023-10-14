#if !GUI
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;

namespace BepInEx.SplashScreen
{
    internal sealed class LoadingLogListener : ILogListener
    {
        private LoadingLogListener() { }

        public static LoadingLogListener StartListening()
        {
            var l = new LoadingLogListener();
            Logger.Listeners.Add(l);
            return l;
        }

        private bool _disposed;
        public void Dispose()
        {
            _disposed = true;
            // todo This is not thread safe and can cause collection modifiex exceptions in random places. If called from inside a LogEvent it's 100% going to happen.
            // Logger.Listeners.Remove(this);
        }

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            if (_disposed) return;

            if (eventArgs.Source.SourceName == "BepInEx" && eventArgs.Data != null)
            {
                try
                {
                    var message = eventArgs.Data as string ?? eventArgs.Data.ToString();
                    if (message == "Chainloader startup complete")
                    {
                        // Nothing to log after this point
                        Dispose();

                        // Wait until the first frame finishes to close the splash screen
                        // Have to do this indirectly to avoid referencing the MonoBehaviour class
                        // Also we are likely not on the main thread here so need to use StartSyncInvoke before StartCoroutine
                        var threadingHelper = Traverse.CreateWithType("BepInEx.ThreadingHelper").Property("Instance");
                        var startInvokeM = threadingHelper.Method("StartSyncInvoke", new[] { typeof(Action) });
                        if (startInvokeM.MethodExists())
                        {
                            startInvokeM.GetValue(new Action(() => threadingHelper.Method("StartCoroutine", new[] { typeof(IEnumerator) }).GetValue(DelayedCo())));
                        }
                        else
                        {
                            // If there is no ThreadingHelper (most likely IL2CPP), fall back to a timer that checks if the process is responding
                            var currentProcess = Process.GetCurrentProcess();
                            var timer = new System.Timers.Timer(500);
                            timer.Elapsed += (_, __) =>
                            {
                                if (currentProcess.Responding)
                                {
                                    timer.Stop();
                                    timer.Dispose();
                                    SplashScreenController.KillSplash();
                                }
                                else
                                {
                                    SplashScreenController.Logger.LogDebug("Process not responding, waiting...");
                                    timer.Stop();
                                    timer.Start();
                                }
                            };
                            timer.AutoReset = false;
                            timer.Start();
                        }
                    }

                    SplashScreenController.SendMessage(message);
                }
                catch (Exception e)
                {
                    SplashScreenController.Logger.LogError((object)$"Crash in {nameof(LogEvent)}, aborting. Exception: {e}");
                    SplashScreenController.KillSplash();
                }
            }
        }

#if Bep6 // todo set actual necessary log levels
        public LogLevel LogLevelFilter => LogLevel.All;
#endif

        private static IEnumerator DelayedCo()
        {
            const int framesUntilFinished = 1;
            for (int i = 0; i < framesUntilFinished; i++)
                yield return null;

            SplashScreenController.KillSplash();
        }
    }
}

#endif