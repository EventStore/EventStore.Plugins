#!/usr/bin/env bash
# Nuget packaging script for EventStore.Plugins

set -o errexit
set -o pipefail

function usage {
    echo "Usage:"
    echo "  $0 [<version=0.0.0.0>] [<configuration=Release|Debug>]"
}

function writeLog {
    message=$1
    echo "[$scriptName] - $message"
}

if [[ "$1" == "-help" || "$1" == "--help" ]] ; then
    usage
    exit
fi

baseDir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/../"
outputDir="$baseDir/packages"
scriptName=$(basename ${BASH_SOURCE[0]})
projectPath="$baseDir/src/EventStore.Plugins/EventStore.Plugins.csproj"

version=$1
configuration=$2

if [[ "$version" == "" ]] ; then
    version="0.0.0.0"
    writeLog "Version defaulted to: 0.0.0.0"
else
    writeLog "Version set to: $version"
fi

if [[ "$configuration" == "" ]] ; then
    configuration="Release"
    writeLog "Configuration defaulted to: Release"
else
	if [[ "$configuration" == "Release" || "$configuration" == "Debug" ]]; then
	    writeLog "Configuration set to: $configuration"
	else
		writeLog "Invalid configuration option: $configuration"
		exit 1
	fi
fi


if [ ! -d $outputDir ]; then
  mkdir -p $outputDir;
fi

writeLog "Packaging $projectPath to $outputDir with version $version"
dotnet pack -c "$configuration" -o "$outputDir" "/p:Version=$version" "$projectPath"


# if ((Test-Path $outputDirectory) -eq $false) {
#     New-Item -Path $outputDirectory -ItemType Directory > $null
# }

# Function RunDotnetPack() {
#     [CmdletBinding()]
#     param(
#         [Parameter(Mandatory=$true, Position=0)][string]$ProjectPath
#     )
#     Start-Process -NoNewWindow -Wait -FilePath "dotnet" -ArgumentList @("pack", "-c", $configuration, "-o", $outputDirectory, "/p:Version=$Version", "$ProjectPath")
# }

# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.ClientAPI\EventStore.ClientAPI.csproj")
# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.ClientAPI.Embedded\EventStore.ClientAPI.Embedded.csproj")
# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.Client\EventStore.Client.csproj")
# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.Client.Operations\EventStore.Client.Operations.csproj")

# function detectOS(){
#     unameOut="$(uname -s)"
#     case "${unameOut}" in
#         Linux*)     os=Linux;;
#         Darwin*)    os=MacOS;;
#         *)          os="${unameOut}"
#     esac

#     if [[ "$os" != "Linux" && $os != "MacOS" ]] ; then
#         writeLog "Unsupported operating system: $os. Only Linux and MacOS are supported."
#         exit 1
#     fi
#     OS=$os
# }

# function publishProject {
#     projectDirectory=$1
#     writeLog "Publishing $projectDirectory for $targetRunTime to $PACKAGEDIRECTORY"

#     dotnet publish $projectDirectory -c $Configuration -r $targetRunTime -o $PACKAGEDIRECTORY --no-build /p:Version=$version /p:Platform=x64
# }

# function preCopy {
#     if [[ "$version" == "" ]] ; then
#         VERSIONSTRING="0.0.0.0"
#         writeLog "Version defaulted to: 0.0.0.0"
#     else
#         VERSIONSTRING=$version
#         writeLog "Version set to: $VERSIONSTRING"
#     fi

#     if [[ "$build" == "" ]] ; then
#         BUILD="oss"
#         writeLog "Build defaulted to: oss"
#     elif [[ "$build" != "oss" && "$build" != "commercial" ]] ; then
#         BUILD="oss"
#         writeLog "Invalid build specified: $build. Build set to: oss"
#     else
#         BUILD=$build
#         writeLog "Build set to: $build"
#     fi

#     OUTPUTDIR="$baseDir/../../bin/packaged"
#     [[ -d $OUTPUTDIR ]] || mkdir -p "$OUTPUTDIR"

#     detectOS

