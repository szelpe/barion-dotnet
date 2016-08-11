$env:Path=$env:Path + ";$env:ProgramFiles\MSBuild\14.0\bin"
MSBuild.exe .\BarionClient.sln /t:Clean,Build /p:Configuration=Release

mkdir -Force Package\lib\net45
cp .\BarionClientLibrary\bin\Release\BarionClient.* .\Package\lib\net45
cp BarionClient.nuspec .\Package\

Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe

.\nuget.exe pack .\Package\BarionClient.nuspec

rm nuget.exe
rm -Recurse -Force .\Package