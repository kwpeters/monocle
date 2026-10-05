# monocle

## Project Overview

Modern .NET libraries and apps.

- See `global.json` for the SDK version used for development.
- For the target platform .NET version for each project, see the `<TargetFramework>`
  setting in each project's `.csproj` file.

### Project Structure

- **apps/**: Applications
- **libs/**: Class libraries

## Building

```powershell
dotnet build
```

## Testing

- All test projects use xUnit as the testing framework

```powershell
dotnet test
```

## Linting

To make sure the source code conforms to the configured formatting and style:

```powershell
dotnet format --verify-no-changes --severity info
```


## Publishing

```powershell
dotnet publish .\apps\SampleConsoleXyzzy\SampleConsoleXyzzy.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```
