param(
	[bool]$rebuild = $true
)

if($rebuild)
{
	$env:Path=$env:Path + ";$env:ProgramFiles\MSBuild\14.0\bin"
	MSBuild.exe BarionClient.sln /t:Clean,Build /p:Configuration=Release
}

# Download nuget.exe if it's not in the PATH
if ((Get-Command nuget -ErrorAction SilentlyContinue) -eq $null) 
{
	Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe
	Set-Alias nuget .\nuget.exe
}

nuget pack BarionClientLibrary\BarionClientLibrary.nuspec

rm .\nuget.exe -ErrorAction SilentlyContinue
