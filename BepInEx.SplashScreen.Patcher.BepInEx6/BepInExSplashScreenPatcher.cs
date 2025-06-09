using System.Reflection;
using System.Threading;
using BepInEx.Configuration;
using BepInEx.Preloader.Core.Patching;

[assembly: AssemblyTitle("BepInEx.SplashScreen.Patcher.Bep6")]

namespace BepInEx.SplashScreen
{
    [PatcherPluginInfo("BepInEx.SplashScreen", "SplashScreen", Metadata.Version)]
    public class BepInExSplashScreenPatcher : BasePatcher
    {
        public BepInExSplashScreenPatcher()
        {
            // Use whatever gets us to run faster, or at all
            Init();
        }

        public override void Initialize()
        {
            // Use whatever gets us to run faster, or at all
            Init();
        }

        private static int _initialized;
        public void Init()
        {
            // Only allow to run once
            if (Interlocked.Exchange(ref _initialized, 1) == 1) return;

            SplashScreenController.SpawnSplash(Config);
        }
    }
}
