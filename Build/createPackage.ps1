param(
	[bool]$rebuild = $true,
	[string]$version = "1.0.0"
)

if($rebuild)
{
	$env:Path=$env:Path + ";$env:ProgramFiles\MSBuild\14.0\bin"
	$env:Path=$env:Path + ";$env:ProgramFiles (x86)\MSBuild\14.0\bin"
	MSBuild.exe BarionClient.sln /t:Clean,Build /p:Configuration=Release
    dotnet restore
    dotnet build BarionClientLibrary\project.json -c Release
}

# Download nuget.exe if it's not in the PATH
if ((Get-Command nuget -ErrorAction SilentlyContinue) -eq $null) 
{
	Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe
	Set-Alias nuget .\nuget.exe
}

nuget pack BarionClientLibrary\BarionClientLibrary.nuspec -Version $version

rm .\nuget.exe -ErrorAction SilentlyContinue
