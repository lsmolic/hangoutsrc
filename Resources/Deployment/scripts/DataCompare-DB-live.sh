#!/bin/sh
#!/bin/bash


echo "Compare the Data from DEV to LIVE DB?: "

read backUpDb
if [ $backUpDb == "y" -o  $backUpDb == "yes" ]
then

/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_LiveAvatars.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Avatar.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_LiveRooms.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Rooms.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_LiveInventory.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Inventory.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_LiveLogging.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Logging.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_LiveMiniGames.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_MiniGame.edc" -B

fi

echo "###### Data Comparison from DEV to LIVE DB Complete ######"
echo " "
echo " "

