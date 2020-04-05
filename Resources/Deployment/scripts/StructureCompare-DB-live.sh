#a!/bin/sh
#!/bin/bash


echo "Compare the Structure from DEV to LIVE DB?: "

read backUpDb
if [ $backUpDb == "y" -o  $backUpDb == "yes" ]
then

mkdir /cygdrive/c/deployment/repo/Resources/Deployment/database/output 2>/dev/null || true

/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Acoounts.mcp" "output\Compare_Accounts_Dev2QA.sql"  /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Avatar.mcp" "output\Compare_Accounts_Dev2QA.sql"  /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Rooms.mcp" "output\Compare_Accounts_Dev2QA.sql"  /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Inventory.mcp" "output\Compare_Accounts_Dev2QA.sql"  /E 
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Logging.mcp" "output\Compare_Accounts_Dev2QA.sql"  /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_MiniGame.mcp" "output\Compare_Accounts_Dev2QA.sql"  /E

fi

echo "###### Structure Comparison from DEV to LIVE DB Complete ######"
echo " "
echo " "

