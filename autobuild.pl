#!/usr/bin/perl
use strict;
use warnings;

#author: matt - respect the awesome

#FOR WINDOWS!!!!
my $projectPath = "C:/Documents and Settings/build/.hudson/jobs/hb/workspace/Architecture/UnityProjects/NewSceneArchitecture";
#OLD PATH
#my $projectPath = "C:/Documents\ and\ Settings/b/Desktop/work/HangoutSrcRepo/Branches/UnityScriptPrototype/Architecture/UnityProjects/NewSceneArchitecture";
#
#my $commandToRun = "C:\Program Files\Unity\Editor\Unity.exe";
my $commandToRun = "/cygdrive/c/Program\ Files/Unity/Editor/Unity.exe";

#FOR OSX!!!!
#TODO: you must fill these two fields out!  examples are commented next to the variable declarations
#my $projectPath = "/Users/deploy/hudson/data/jobs/hangout-build/workspace/Architecture/UnityProjects/NewSceneArchitecture";
#my $commandToRun = "/Applications/Unity/Unity.app/Contents/MacOS/Unity";

my $scriptAssemblyPath = $projectPath . "/Library/ScriptAssemblies";

my $scriptAssembly = $scriptAssemblyPath . "/Assembly\ -\ CSharp.dll";
my $editorScriptAssembly = $scriptAssemblyPath . "/Assembly\ -\ CSharp\ -\ Editor.dll";


#delete all the files in the ScriptAssemblies folder
my $filesDeleted = unlink $scriptAssembly, $editorScriptAssembly;
print("$filesDeleted files were deleted!\n");

#try to make a build
print("BUILD ME!!\n");
#print("\"$commandToRun\" -quit -projectPath \"$projectPath\" -nographics -batchmode -executeMethod BuildScript.CommandLineBuild\n");
my $result = `"$commandToRun" -projectPath \"$projectPath\" -executeMethod BuildScript.CommandLineBuild -quit`;
print("DONE WITH BUILD!!\n");
#print the Unity Editor log file to the output because this is useful for seeing any errors that occur
#open(UNITYEDITORLOGFILE, "</Users/deploy/Library/Logs/Unity/Editor.log"); # open for input
open(UNITYEDITORLOGFILE, "/cygdrive/c/Documents\ and\ Settings/build/Local\ Settings/Application\ Data/Unity/Editor/Editor.log"); # open for input
my(@lines) = <UNITYEDITORLOGFILE>; # read file into list
my($line);
foreach $line (@lines) # loop thru list
{
    print "$line"; # print in sort order
}
close(UNITYEDITORLOGFILE);

print("\n");

#if the assemblies were not generated, the build failed
if (!(-e $scriptAssembly && -e $editorScriptAssembly))
{
	print("build FAILED!\n");
	exit 1;
}
#else the build succeded
else
{
	print("build SUCCESS!\n");
	exit 0;
}


