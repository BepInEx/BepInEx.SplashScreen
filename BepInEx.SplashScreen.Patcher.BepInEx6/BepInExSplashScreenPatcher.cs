using System;
using System.Reflection;
using BepInEx.Preloader.Core.Patching;
using BepInEx.SplashScreen.Patcher.Common;

[assembly: AssemblyTitle("BepInEx.SplashScreen.Patcher.Bep6")]

namespace BepInEx.SplashScreen.Patcher.Bep6
{
    [PatcherPluginInfo("BepInEx.SplashScreen.Patcher", "BepInEx.SplashScreen", Metadata.Version)]
    public class BepInExSplashScreenPatcher : BasePatcher
    {
        static BepInExSplashScreenPatcher()
        {
            SplashScreenController.SpawnSplash();
        }

        //public BepInExSplashScreenPatcher()
        //{
        //    SplashScreenController.SpawnSplash();
        //}

        //public override void Initialize() { }

        //public override void Finalizer() { }

    }
}
