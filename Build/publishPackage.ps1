$env:Path=$env:Path + ";$env:ProgramFiles\NuGet\"

# Download nuget.exe if it's not in the PATH
if ((Get-Command nuget -ErrorAction SilentlyContinue) -eq $null) 
{
	Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe
	Set-Alias nuget .\nuget.exe
}

nuget.exe push .\BarionClient.*.nupkg -Source https://www.nuget.org/api/v2/package

rm .\nuget.exe -ErrorAction SilentlyContinue
