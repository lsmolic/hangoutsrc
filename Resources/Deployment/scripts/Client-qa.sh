#!/bin/sh
#!/bin/bash


echo "Copy UnityClient to QA?: "
read deployUnityClient
if [ $deployUnityClient == "y" -o  $deployUnityClient == "yes" ]
then
ssh deploy@web016.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangoutqa.www/ 2> /dev/null || true  "
ssh deploy@web016.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangoutqa.www/Assets 2> /dev/null || true  "
ssh deploy@web016.hangoutops.net "mkdir /cygdrive/e/wwwroot/net.hangoutqa.www/Assets/Unity 2> /dev/null || true  "

echo "--> rsync copying UnityClient to QA "
rsync -crvvz  /cygdrive/c/snapshots/$1/website/Assets/Unity/hangout.unity3d deploy@web016.hangoutops.net:/cygdrive/e/wwwroot/net.hangoutqa.www/Assets/Unity
echo "--> DONE copying UnityClient to QA "
fi


echo "###### UnityClient to QA Deploy Complete ######"
echo " "
echo " "




