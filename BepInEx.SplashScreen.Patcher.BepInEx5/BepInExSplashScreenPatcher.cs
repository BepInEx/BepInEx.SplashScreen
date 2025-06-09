using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using BepInEx.Configuration;
using HarmonyLib;
using Mono.Cecil;

[assembly: AssemblyTitle("BepInEx.SplashScreen.Patcher")]

namespace BepInEx.SplashScreen
{
    public static class BepInExSplashScreenPatcher
    {
        public const string Version = "1.0";

        static BepInExSplashScreenPatcher()
        {
            // Use whatever gets us to run faster, or at all
            Init();
        }

        public static IEnumerable<string> TargetDLLs
        {
            get
            {
                // Use whatever gets us to run faster, or at all
                Init();
                return Enumerable.Empty<string>();
            }
        }

        public static void Patch(AssemblyDefinition _)
        {
            // Use whatever gets us to run faster, or at all
            Init();
        }

        private static int _initialized;
        public static void Init()
        {
            // Only allow to run once
            if (Interlocked.Exchange(ref _initialized, 1) == 1) return;

            var configPath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, "BepInEx.SplashScreen.cfg");
            var metadata = new BepInPlugin("BepInEx.SplashScreen.Patcher", "BepInEx.SplashScreen", Version);
            var config = new ConfigFile(configPath, false, metadata);
            SplashScreenController.SpawnSplash(config);
        }
    }
}
