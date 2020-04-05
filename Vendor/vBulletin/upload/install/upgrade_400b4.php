<?php
/*======================================================================*\
|| #################################################################### ||
|| # vBulletin 4.0.0 Beta 4 - Licence Number VBFB74B3F1
|| # ---------------------------------------------------------------- # ||
|| # Copyright ©2000-2009 vBulletin Solutions Inc. All Rights Reserved. ||
|| # This file may not be redistributed in whole or significant part. # ||
|| # ---------------- VBULLETIN IS NOT FREE SOFTWARE ---------------- # ||
|| # http://www.vbulletin.com | http://www.vbulletin.com/license.html # ||
|| #################################################################### ||
\*======================================================================*/

error_reporting(E_ALL & ~E_NOTICE);

// This is a template file for creating new upgrade scripts. To use it, 
// simply alter the following definitions.

// You also need to create the appropriate array in 
// $upgrade_phrases['upgradeX.php']['steps'] in the upgrade_language_en.php
// for the step titles 

// After that write the code for the steps.

define('THIS_SCRIPT', 'upgrade_400b4.php');
define('VERSION', '4.0.0 Beta 4');
define('PREV_VERSION', '4.0.0 Beta 3');

$phrasegroups = array();
$specialtemplates = array();


// require the code that makes it all work...
require_once('./upgradecore.php');

// #############################################################################
// welcome step
if ($vbulletin->GPC['step'] == 'welcome')
{	
	if ($vbulletin->options['templateversion'] == PREV_VERSION)
	{
		echo "<blockquote><p>&nbsp;</p>";
		echo "$vbphrase[upgrade_start_message]";
		echo "<p>&nbsp;</p></blockquote>";
	}
	else
	{
		echo "<blockquote><p>&nbsp;</p>";
		echo "$vbphrase[upgrade_wrong_version]";
		echo "<p>&nbsp;</p></blockquote>";
		print_upgrade_footer();
	}
}

// #############################################################################
// Advertising step 
if ($vbulletin->GPC['step'] == 1)
{
	
	$upgrade->run_query(
		sprintf($vbphrase['create_table'], TABLE_PREFIX . "ad"),
		"CREATE TABLE " . TABLE_PREFIX . "ad (
			adid INT UNSIGNED NOT NULL auto_increment,
			title VARCHAR(250) NOT NULL DEFAULT '',
			adlocation VARCHAR(250) NOT NULL DEFAULT '',
			displayorder INT UNSIGNED NOT NULL DEFAULT '0',
			active SMALLINT UNSIGNED NOT NULL DEFAULT '0',
			snippet MEDIUMTEXT,
			PRIMARY KEY (adid),
			KEY active (active)
		)",
		MYSQL_ERROR_TABLE_EXISTS
	);
	
	$upgrade->run_query(
		sprintf($vbphrase['create_table'], TABLE_PREFIX . "adcriteria"),
		"CREATE TABLE " . TABLE_PREFIX . "adcriteria (
			adid INT UNSIGNED NOT NULL DEFAULT '0',
			criteriaid VARCHAR(250) NOT NULL DEFAULT '',
			condition1 VARCHAR(250) NOT NULL DEFAULT '',
			condition2 VARCHAR(250) NOT NULL DEFAULT '',
			condition3 VARCHAR(250) NOT NULL DEFAULT '',
			PRIMARY KEY (adid,criteriaid)
		)
		",
		MYSQL_ERROR_TABLE_EXISTS
	);
	

	if (!$upgrade->field_exists('language', 'phrasegroup_advertising'))
	{
		$upgrade->run_query(
			sprintf($vbphrase['update_table'], TABLE_PREFIX . "advertising"),
			"ALTER TABLE " . TABLE_PREFIX . "language ADD phrasegroup_advertising mediumtext not null"
		);
	}
	
	if (!$db->query_first("SELECT * FROM " . TABLE_PREFIX . "phrasetype WHERE fieldname = 'advertising'"))
	{
		$upgrade->run_query(
			sprintf($vbphrase['update_table'], TABLE_PREFIX . "phrasetype"),
			"INSERT INTO " . TABLE_PREFIX . "phrasetype
			VALUES
				('advertising', 'Advertising', 3, '', 0)
			"
		);
	}	
		
	$upgrade->execute();
}


// #############################################################################
// FINAL step (notice the SCRIPTCOMPLETE define)
if ($vbulletin->GPC['step'] == 2)
{
	// tell log_upgrade_step() that the script is done
	define('SCRIPTCOMPLETE', true);
}

// #############################################################################

print_next_step();
print_upgrade_footer();

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # CVS: $RCSfile$ - $Revision: 33227 $
|| ####################################################################
\*======================================================================*/
?>
