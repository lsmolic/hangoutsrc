#a!/bin/sh
#!/bin/bash


echo "Compare the Structure from DEV to QA DB?: "

read backUpDb
if [ $backUpDb == "y" -o  $backUpDb == "yes" ]
then

mkdir /cygdrive/c/deployment/repo/Resources/Deployment/database/output 2>/dev/null || true

/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Accounts_Dev2QA.mcp" "output\Compare_Accounts_Dev2QA.sql" /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Avatar_Dev2QA.mcp" "output\Compare_Avatar_Dev2QA.sql" /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Rooms_Dev2QA.mcp" "output\Compare_Rooms_Dev2QA.sql" /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Inventory_Dev2QA.mcp" "output\Compare_Inventory_Dev2QA.sql" /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_Logging_Dev2QA.mcp" "output\Compare_Logging_Dev2QA.sql" /E
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Comparer/MyComparerC.exe "c:\deployment\repo\Resources\Deployment\database\Compare_MiniGame_Dev2QA.mcp" "output\Compare_MiniGame_Dev2QA.sql" /E

fi

echo "###### Structure Comparison from DEV to QA DB Complete ######"
echo " "
echo " "