#     soext=""
#     if [ "$OS" == "Linux" ]; then
#         soext="so"
#     elif [ "$OS" == "MacOS" ]; then
#         soext="dylib"
#     fi

#     PACKAGENAME="EventStore"
#     if [[ "$BUILD" == "oss" ]]; then
#         PACKAGENAME="$PACKAGENAME-OSS"
#     else
#         PACKAGENAME="$PACKAGENAME-Commercial"
#     fi

#     PACKAGENAME="$PACKAGENAME-$OS"

#     if [[ "$distro" != "" ]]; then
#         PACKAGENAME="$PACKAGENAME-$distro"
#     fi

#     PACKAGENAME="$PACKAGENAME-v$VERSIONSTRING"
#     PACKAGEDIRECTORY="$OUTPUTDIR/$PACKAGENAME"

#     if [[ -d $PACKAGEDIRECTORY ]] ; then
#         rm -rf "$PACKAGEDIRECTORY"
#     fi
#     mkdir "$PACKAGEDIRECTORY"
# }

# function postCopy {
#     local subdir=$1

#     pushd "$OUTPUTDIR" &> /dev/null

#     tar -zcvf "$PACKAGENAME.tar.gz" "$PACKAGENAME"
#     rm -r "$PACKAGEDIRECTORY"

#     [[ -d ../../packages/$subdir ]] || mkdir -p ../../packages/$subdir
#     mv "$PACKAGENAME.tar.gz" ../../packages/$subdir
#     writeLog "Created package: $PACKAGENAME.tar.gz under packages/$subdir"
#     popd &> /dev/null

#     rm -r "$OUTPUTDIR"
# }

# preCopy

# clusterNodeProject="$baseDir/../../oss/src/EventStore.ClusterNode/EventStore.ClusterNode.csproj"
# testClientProject="$baseDir/../../oss/src/EventStore.TestClient/EventStore.TestClient.csproj"
# ldapProject="$baseDir/../../src/EventStore.Auth.Ldaps/EventStore.Auth.Ldaps.csproj"

# publishProject $clusterNodeProject

# if [[ "$BUILD" == "commercial" ]]; then
#     mkdir -p $PACKAGEDIRECTORY/plugins/EventStore.Auth.Ldaps
#     dotnet publish $ldapProject -c $Configuration -r $targetRunTime -o $PACKAGEDIRECTORY/plugins/EventStore.Auth.Ldaps --no-build /p:Version=$version /p:Platform=x64
# fi

# publishProject $testClientProject

# cp "$baseDir/run-node.sh" "$PACKAGEDIRECTORY/run-node.sh"
# mv $PACKAGEDIRECTORY/EventStore.ClusterNode $PACKAGEDIRECTORY/eventstored
# mv $PACKAGEDIRECTORY/EventStore.TestClient $PACKAGEDIRECTORY/testclient

# postCopy "$distro_shortname"


# [CmdletBinding()]
# Param(
#     [Parameter(HelpMessage="NuGet package version number", Mandatory=$true)]
#     [string]$Version
# )

# $baseDirectory = Resolve-Path (Join-Path $PSScriptRoot "..\..\")
# $configuration = "Release"
# $outputDirectory = Join-Path $baseDirectory "packages"

# if ((Test-Path $outputDirectory) -eq $false) {
#     New-Item -Path $outputDirectory -ItemType Directory > $null
# }

# Function RunDotnetPack() {
#     [CmdletBinding()]
#     param(
#         [Parameter(Mandatory=$true, Position=0)][string]$ProjectPath
#     )
#     Start-Process -NoNewWindow -Wait -FilePath "dotnet" -ArgumentList @("pack", "-c", $configuration, "-o", $outputDirectory, "/p:Version=$Version", "$ProjectPath")
# }

# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.ClientAPI\EventStore.ClientAPI.csproj")
# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.ClientAPI.Embedded\EventStore.ClientAPI.Embedded.csproj")
# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.Client\EventStore.Client.csproj")
# RunDotnetPack -ProjectPath (Join-Path $baseDirectory "oss\src\EventStore.Client.Operations\EventStore.Client.Operations.csproj")

