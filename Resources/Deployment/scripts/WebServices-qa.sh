#!/bin/sh
#!/bin/bash
export PATH=$PATH:/usr/bin

if [ -z "$webserverInstance" ]
then
	echo "Which instance of the WebServices would you like to deploy to? (hangoutqa,hangoutqa2):"
	read instance
else
	instance=$webserverInstance
fi

mkdir -p /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/bin 2> /dev/null || true

echo "Compare QA WebServices config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing Web.config"
sourceFileOne=`cygpath -wa /cygdrive/c/snapshots/$1/webservices/Web.config` 
destFileOne=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.services/Web.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileOne $destFileOne

echo "----> Comparing WebServices.Service.exe.config"
sourceFileTwo=`cygpath -wa /cygdrive/c/snapshots/$1/webservices/bin/WebServices.Service.exe.config` 
destFileTwo=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.services/bin/WebServices.Service.exe.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileTwo $destFileTwo

echo "----> Comparing WebServices.Console.exe.config"
sourceFileThree=`cygpath -wa /cygdrive/c/snapshots/$1/webservices/bin/WebServices.Console.exe.config` 
destFileThree=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.services/bin/WebServices.Console.exe.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileThree $destFileThree


echo "--> COPYING Config files to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.services/Web.config /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.services/bin/WebServices.Service.exe.config /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/bin
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.services/bin/WebServices.Console.exe.config /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/bin

echo "--> COPYING Config files to QA WebServices "
scp /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/Web.config deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.services
scp /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/bin/WebServices.Service.exe.config deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.services/bin
scp /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.services/bin/WebServices.Console.exe.config deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.services/bin

fi

serviceName="WebServicesService"
if [ $instance == "hangoutqa2" ]
then
serviceName="WebServicesService2"
fi

echo "Copy WebServices files to QA?: "
read deployWebServices
if [ $deployWebServices == "y" -o  $deployWebServices == "yes" ]
then


ssh deploy@web016.hangoutops.net "mkdir -p /cygdrive/e/wwwroot/net.$instance.services/bin 2> /dev/null || true  "

ssh deploy@web016.hangoutops.net "net stop $serviceName"
echo "--> rsync copying WebServices files to QA "
rsync -crvvz --filter "- .svn/" --filter "- *.config" /cygdrive/c/snapshots/$1/webservices/* deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.services/
ssh deploy@web016.hangoutops.net "net start $serviceName"
echo "--> DONE copying WebServices files to QA "

fi

echo "###### WebServices QA Deploy Complete ######"
echo " "
echo " "
