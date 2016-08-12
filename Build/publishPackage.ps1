$env:Path=$env:Path + ";$env:ProgramFiles\NuGet\"

nuget.exe push .\BarionClient.*.nupkg -Source https://www.nuget.org/api/v2/package