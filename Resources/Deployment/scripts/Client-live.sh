#!/bin/sh
#!/bin/bash


echo "Copy UnityClient to LIVE?: "
read deployUnityClient
if [ $deployUnityClient == "y" -o  $deployUnityClient == "yes" ]
then
ssh deploy@web135.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangout.www/ 2> /dev/null || true  "
ssh deploy@web135.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangout.www/Assets 2> /dev/null || true  "
ssh deploy@web135.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangout.www/Assets/Unity 2> /dev/null || true  "

echo "--> rsync copying UnityClient to LIVE "
rsync -crvvz  /cygdrive/c/snapshots/$1/website/Assets/Unity/hangout.unity3d deploy@web135.hangoutops.net:/cygdrive/e/wwwroot/net.hangout.www/Assets/Unity
echo "--> DONE copying UnityClient to LIVE "
fi


echo "###### UnityClient to LIVE Deploy Complete ######"
echo " "
echo " "




