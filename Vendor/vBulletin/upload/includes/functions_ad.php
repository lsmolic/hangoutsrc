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

function build_ad_template($location)
{
	global $ad_cache, $vbulletin;
	$template = "";
	foreach($ad_cache AS $adid => $ad)
	{
		// active ads on the same location only
		if ($ad['active'] AND $ad['adlocation'] == $location)
		{
			$criterion = $vbulletin->db->query_read("
				SELECT * FROM " . TABLE_PREFIX . "adcriteria
				WHERE adid = " . $adid . "
			");

			// create the template conditionals
			$conditional_prefix = "";
			$conditional_postfix = "";

			while($criteria = $vbulletin->db->fetch_array($criterion))
			{
				switch($criteria['criteriaid'])
				{
					case "in_usergroup_x":
						$conditional_prefix .= '<if condition="is_member_of($' . 'bbuserinfo, ' . $criteria['condition1'] . ')">';
						$conditional_postfix .= "</if>";
						break;
					case "not_in_usergroup_x":
						$conditional_prefix .= '<if condition="!is_member_of($' . 'bbuserinfo, ' . $criteria['condition1'] . ')">';
						$conditional_postfix .= "</if>";
						break;
					case "browsing_forum_x":
						$conditional_prefix .= '<if condition="$' . 'forumid == ' . $criteria['condition1'] . '">';
						$conditional_postfix .= "</if>";
						break;
					case "browsing_forum_x_and_children":
						// find out who the children are:
						$forum = $db->query_first("SELECT childlist FROM " . TABLE_PREFIX . "forum WHERE forumid = " . intval($criteria['condition1']));
						$conditional_prefix .= '<if condition="in_array($' . 'forumid, array(' . $forum['childlist'] . '))">';
						$conditional_postfix .= "</if>";
						break;
					case "style_is_x":
						$conditional_prefix .= '<if condition="STYLEID == ' . intval($criteria['condition1']) . '">';
						$conditional_postfix .= "</if>";
						break;
					case "no_visit_in_x_days":
						$conditional_prefix .= '<if condition="$' . 'bbuserinfo[\'lastactivity\'] < TIMENOW - (86400*' . intval($criteria['condition1']) . ')">';
						$conditional_postfix .= "</if>";
						break;
					case "no_posts_in_x_days":
						$conditional_prefix .= '<if condition="$' . 'bbuserinfo[\'lastpost\'] < TIMENOW - (86400*' . intval($criteria['condition1']) . ')">';
						$conditional_postfix .= "</if>";
						break;
					case "has_x_postcount":
						$conditional_prefix .= '<if condition="$' . 'bbuserinfo[\'posts\'] > ' . intval($criteria['condition1']) . ' AND $' . 'bbuserinfo[\'posts\'] < ' . intval($criteria['condition2']) . '">';
						$conditional_postfix .= "</if>";
						break;
					case "has_never_posted":
						$conditional_prefix .= '<if condition="$' . 'bbuserinfo[\'lastpost\'] == 0">';
						$conditional_postfix .= "</if>";
						break;
					case "has_x_reputation":
						$conditional_prefix .= '<if condition="$' . 'bbuserinfo[\'reputation\'] > ' . intval($criteria['condition1']) . ' AND $' . 'bbuserinfo[\'reputation\'] < ' . intval($criteria['condition2']) . '">';
						$conditional_postfix .= "</if>";
						break;
					case "pm_storage_x_percent_full":
						$conditional_prefix .= '<if condition="$' . 'pmboxpercentage = $' . 'bbuserinfo[\'pmtotal\'] / $' . 'bbuserinfo[\'permissions\'][\'pmquota\'] * 100"></if>';
						$conditional_prefix .= '<if condition="$' . 'pmboxpercentage > ' . intval($criteria['condition1']) . ' AND $' . 'pmboxpercentage < ' . intval($criteria['condition2']) . '">';
						$conditional_postfix .= "</if>";
						break;
					case "came_from_search_engine":
						$conditional_prefix .= '<if condition="is_came_from_search_engine()">';
						$conditional_postfix .= "</if>";
						break;
					case "is_date":
						if ($criteria['condition2'])
						{
							$conditional_prefix .= '<if condition="gmdate(\'d-m-Y\', TIMENOW) == ' . $criteria['condition1'] .'">';
							$conditional_postfix .= "</if>";
						}
						else
						{
							$conditional_prefix .= '<if condition="vbdate(\'d-m-Y\', TIMENOW, false, false) == ' . $criteria['condition1'] . '">';
							$conditional_postfix .= "</if>";
						}
						break;
					case "is_time":
						if (preg_match('#^(\d{1,2}):(\d{2})$#', $criteria[1], $start_time) AND preg_match('#^(\d{1,2}):(\d{2})$#', $criteria[2], $end_time))
						{
							if ($criteria['condition3'])
							{
								$start = gmmktime($start_time[1], $start_time[2]);
								$end   = gmmktime($end_time[1], $end_time[2]);
								// $now   = gmmktime();
								$conditional_prefix .= '<if condition="$' . 'now = gmmktime()"></if>';
							}
							else
							{
								$start = mktime($start_time[1], $start_time[2]) + $vbulletin->options['hourdiff'];
								$end   = mktime($end_time[1], $end_time[2]) + $vbulletin->options['hourdiff'];
								// $now   = mktime() + $vbulletin->options['hourdiff'];
								$conditional_prefix .= '<if condition="$' . 'now = mktime() + ' . $vbulletin->options['hourdiff'] . '"></if>';
							}
							$conditional_prefix .= '<if condition="$' . 'now > ' . $start . ' OR $' . 'now < ' . $end . '">';
							$conditional_postfix .= '</if>';
						}
						break;
					case "ad_x_not_displayed":
						// no ad shown? make note of it, and create the array for us
						$conditional_prefix .= '<if condition="$noadshown = !isset($' . 'adsshown)"></if>';
						$conditional_prefix .= '<if condition="$noadshown"><if condition="$' . 'adsshown = array()"></if></if>';
						// if no ads shown, OR ad x have not been shown, show the ad
						$conditional_prefix .= '<if condition="$noadshown OR !in_array($adid, $' . 'adsshown)">';
						$conditional_postfix .= '</if>';
						break;
					default:
						($hook = vBulletinHook::fetch_hook('ad_check_criteria')) ? eval($hook) : false;
						break;
				}
			}
			// add a faux conditional before all the closing conditions to mark that we've shown certain ad already
			$conditional_postfix = '<if condition="$' . 'adsshown[] = ' . $adid . '"></if>' . $conditional_postfix;

			// wrap the conditionals around their ad snippet / template
			$template .= $conditional_prefix . $ad['snippet'] . $conditional_postfix;
		}
	}

	return $template;
}

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # CVS: $RCSfile$ - $Revision: 32878 $
|| ####################################################################
\*======================================================================*/
?>