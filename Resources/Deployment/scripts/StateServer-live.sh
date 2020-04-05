#!/bin/sh
#!/bin/bash

echo "Which server would you like to deploy to? (liveserver032,liveserver033):"
read serverName

mkdir -p /cygdrive/c/snapshots/$1/configcompare/live/$serverName 2> /dev/null | true

echo "Compare LIVE StateServer config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing StateServerConsole.exe.config"
sourceFile=`cygpath -wa /cygdrive/c/snapshots/$1/stateserver/bin/release/StateServerConsole.exe.config` 
destFile=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/StateServerConsole.exe.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFile $destFile

echo "--> COPYING Config file to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/StateServerConsole.exe.config /cygdrive/c/snapshots/$1/configcompare/live/$serverName/

echo "--> COPYING Config file back to StateServer"
scp /cygdrive/c/snapshots/$1/configcompare/live/$serverName/StateServerConsole.exe.config deploy@$serverName.hangoutops.net:~/stateserver/bin/release/
fi

echo "Copy StateServer files to LIVE?: "
read deployStateServer
if [ $deployStateServer == "y" -o  $deployStateServer == "yes" ]
then

echo "-->Deploying stateserver files to LIVE "


ssh deploy@$serverName.hangoutops.net "mkdir -p ~/stateserver/bin/release 2> /dev/null || true"

rsync -crvvz --filter "- .svn/" --filter "- *.config" /cygdrive/c/snapshots/$1/stateserver/bin/release/* deploy@$serverName.hangoutops.net:~/stateserver/bin/release/

ssh deploy@$serverName.hangoutops.net "~/stateserver/restart_server.sh"

fi

echo "###### StateServer LIVE Deploy Complete ######"
echo " "
echo " "


