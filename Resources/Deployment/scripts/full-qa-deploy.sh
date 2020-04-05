#!/bin/sh
#!/bin/bash

echo " ################################# "
echo " #######  BEGINNING DEPLOY TO QA  ####### "
echo " ################################# "

select_webserver_instance() 
{
	echo "Which QA webserver instance do you wish to deploy?"
	select webInstance in hangoutqa hangoutqa2; do
		if [[ -n "$webInstance" ]]; 
			then 
			export webserverInstance=$webInstance
			break
		else
			echo 'invalid'
			select_webserver_instance
		fi
	done
}

select_stateserver_instance() 
{
	echo "Which QA stateserver instance do you wish to deploy?"
	select stateInstance in stateserver stateserver2; do
		if [[ -n "$stateInstance" ]]; 
		then 
			export stateserverInstance=$stateInstance
			break
		else
			echo 'invalid'
			select_webserver_instance
		fi
	done
}

#echo "DID YOU RUN THE DATA COMPARE YET? "
#read dataCompare

#./StructureCompare-DB-qa.sh $1;


select_webserver_instance
./WebServices-qa.sh $1; 
./WebSite-qa.sh $1; 
./SecureWebSite-qa.sh $1;

select_stateserver_instance
./StateServer-qa.sh $1; 

./Assets-qa.sh $1;

#no need to back up db
#Data compare can't work from here because truncate doesn't have a command line option.. PISSY PISSY PISSY
#./DataCompare-DB-qa.sh $1; 

echo " "
echo " "
echo " "
echo " "
echo "#################################"
echo "#################################"
echo "~~~......## DEPLOY TO QA COMPLETE ##......~~~"
echo "#################################"
echo "#################################"
echo " "
echo " "
echo " "
echo " "