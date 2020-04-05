#!/bin/sh
#!/bin/bash

echo " ################################# "
echo " ######  BEGINNING DEPLOY TO LIVE  ####### "
echo " ################################# "

echo "DID YOU RUN THE DATA COMPARE YET? "
read dataCompare

if [ $dataCompare == "y" -o "yes" ]
then
./WebServices-live.sh $1; 
./WebSite-live.sh $1; 
./Assets-live.sh $1;
./StateServer-live.sh $1; 
#./BackupDB-live.sh $1; 
#./StructureCompare-DB-live.sh $1;
#./DataCompare-DB-live.sh $1;

echo " "
echo " "
echo " "
echo " "
echo "#################################"
echo "#################################"
echo "~~~.....## DEPLOY TO LIVE COMPLETE ##.....~~~"
echo "#################################"
echo "#################################"
echo " "
echo " "
echo " "
echo " "

fi





