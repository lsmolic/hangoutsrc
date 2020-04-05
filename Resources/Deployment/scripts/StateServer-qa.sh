#!/bin/sh
#!/bin/bash

if [ -z "$stateserverInstance" ]
then
	echo "Which instance of the StateServer would you like to deploy to? (stateserver,stateserver2):"
	read instance
else
	instance=$stateserverInstance
fi


mkdir -p /cygdrive/c/snapshots/$1/configcompare/qa/qaserver034/$instance 2> /dev/null | true

echo "Compare QA StateServer config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing StateServerConsole.exe.config"
sourceFile=`cygpath -wa /cygdrive/c/snapshots/$1/stateserver/bin/release/StateServerConsole.exe.config` 
destFile=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/qaserver034/$instance/StateServerConsole.exe.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFile $destFile

echo "--> COPYING Config file to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/qaserver034/$instance/StateServerConsole.exe.config /cygdrive/c/snapshots/$1/configcompare/qa/qaserver034/$instance/

echo "--> COPYING Config file to StateServer"
scp /cygdrive/c/snapshots/$1/configcompare/qa/qaserver034/$instance/StateServerConsole.exe.config deploy@qaserver034.hangoutops.net:~/$instance/bin/release
fi

echo "Copy StateServer files to QA?: "
read deployStateServer
if [ $deployStateServer == "y" -o  $deployStateServer == "yes" ]
then

echo "-->Deploying stateserver files to QA "

ssh deploy@qaserver034.hangoutops.net "mkdir ~/$instance 2> /dev/null || true"
ssh deploy@qaserver034.hangoutops.net "mkdir ~/$instance/bin 2> /dev/null || true"
ssh deploy@qaserver034.hangoutops.net "mkdir ~/$instance/bin/release 2> /dev/null || true"

rsync -crvvz --filter "- .svn/" --filter "- *.config" /cygdrive/c/snapshots/$1/stateserver/bin/release/* deploy@qaserver034.hangoutops.net:~/$instance/bin/release

ssh deploy@qaserver034.hangoutops.net "~/restart_server.sh"

fi

echo "###### StateServer QA Deploy Complete ######"
echo " "
echo " "

