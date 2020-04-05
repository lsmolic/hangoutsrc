#!/bin/sh
#!/bin/bash
export PATH=$PATH:/usr/bin

echo "Which server would you like to deploy to? (web135,web136):"
read serverName

mkdir -p /cygdrive/c/snapshots/$1/configcompare/qa/net.$instance.secure 2> /dev/null || true

echo "Compare QA Secure Site config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing Secure Site Web.config"
sourceFileOne=`cygpath -wa /cygdrive/c/snapshots/$1/securewebsite/Web.config` 
destFileOne=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.secure/Web.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileOne $destFileOne

echo "--> COPYING Secure Site Config files to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.secure/Web.config /cygdrive/c/snapshots/$1/configcompare/qa/net.$instance.secure

echo "--> COPYING Config files back to QA Secure Site "
scp /cygdrive/c/snapshots/$1/configcompare/qa/net.$instance.secure/Web.config deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.secure/

fi


echo "Copy Secure Site files to QA?: "
read deployWebSite
if [ $deployWebSite == "y" -o  $deployWebSite == "yes" ]
then
ssh deploy@web016.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.$instance.secure/ 2> /dev/null || true  "

echo "--> rsync copying SecureWebSite files to QA "
rsync -crvvz --no-p --filter "- .svn/" --filter "- *.config" /cygdrive/c/snapshots/$1/securewebsite/* deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.secure/
echo "--> DONE copying SecureWebSite files to QA "
fi

echo "###### Secure Site QA Deploy Complete ######"
echo " "
echo " "
