#!/bin/sh
#!/bin/bash
export PATH=$PATH:/usr/bin

echo "Which server would you like to deploy to? (web135,web136):"
read serverName

mkdir -p /cygdrive/c/snapshots/$1/configcompare/live/$serverName/website 2> /dev/null || true

echo "Compare LIVE WebSite config files with snapshot?: "
read compareConfigs
if [ $compareConfigs == "y" -o  $compareConfigs == "yes" ]
then

echo "----> Comparing Web.config"
sourceFileOne=`cygpath -wa /cygdrive/c/snapshots/$1/website/Web.config` 
destFileOne=`cygpath -wa /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.new.live/Web.config`
/cygdrive/c/Program\ Files/Araxis/Araxis\ Merge/Merge.exe $sourceFileOne $destFileOne

echo "--> COPYING Config files to Snapshot configcompare dir "
cp /cygdrive/c/deployment/repo/Resources/Deployment/configs/live/$serverName/net.hangout.new.live/Web.config /cygdrive/c/snapshots/$1/configcompare/live/$serverName/website

echo "--> COPYING Config files back to LIVE WebSite "
scp /cygdrive/c/snapshots/$1/configcompare/live/$serverName/website/Web.config deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.new.live/
fi

echo "Copy WebSite files to LIVE?: "
read deployWebSite
if [ $deployWebSite == "y" -o  $deployWebSite == "yes" ]
then
ssh deploy@$serverName.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangout.new.live/ 2> /dev/null || true  "

echo "--> rsync copying WebSite files to LIVE "
rsync -crvvz --filter "- .svn/" --filter "- *.config" --exclude "Assets/" /cygdrive/c/snapshots/$1/website/* deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.new.live/
echo "--> DONE copying WebSite files to LIVE "
fi

echo "###### WebSite LIVE Deploy Complete ######"
echo " "
echo " "
