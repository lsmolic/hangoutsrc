#!/bin/sh
#!/bin/bash


echo "Compare the Data from DEV to QA DB?: "

read backUpDb
if [ $backUpDb == "y" -o  $backUpDb == "yes" ]
then

/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_QAAvatars.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Avatar_Dev2QA.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_QAAvatars.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Rooms_Dev2QA.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_QAInventory.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Inventory_Dev2QA.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_QALogging.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_Logging_Dev2QA.edc" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/SQL\ Manager/MyManager.exe "c:\deployment\repo\Resources\Deployment\database\Truncate_QAMiniGames.mt" -B
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/Data\ Comparer/MyDataComparerC.exe "c:\deployment\repo\Resources\Deployment\database\DataCompare_MiniGame_Dev2QA.edc" -B

fi

echo "###### Data Comparison from DEV to QA DB Complete ######"
echo " "
echo " "

