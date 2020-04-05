#!/bin/sh
#!/bin/bash

echo "Which server would you like to deploy to? (web135,web136):"
read serverName

echo "Copy Assets to LIVE?: "
read deployAssets
if [ $deployAssets == "y" -o  $deployAssets == "yes" ]
then

ssh deploy@$serverName.hangoutops.net "mkdir -p /cygdrive/e/wwwroot/net.hangout.dl.live/Assets 2> /dev/null || true  "

echo "--> rsync copying Assets to LIVE "
rsync -crvvz  /cygdrive/c/snapshots/$1/Assets/* deploy@$serverName.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.dl.live/Assets
echo "--> DONE copying Assets to LIVE "
fi


echo "###### Assets to LIVE Deploy Complete ######"
echo " "
echo " "




