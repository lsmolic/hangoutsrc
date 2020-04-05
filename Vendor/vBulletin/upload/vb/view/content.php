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
 * Default Content View
 * Provides default functionality for common content fields.
 *
 * @package vBulletin
 * @author vBulletin Development Team
 * @version $Revision: 28694 $
 * @since $Date: 2008-12-04 16:12:22 +0000 (Thu, 04 Dec 2008) $
 * @copyright vBulletin Solutions Inc.
 */
class vB_View_Content extends vB_View
{
	/*Render========================================================================*/

	/**
	 * Prepares properties for rendering.
	 */
	protected function prepareProperties()
	{
		$this->title = htmlspecialchars_uni($this->title);
		$this->description = htmlspecialchars_uni($this->description);
		$this->contenttype = vB_Types::instance()->getContentTypeTitle(array('package' => $this->package, 'class' => $this->class));
	}
}

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # SVN: $Revision: 28694 $
|| ####################################################################
\*======================================================================*/