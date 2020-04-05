<?php
/*======================================================================*\
|| #################################################################### ||
|| # vBulletin Project Tools 4.0.0 Beta 4 - Licence Number VBFB74B3F1
|| # ---------------------------------------------------------------- # ||
|| # Copyright �2000-2009 vBulletin Solutions Inc. All Rights Reserved. ||
|| # This file may not be redistributed in whole or significant part. # ||
|| # ---------------- VBULLETIN IS NOT FREE SOFTWARE ---------------- # ||
|| # http://www.vbulletin.com | http://www.vbulletin.com/license.html # ||
|| #################################################################### ||
\*======================================================================*/

// ######################## SET PHP ENVIRONMENT ###########################
error_reporting(E_ALL & ~E_NOTICE);
if (!is_object($vbulletin->db))
{
	exit;
}

// note hashes are only valid for 5 minutes
$vbulletin->db->query_write("
	DELETE FROM " . TABLE_PREFIX . "pt_issuenotehash
	WHERE dateline < " . (TIMENOW - 300)
);

$mysqlversion = $vbulletin->db->query_first("SELECT version() AS version");
define('MYSQL_VERSION', $mysqlversion['version']);

//searches expire after one hour
if (version_compare(MYSQL_VERSION, '4.1.0', '>='))
{
	$vbulletin->db->query_write("
		DELETE issuesearch, issuesearchresult
		FROM " . TABLE_PREFIX . "pt_issuesearch AS issuesearch
		LEFT JOIN " . TABLE_PREFIX . "pt_issuesearchresult AS issuesearchresult ON (issuesearchresult.issuesearchid = issuesearch.issuesearchid)
		WHERE issuesearch.dateline < " . (TIMENOW - 3600)
	);
}
else
{
	$vbulletin->db->query_write("
		DELETE " . TABLE_PREFIX . "pt_issuesearch, " . TABLE_PREFIX . "pt_issuesearchresult
		FROM " . TABLE_PREFIX . "pt_issuesearch AS issuesearch
		LEFT JOIN " . TABLE_PREFIX . "pt_issuesearchresult AS issuesearchresult ON (issuesearchresult.issuesearchid = issuesearch.issuesearchid)
		WHERE issuesearch.dateline < " . (TIMENOW - 3600)
	);
}

// remove old issue read marking data
$vbulletin->db->query_write("
	DELETE FROM " . TABLE_PREFIX . "pt_issueread
	WHERE readtime < " . (TIMENOW - ($vbulletin->options['markinglimit'] * 86400))
);

// remove old project read marking data
$vbulletin->db->query_write("
	DELETE FROM " . TABLE_PREFIX . "pt_projectread
	WHERE readtime < " . (TIMENOW - ($vbulletin->options['markinglimit'] * 86400))
);

log_cron_action('', $nextitem, 1);

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # RCS: $Revision: 32878 $
|| ####################################################################
\*======================================================================*/
?>