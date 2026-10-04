# monocle
Dotnet libraries and apps




## Publishing

```powershell
dotnet publish .\apps\SampleConsoleXyzzy\SampleConsoleXyzzy.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```
