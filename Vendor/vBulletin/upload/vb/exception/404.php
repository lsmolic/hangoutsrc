<?php if (!defined('VB_ENTRY')) die('Access denied.');
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

/**
 * 404 Exception
 * Exception to throw to redirect to a 404 page.
 *
 * A redirect message can be assigned to display to the user.  The message should
 * be phrased as it could be displayed to the user.
 *
 * @todo Set the reroute path from vB_Router::$App->get404Route();
 *
 * @package vBulletin
 * @author vBulletin Development Team
 * @version $Revision: 32878 $
 * @since $Date: 2009-10-28 13:38:49 -0500 (Wed, 28 Oct 2009) $
 * @copyright vBulletin Solutions Inc.
 */
class vB_Exception_404 extends vB_Exception_Reroute
{
	/*Initialisation================================================================*/

	/**
	 * Creates a 404 exception with the given message
	 *
	 * @param string $message					- A user friendly error
	 * @param int $code							- The PHP code of the error
	 * @param string $file						- The file the exception was thrown from
	 * @param int $line							- The line the exception was thrown from
	 */
	public function __construct($message = false, $code = false, $file = false, $line = false)
	{
		$message = $message ? $message : new vB_Phrase('error', 'page_not_found');

		// Standard exception initialisation
		parent::__construct(vB_Router::get404Path(), $message, $code, $file, $line);
	}
}

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # SVN: $Revision: 32878 $
|| ####################################################################
\*======================================================================*/
