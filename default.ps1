properties {
    $baseDirectory  = resolve-path .
    $buildDirectory = ($buildDirectory, "$baseDirectory\build") | select -first 1
    $version = "0.0.16"
    $releaseNotes = "Built against Selenium Webdriver 2.7"

    $shortDescription = "An extension to Selenium to support Sizzle based CSS selectors.  Also, an extension method for waiting."
}

import-module .\tools\PSUpdateXML.psm1
. .\psake_ext.ps1

task default -depends TraceSourceControlCommit,Build,RunTests,BuildNuget

task TraceSourceControlCommit {
    git log -1 --oneline | % { "Current commit: " + $_ }
}

task Cleanup {
    if (test-path $buildDirectory) {
        rm $buildDirectory -recurse
    }
}

task GenerateAssemblyInfo {
	
	$projectFiles = ls -path $base_dir -include *.csproj -recurse

    $projectFiles | write-host
	foreach($projectFile in $projectFiles) {
		
		$projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
		$projectName = [System.IO.Path]::GetFileName($projectDir)
		$asmInfo = [System.IO.Path]::Combine($projectDir, [System.IO.Path]::Combine("Properties", "AssemblyInfo.cs"))
				
		Generate-Assembly-Info `
			-file $asmInfo `
			-title "$projectName $version.0" `
			-description $shortDescription `
			-company "n/a" `
			-product "SizSelCsZzz $version.0" `
			-version "$version.0" `
			-fileversion "$version.0" `
			-copyright "Copyright © Frank Schwieterman 2011" `
			-clsCompliant "false"
	}
}

task Build -depends Cleanup,GenerateAssemblyInfo {
    $v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    $dearlySolution = "$baseDirectory\dearly.sln"

    exec { &"C:\Windows\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" SizSelCsZzz.sln /T:"Clean,Build" /property:OutDir="$buildDirectory\" }    
}

task RunTests -depends Build {
    exec { & "$baseDirectory\packages\NUnit.2.5.10.11092\tools\nunit-console.exe" "$buildDirectory\SizSelCsZzz.Test.dll" -xml:"$buildDirectory\TestResults.xml" }
}

task BuildNuget -depends RunTests {

    $nugetTarget = "$buildDirectory\nuget"

    $null = mkdir "$nugetTarget\lib\"
    $null = mkdir "$nugetTarget\tools\"

    cp "$buildDirectory\SizSelCsZzz.dll" "$nugetTarget\lib\"
    cp "$buildDirectory\SizSelCsZzz.pdb" "$nugetTarget\lib\"

    $old = pwd
    cd $nugetTarget
    
    ..\..\tools\nuget.exe spec -a ".\lib\SizSelCsZzz.dll"

    update-xml "SizSelCsZzz.nuspec" {

        for-xml "//package/metadata" {
            set-xml -exactlyOnce "//version" "$version.0"
            set-xml -exactlyOnce "//owners" "fschwiet"
            set-xml -exactlyOnce "//authors" "Frank Schwieterman"
            set-xml -exactlyOnce "//description" $shortDescription

            set-xml -exactlyOnce "//licenseUrl" "https://github.com/fschwiet/SizSelCsZzz/blob/master/LICENSE.txt"
            set-xml -exactlyOnce "//projectUrl" "https://github.com/fschwiet/SizSelCsZzz/"
            remove-xml -exactlyOnce "//iconUrl"
            set-xml -exactlyOnce "//tags" "Selenium WebDriver Browser Automation"

            set-xml -exactlyOnce "//dependencies" ""
            append-xml -exactlyOnce "//dependencies" "<dependency id=`"Newtonsoft.Json`" version=`"4.0`" />"
            append-xml -exactlyOnce "//dependencies" "<dependency id=`"Selenium.WebDriver`" version=`"2.6`" />"
            append-xml -exactlyOnce "//dependencies" "<dependency id=`"Selenium.Support`" version=`"2.6`" />"

            append-xml "." "<releaseNotes>$releaseNotes</releaseNotes>";
            append-xml "." "<summary>$shortDescription  This library requires .NET 4.</summary>"
        }
    }

    ..\..\tools\nuget pack "SizSelCsZzz.nuspec"

    cd $old
}
