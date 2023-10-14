using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Mono.Cecil;

[assembly: AssemblyTitle("BepInEx.SplashScreen.Patcher")]

namespace BepInEx.SplashScreen
{
    public static class BepInExSplashScreenPatcher
    {
        public const string Version = "1.0";

        private static int _initialized;

        static BepInExSplashScreenPatcher() //todo test
        {
            Initialize();
        }

        public static IEnumerable<string> TargetDLLs
        {
            get
            {
                // Use whatever gets us to run faster, or at all
                Initialize();
                return Enumerable.Empty<string>();
            }
        }

        public static void Patch(AssemblyDefinition _)
        {
            // Use whatever gets us to run faster, or at all
            Initialize();
        }

        public static void Initialize()
        {
            // Only allow to run once
            if (Interlocked.Exchange(ref _initialized, 1) == 1) return;

            SplashScreenController.SpawnSplash();
        }
    }
}
