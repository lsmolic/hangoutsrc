<?php
/*======================================================================*\
|| #################################################################### ||
|| # vBulletin Blog 4.0.0 Beta 4 - Licence Number VBFB74B3F1
|| # ---------------------------------------------------------------- # ||
|| # Copyright ©2000-2009 vBulletin Solutions Inc. All Rights Reserved. ||
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

$vbulletin->db->query_write("
	DELETE FROM " . TABLE_PREFIX . "cms_node 
	WHERE new = 1 AND lastupdated < " . (TIMENOW - 3600)
);

log_cron_action('', $nextitem, 1);

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # CVS: $Revision: 25612 $
|| ####################################################################
\*======================================================================*/