$PATH = Resolve-Path -Path "..\src"
Get-ChildItem -Path $PATH -Include obj -Recurse -Force | Remove-Item -Force -Recurse
Get-ChildItem -Path $PATH -Include bin -Recurse -Force | Remove-Item -Force -Recurse
Get-ChildItem -Path $PATH -Include BundleArtifacts -Recurse -Force | Remove-Item -Force -Recurse