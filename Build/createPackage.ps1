$env:Path=$env:Path + ";$env:ProgramFiles\MSBuild\14.0\bin"
MSBuild.exe BarionClient.sln /t:Clean,Build /p:Configuration=Release

Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe

.\nuget.exe pack BarionClientLibrary\BarionClientLibrary.nuspec

rm nuget.exe
