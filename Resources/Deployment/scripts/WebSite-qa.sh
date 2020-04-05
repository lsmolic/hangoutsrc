#!/bin/sh
#!/bin/bash
export PATH=$PATH:/usr/bin

if [ -z "$webserverInstance" ]
then
	echo "Which instance of the WebSite would you like to deploy to? (hangoutqa,hangoutqa2):"
	read instance
else
	instance=$webserverInstance
fi

mkdir -p /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.www 2> /dev/null || true

echo "Compare QA WebSite config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing Web.config"
sourceFileOne=`cygpath -wa /cygdrive/c/snapshots/$1/website/Web.config` 
destFileOne=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.www/Web.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileOne $destFileOne

echo "--> COPYING Config files to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/qa/web016/net.$instance.www/Web.config /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.www

echo "--> COPYING Config files back to QA WebSite "
scp /cygdrive/c/snapshots/$1/configcompare/qa/web016/net.$instance.www/Web.config deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.www/
fi

echo "Copy WebSite files to QA?: "
read deployWebSite
if [ $deployWebSite == "y" -o  $deployWebSite == "yes" ]
then
ssh deploy@web016.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.$instance.www/ 2> /dev/null || true  "

echo "--> rsync copying WebSite files to QA "
rsync -crvvz --no-p --filter "- .svn/" --filter "- *.config" --exclude "Assets/" /cygdrive/c/snapshots/$1/website/* deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.$instance.www/
echo "--> DONE copying WebSite files to QA "
fi

echo "###### WebSite QA Deploy Complete ######"
echo " "
echo " "
