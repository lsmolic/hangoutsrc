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

// ###################### Start print_statistic_result #######################
function print_statistic_result($date, $bar, $value, $width)
{
	global $vbulletin;
	$bgclass = fetch_row_bgclass();

	if (preg_match('#^(https?://|/)#i', vB_Template_Runtime::fetchStyleVar('imgdir_poll')))
	{
		$imgpath = vB_Template_Runtime::fetchStyleVar('imgdir_poll');
	}
	else
	{
		$imgpath = '../' . vB_Template_Runtime::fetchStyleVar('imgdir_poll');
	}

	if ($vbulletin->userinfo['lang_options'] & $vbulletin->bf_misc_languageoptions['direction'])
	{
		// ltr
		$l_img = 'l';
		$r_img = 'r';
	}
	else
	{
		// rtl
		$l_img = 'r';
		$r_img = 'l';
	}

	echo '<tr><td width="0" class="' . $bgclass . '">' . $date . "</td>\n";
	echo '<td width="100%" class="' . $bgclass . '" nowrap="nowrap"><img src="' . $imgpath . '/bar' . "$bar-$l_img" . '.gif" height="10" /><img src="' . $imgpath . '/bar' . $bar . '.gif" width="' . $width . '%" height="10" /><img src="' . $imgpath . "/bar$bar-$r_img.gif\" height=\"10\" /></td>\n";
	echo '<td width="0%" class="' . $bgclass . '" nowrap="nowrap">' . $value . "</td></tr>\n";
}

// ###################### Start print_statistic_code #######################
function print_statistic_code($title, $name, $start, $end, $nullvalue = true, $scope = 'daily', $sort = 'date_desc')
{

	global $vbphrase;

	print_form_header('stats', $name);
	print_table_header($title);

	print_time_row($vbphrase['start_date'], 'start', $start, false);
	print_time_row($vbphrase['end_date'], 'end', $end, false);

	if ($name != 'activity')
	{
		print_select_row($vbphrase['scope'], 'scope', array('daily' => $vbphrase['daily'], 'weekly' => $vbphrase['weekly'], 'monthly' => $vbphrase['monthly']), $scope);
	}
	else
	{
		construct_hidden_code('scope', 'daily');
	}
	print_select_row($vbphrase['order_by'], 'sort', array(
		'date_asc'   => $vbphrase['date_ascending'],
		'date_desc'  => $vbphrase['date_descending'],
		'total_asc'  => $vbphrase['total_ascending'],
		'total_desc' => $vbphrase['total_descending'],
	), $sort);
	print_yes_no_row($vbphrase['include_empty_results'], 'nullvalue', $nullvalue);
	print_submit_row($vbphrase['go']);
}

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # CVS: $RCSfile$ - $Revision: 32878 $
|| ####################################################################
\*======================================================================*/
?>