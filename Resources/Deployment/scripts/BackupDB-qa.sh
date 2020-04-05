#!/bin/sh
#!/bin/bash


echo "Backup QA DB?: "
read backUpDb
if [ $backUpDb == "y" -o  $backUpDb == "yes" ]
then

touch "/cygdrive/c/snapshots/$1/deployhistory.txt"
echo "Backup of QA Executed at `date -u`" >> /cygdrive/c/snapshots/$1/deployhistory.txt
/cygdrive/c/Program\ Files/EMS/SQL\ Studio\ for\ MySQL/DB\ Extract/MyExtractC.exe "c:\deployment\repo\Resources\Deployment\database\Extract_QaDB.ext" -B

fi

echo "###### QA DB Backup Complete ######"
echo " "
echo " "

