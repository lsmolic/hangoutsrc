#!/bin/sh
#!/bin/bash
export PATH=$PATH:/usr/bin

echo "Which server would you like to deploy to? (web135,web136):"
read serverName

mkdir -p /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices/bin 2> /dev/null || true

echo "Compare LIVE WebServices config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing Web.config"
sourceFileOne=`cygpath -wa /cygdrive/c/snapshots/$1/webservices/Web.config` 
destFileOne=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.services.new.live/Web.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileOne $destFileOne

echo "----> Comparing WebServices.Service.exe.config"
sourceFileTwo=`cygpath -wa /cygdrive/c/snapshots/$1/webservices/bin/WebServices.Service.exe.config` 
destFileTwo=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.services.new.live/bin/WebServices.Service.exe.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileTwo $destFileTwo

echo "----> Comparing WebServices.Console.exe.config"
sourceFileThree=`cygpath -wa /cygdrive/c/snapshots/$1/webservices/bin/WebServices.Console.exe.config` 
destFileThree=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.services.new.live/bin/WebServices.Console.exe.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileThree $destFileThree

echo "--> COPYING Config files to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.services.new.live/Web.config /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.services.new.live/bin/WebServices.Service.exe.config /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices/bin
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.services.new.live/bin/WebServices.Console.exe.config /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices/bin

echo "--> COPYING Config files back to LIVE WebServices "
scp /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices/Web.config deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.services.new.live
scp /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices/bin/WebServices.Service.exe.config deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.services.new.live/bin
scp /cygdrive/c/snapshots/$1/configcompare/live/$serverName/webservices/bin/WebServices.Console.exe.config deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.services.new.live/bin

fi

echo "Copy WebServices files to LIVE?: "
read deployWebServices
if [ $deployWebServices == "y" -o  $deployWebServices == "yes" ]
then

ssh deploy@$serverName.hangoutops.net "mkdir -p /cygdrive/e/wwwroot/net.hangout.services.new.live/bin 2> /dev/null || true  "

ssh deploy@$serverName.hangoutops.net "net stop WebServicesService"
echo "--> rsync copying WebServices files to LIVE "
rsync -crvvz --filter "- .svn/" --filter "- *.config" /cygdrive/c/snapshots/$1/webservices/* deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.services.new.live/
ssh deploy@$serverName.hangoutops.net "net start WebServicesService"

echo "--> DONE copying WebServices files to LIVE "

fi


echo "###### WebServices LIVE Deploy Complete ######"
echo " "
echo " "
