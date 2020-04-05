#!/bin/sh
#!/bin/bash


echo "Copy Assets to QA?: "
read deployAssets
if [ $deployAssets == "y" -o  $deployAssets == "yes" ]
then

ssh deploy@web016.hangoutops.net "mkdir -p /cygdrive/e/wwwroot/net.hangoutqa.dl/Assets 2> /dev/null || true  "

echo "--> rsync copying Assets to QA "
rsync -crvvz  /cygdrive/c/snapshots/$1/Assets/* deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.hangoutqa.dl/Assets
echo "--> DONE copying Assets to QA "
fi


echo "###### Assets to QA Deploy Complete ######"
echo " "
echo " "




