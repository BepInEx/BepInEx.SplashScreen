if ($PSScriptRoot -match '.+?\\bin\\?') {
    $dir = $PSScriptRoot + "\"
}
else {
    $dir = $PSScriptRoot + "\bin\"
}

$BIEdir = $dir + "BepInEx\"

$ver = (Get-ChildItem -Path ($BIEdir) -Filter ("*.dll") -Recurse -Force)[0].VersionInfo.FileVersion.ToString() -replace "([\d+\.]+?\d+)[\.0]*$", '${1}'

$copy = $dir + "copy\"

# Compress-Archive -Path ($BIEdir) -Force -CompressionLevel "Optimal" -DestinationPath ($dir + "BepInEx.SplashScreen_" + $ver + ".zip")

Remove-Item -Force -Path ($copy) -Recurse -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path ($copy)
Copy-Item -Path ($BIEdir) -Destination ($copy) -Recurse -Force
Remove-Item -Path ($copy + "\BepInEx\patchers\BepInEx.SplashScreen\BepInEx.SplashScreen.Patcher.BepInEx6.dll")  -Force
Compress-Archive -Path ($copy + "BepInEx") -Force -CompressionLevel "Optimal" -DestinationPath ($dir + "BepInEx.SplashScreen_BepInEx5_v" + $ver + ".zip")

Remove-Item -Force -Path ($copy) -Recurse -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path ($copy)
Copy-Item -Path ($BIEdir) -Destination ($copy) -Recurse -Force
Remove-Item -Path ($copy + "\BepInEx\patchers\BepInEx.SplashScreen\BepInEx.SplashScreen.Patcher.BepInEx5.dll")  -Force
Compress-Archive -Path ($copy + "BepInEx") -Force -CompressionLevel "Optimal" -DestinationPath ($dir + "BepInEx.SplashScreen_BepInEx6_v" + $ver + ".zip")

Remove-Item -Force -Path ($copy) -Recurse -ErrorAction SilentlyContinue
