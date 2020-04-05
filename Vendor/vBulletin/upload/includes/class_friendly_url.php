<?php if (!class_exists('vB_Database')) exit;
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

/**#@+
* Friendly URL types
*/
define('FRIENDLY_URL_OFF', 0);
define('FRIENDLY_URL_BASIC',    1);
define('FRIENDLY_URL_ADVANCED', 2);
define('FRIENDLY_URL_REWRITE',  3);

define('SEO_NOSESSION', 1);
define('SEO_JS', 2);
/**#@-*/

/**
* Class for selecting which class to use for doing pretty magic upon URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url
{
	/**
    * public static member variable for tracking the friendly url page information
    *
    * @var    mixed
    */
    public static $friendlies = array(
			'showthread'   => array(
				'primaryvar' => 't',
				'script'     => 'showthread.php',
				'ignorelist' => array('t', 'threadid'),
				'rewritevar' => 'threads',
			),
			'member'       => array(
				'primaryvar' => 'u',
				'script'     => 'member.php',
				'ignorelist' => array('u', 'userid', 'username'),
				'rewritevar' => 'members'
			),
			'forumdisplay' => array(
				'primaryvar' => 'f',
				'script'     => 'forumdisplay.php',
				'ignorelist' => array('f', 'forumid'),
				'rewritevar' => 'forums'
			),
			'blog'         => array(
				'script'     => 'blog.php',
				'primaryvar' => 'u',
				'ignorelist' => array('u', 'userid', 'b', 'blogid'),
				'rewritevar' => 'blogs',
			),
			'entry'      => array(
				'script'     => 'entry.php',
				'primaryvar' => 'b',
				'ignorelist' => array('b', 'blogid'),
				'rewritevar' => 'entries',
			),
			'vbcms'		=> array(
				'script'	=> false,
				'primaryvar'=> 'r',
				'ignorelist'=> array('r'),
				'rewritevar'=> false,
				'regex'		=> '#^(.*)#',
				'parse_post'=> true,
				'raw_scriptpath'	=>	true
			)
    );

    /**
    * public static member variable for tracking the friendly url page information
    *
    * @var    string
    */
    public static $decoded_fragment = '';

    /**
    * public static member variable for tracking which page the friendly url is requesting
    *
    * @var    string
    */
    public static $decoded_pagenumber;

	/**
	* Select library
	*
	* @return	vB_Friendly_Url
	*/
	public static function fetchLibrary(&$registry, $link, $linkinfo, $pageinfo, $primaryid, $primarytitle)
	{
		global $show;

		$linkoptions = explode('|', $link);
		$linktype = $linkoptions[0];

		$urloptions = 0;
		if (in_array('nosession', $linkoptions))
		{
			$urloptions += SEO_NOSESSION;
		}
		if (in_array('js', $linkoptions))
		{
			$urloptions += SEO_JS;
		}

		$selectclass = 'vB_Friendly_Url_' . ucfirst($linktype);
		if (class_exists($selectclass, false))
		{
			$instance = new $selectclass($registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions);
		}
		else
		{
			$instance = new vB_Friendly_Url_Error($linktype);
		}

		return $instance;
	}

	/**
    * Checks if the current request is eligible for friendly urls
    *
    * @return    bool
    */
    public static function is_friendly_eligible()
    {
        // must come from GET request, and must be one of the "friendly" pages
        return (in_array(THIS_SCRIPT, array_keys(self::$friendlies)) AND
        		($_SERVER['REQUEST_METHOD'] == 'GET' OR self::$friendlies[THIS_SCRIPT]['parse_post']));
    }

    /**
    * This function serves a dual purposeReturns a normalized version of the current URL, empty string if page is ineligible
    * also saves the parsed page fragment as a member variable
    *
    * @return    string
    */
    public static function decode_friendly_url()
    {
        // first, verify that requested page is eligible for friendly urls
        // if not, return empty string
        if ( !(self::is_friendly_eligible()) )
        {
        	self::$decoded_fragment = '';
            define('FRIENDLY_URL', FRIENDLY_URL_OFF);
            return '';
        }

    	$scriptpath = (isset(self::$friendlies[THIS_SCRIPT]['raw_scriptpath']) AND self::$friendlies[THIS_SCRIPT]['raw_scriptpath']) ? SCRIPTPATH_RAW : SCRIPTPATH;

    	// auto resolve script
    	if (!self::$friendlies[THIS_SCRIPT]['script'])
    	{
    		self::$friendlies[THIS_SCRIPT]['script'] = basename(SCRIPT);
    	}

    	// auto resolve rewrite var
    	if (!self::$friendlies[THIS_SCRIPT]['rewritevar'])
    	{
    		$pathinfo = pathinfo(SCRIPT);
    		self::$friendlies[THIS_SCRIPT]['rewritevar'] = $pathinfo['filename'];
    	}

        // now, check for slash or query string friendly urls, ie option 1 or 2
        if (
            stripos(SCRIPT, self::$friendlies[THIS_SCRIPT]['script']) !== false
                AND
            ((
                // Slash Method -- showthread.php/1899-Thread-Title/page4?pp=4
                preg_match('#^' . preg_quote(SCRIPT, '#') . '(/)([^\?]*)#', $scriptpath, $firstmatch)
                    AND
                $firstmatch[2]
            )
            OR
            ( 
                // Query string method -- showthread.php?1899-Thread-Title/page4&pp=4
                $method_query = preg_match('#^' . preg_quote(SCRIPT, '#') . '(\?)([^&=]+)(?:&|$)#', $scriptpath, $firstmatch)
                    AND
                $firstmatch[2]
        )))
        {
        	if (isset(self::$friendlies[THIS_SCRIPT]['regex']) AND self::$friendlies[THIS_SCRIPT]['regex'])
        	{
        		if ($method_query AND isset($_GET[self::$friendlies[THIS_SCRIPT]['primaryvar']]))
        		{
        			self::$decoded_fragment = urlencode($_GET[$var = self::$friendlies[THIS_SCRIPT]['primaryvar']]);
        			define('FRIENDLY_URL', FRIENDLY_URL_OFF);
        			return '';
        		}

        		$pat = $pat = self::$friendlies[THIS_SCRIPT]['regex'];
        	}
        	else
        	{
        		$pat = '#^(\d+)(?:[^/]*)?(?:/page(\d+))?#si';
        	}

            // make sure we can get the main variable value (ie thread, member id) from the page fragment
            if ((preg_match($pat, $firstmatch[2], $secondmatch) AND $secondmatch[1]))
            {
                $qs = array();
                $mainvariable = self::$friendlies[THIS_SCRIPT]['primaryvar'];
                $_GET["$mainvariable"] = $_REQUEST["$mainvariable"] = $secondmatch[1];
                $qs[] = "$mainvariable=$secondmatch[1]";
                if (!empty($secondmatch[2]))
                {
                    self::$decoded_pagenumber = $_GET['page'] = $_REQUEST['page'] = $secondmatch[2];
                    $qs[] = "page=$secondmatch[2]";
                }
                if (!empty($_SERVER['QUERY_STRING']))
                {
                    $qs[] = $_SERVER['QUERY_STRING'];
                }

                // set up friendly flag, and return decoded url
                self::$decoded_fragment = $firstmatch[2];
                define('FRIENDLY_URL', $firstmatch[1] == '?' ? FRIENDLY_URL_BASIC : FRIENDLY_URL_ADVANCED);
                return urldecode(SCRIPT . '?' . implode('&', $qs));
            }
            // if we couldnt find a friendly url, we must be in standard mode
            else
            {
            	self::$decoded_fragment = (isset($firstmatch[2]) ? $firstmatch[2] : '');
                define('FRIENDLY_URL', FRIENDLY_URL_OFF);
                return '';
            }
        }

        // check for mod rewrite, ie. we are in showthread.php but our rewrite link doesn't contain showthread.php
        else if (stripos($scriptpath, self::$friendlies[THIS_SCRIPT]['script']) === false AND $_SERVER['QUERY_STRING'])
        {
        	// extract the page fragment from the REDIRECT_URL
        	//  - remove query string from script path (ie everything after "?"), otherwise just $scriptpath
        	$strippedpath = strpos($scriptpath, "?") ? substr($scriptpath, 0, strpos($scriptpath, "?")) : $scriptpath;
        	//  - get the starting position of the fragment
        	$rewritevar_endpos = strripos($strippedpath, self::$friendlies[THIS_SCRIPT]['rewritevar'] . '/') + strlen(self::$friendlies[THIS_SCRIPT]['rewritevar'] . '/');
					//  - get the whole fragment from the stripped path
        	self::$decoded_fragment = substr($strippedpath, $rewritevar_endpos);
        	//  - get the page fragment
        	if (preg_match('#^(\d+)(?:[^/]*)?(?:/page(\d+))?#si', self::$decoded_fragment, $page_fragment) AND isset($page_fragment[2]))
        	{
        		self::$decoded_pagenumber = $_GET['page'] = $_REQUEST['page'] = $page_fragment[2];
        	}

            define('FRIENDLY_URL', FRIENDLY_URL_REWRITE);
            return urldecode(self::$friendlies[THIS_SCRIPT]['script'] . '?' . $_SERVER['QUERY_STRING']);
        }

        // otherwise, friendly URLs must be off, so there is nothing to decode
        else
        {
        	self::$decoded_fragment = '';
            define('FRIENDLY_URL', FRIENDLY_URL_OFF);
            return '';
        }
    }

    /**
    * Returns a cleaned querystring to be used as a pageinfo array for the vB_Friendly_Url classes
    *
    * @return    array
    */
    public static function get_cleaned_querystring()
    {
    	global $vbulletin;

    	$qs = explode('&', $_SERVER['QUERY_STRING']);
		foreach ($qs AS $key => $value)
		{
			if ($value)
			{
				$foo = explode('=', $value);
				// make sure both query string argument is properly formed, and that its not an ignored element for friendlies
				if (isset($foo[0]) AND isset($foo[1]) AND !in_array($foo[0], vB_Friendly_Url::$friendlies[THIS_SCRIPT]['ignorelist']))
				{
					$fkey = $vbulletin->input->xss_clean($foo[0]);
					$pageinfo["$fkey"] = $vbulletin->input->xss_clean($foo[1]);
				}
			}
		}
		// add a pagenumber if one was already decoded from friendly url
		if (!empty(self::$decoded_pagenumber))
		{
			$pageinfo['pagenumber'] = self::$decoded_pagenumber;
		}

		return $pageinfo;
    }

    public static function fetch_friendlies()
    {
    	return vB_Friendly_Url::$friendlies;
    }
}

/**
* Abstract class for doing pretty magic upon URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
abstract class vB_Friendly_Url_Abstract
{
	/**
	* Regex to clean fragments
	*/
	const CLEAN_REGEX = '#[\s;/\\\?:@&=+$,<>\#%"\'\.\r\n\t\x00-\x1f\x7f]#s';

	/**
	* Type of link for current page (ie 'thread', 'member', etc.)
	*
	* @var	string
	*/
	protected $link = null;

	/**
	* Linkinfo Array
	*
	* @var	array
	*/
	protected $linkinfo = null;

	/**
	* Pageinfo Array
	*
	* @var	array
	*/
	protected $pageinfo = null;

	/**
	* vBulletin Registry
	*
	* @var	array
	*/
	protected $registry = null;

	/**
	* URL Options
	*
	* @var int;
	*/
	protected $urloptions = 0;

	/**
    * Ampersand
    *
    * @var string;
    */
    protected  $amp = null;

    /**
    * The id of the current page (ie thread/forum/member id)
    *
    * @var string;
    */
    protected  $id = null;

	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct(&$registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions)
	{
		$this->linkinfo = $linkinfo;
		$this->pageinfo = $pageinfo;
		$this->registry =& $registry;
		$this->urloptions = $urloptions;
		$this->primaryid = $primaryid;
		$this->primarytitle = $primarytitle;
		$this->amp = !($this->urloptions & SEO_JS) ? '&amp;' : '&';
	}

	/**
    * Returns the proper main fragment of an seo url - ie "1899-Thread-Title"
    *
    * @return    string
    */
    protected function get_main_fragment()
    {
			$titlekey = '';
			$idkey = '';

			// populate the fallback keys depending on the link
			switch ($this->link)
			{
				case 'thread':
				{
					$titlekey = 'title';
					$idkey = 'threadid';
					break;
				}
				case 'member':
				{
					$titlekey = 'username';
					$idkey = 'userid';
					break;
				}
				case 'forum':
				{
					$titlekey = 'title';
					$idkey = 'forumid';
					break;
				}
				case 'blog':
				{
					$titlekey = 'title';
					$idkey = 'userid';
					break;
				}
				case 'entry':
				{
					$titlekey = 'title';
					$idkey = 'blogid';
					break;
				}

				// we are not on a friendly page, return empty string
				default:
					$handled = false;
					($hook = vBulletinHook::fetch_hook('friendlyurl_fetch_main_fragment_fallback')) ? eval($hook) : false;
					if (!$handled)
					{
						return '';
					}
			}

			// build and return the main fragment
			$title = $this->primarytitle ? $this->linkinfo["{$this->primarytitle}"] : $this->linkinfo[$titlekey];
			// store this calculated id in case we need it in the final url
			$id = $this->id = $this->primaryid ? $this->linkinfo["{$this->primaryid}"] : $this->linkinfo[$idkey];
			$returnfragment = $id . '-' . self::cleanFragment($title);
			$returnfragment = self::cleanURL($returnfragment);

			return $returnfragment;
    }

    /**
    * Returns the page fragment of an seo url - ie "/page2"
    *
    * @return    string
    */
    protected function get_page_fragment()
    {
        if (isset($this->pageinfo['pagenumber']) AND $this->pageinfo['pagenumber'] > 1)
        {
            return '/page' . $this->pageinfo['pagenumber'];
        }
        else if (isset($this->pageinfo['page']) AND $this->pageinfo['page'] > 1)
        {
            return '/page' . $this->pageinfo['page'];
        }
        else
        {
            return '';
        }
    }

    /**
    * Returns an array with each query string parameter
    *
    * @return    array
    */
    protected function get_qs_arguments()
    {
        $arguments = array();

        // grab any ignored query string parameters for the current page
        $ignorelist = isset(vB_Friendly_Url::$friendlies[THIS_SCRIPT]['ignorelist']) ? vB_Friendly_Url::$friendlies[THIS_SCRIPT]['ignorelist'] : array();

        // add session argument if settings require it
        if (!($this->urloptions & SEO_NOSESSION) AND $this->registry->session->visible)
        {
            $arguments[] = 's=' . $this->registry->session->vars['dbsessionhash'];
        }

        // add any arguments that are not already displayed as part of friendly url
        if (!empty($this->pageinfo))
        {
            foreach ($this->pageinfo AS $var => $value)
            {
                if (
                    in_array($var, $ignorelist)
                        OR
                    (
                        in_array($var, array('page', 'pagenumber'))
                            AND
                        (
                            $this->registry->options['friendlyurl']
                                OR
                            (
                                !$this->registry->options['friendlyurl']
                                    AND
                                $this->pageinfo['page'] <= 1
                            )
                        )
                    )
                )
                {
                    continue;
                }
                $arguments[] = "$var=$value";
            }
        }

        return $arguments;
    }

    /**
    * Checks if current request is the coninical url, and redirects there if not
    *
    * @param	bool	whether or we want this function to perform redirect to canonical
    *
    * @return	bool	true if
    */
    public function verify_canonical_url($do_redirect = false)
    {
        // if friendly urls are turned off (STANDARD), or the page is not eligible, dont worry about canonical urls
        if ( ($this->registry->options['friendlyurl'] == FRIENDLY_URL_OFF) OR !(vB_Friendly_Url::is_friendly_eligible()) )
        {
            return true;
        }

        // make sure current URL has been decoded, and friendly settings detected
        if (!defined('FRIENDLY_URL'))
        {
        	vB_Friendly_Url::decode_current_url();
        }

        // performing actual canoncial check...
        // - check if detected settings match whats actually set by admin
        if (
        	$this->registry->options['friendlyurl'] != FRIENDLY_URL
        		OR
        	$this->get_main_fragment() . $this->get_page_fragment() != vB_Friendly_Url::$decoded_fragment
        	)
        {
        	if (!$do_redirect)
        	{
        		return false;
        	}
/*
        	echo $this->get_main_fragment() . $this->get_page_fragment() . '<hr />';
        	echo vB_Friendly_Url::$decoded_fragment;
        	echo '<hr />';
        	echo $this->output();
        	exit;
*/
        	$url = $this->output();
        	
        	if (defined('THREADNEXT'))
        	{
        		$url = str_replace('&goto=nextnewest', '', str_replace('&goto=nextoldest', '', $url)); 
        	}
        	
	        // - if not canonical and doing redirect, then redirect to canonical url
	        // Note: use 302 if we are on showthread and we came in via *&goto=next*
	       	exec_header_redirect($url, defined('THREADNEXT') ? 302 : 301);
        }
        else
        {
        	// - if page fragments and detected settings all line up, we are at the canonical URL
        	return true;
        }
    }

	/**
	* Output final product
	*
	* @return	string
	*/
	abstract public function output();

	/**
	 * Cleans input to be parsed into the url.
	 * Note: Input is expected to be passed htmlspecialchar'd.
	 *
	 * @param string $fragment
	 * @return string
	 */
	protected static function cleanFragment($fragment)
	{
		$fragment = unhtmlspecialchars($fragment);
		return preg_replace(self::CLEAN_REGEX, '-', strip_tags($fragment));
	}

	/**
	 * Cleans a compiled fragment for rendering.
	 *
	 * @param string $fragment
	 * @return string
	 */
	protected static function cleanURL($fragment)
	{
		$fragment = to_ascii($fragment);
		return trim(urlencode(preg_replace('#-+#', '-', $fragment)), '-');
	}
}

/**
* Class for throwing an error when we can't work pretty magic upon the supplied link type
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url_Error
{
	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct($link)
	{
		$this->link = $link;
	}

	/**
	* Generic error
	*
	* @return	string
	*/
	public function output()
	{
		// todo: Phrase me
		return '~~Invalid Link Type~~ : ' . htmlspecialchars_uni($this->link);
	}
}

/**
* Class for doing pretty magic upon showthread URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url_Thread extends vB_Friendly_Url_Abstract
{
	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct(&$registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions)
	{
		$this->link = 'thread';
		parent::__construct($registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions);
	}

	/**
	* Output final product
	*
	* @return	string
	*/
	public function output()
	{
		// Option 0
		// showthread.php?t=1234&p=2
		//
		// Option 1
		// showthread.php?1234-Thread-Title/page2&pp=2
		//
		// Option 2 (apache pathinfo)
		// showthread.php/1234-Thread-Title/page2?pp=2
		//
		// Option 3 (mod rewrite)
		// /threads/1234-Thread-Title/page2?pp=2
		// RewriteRule ^/vb4/threads/([0-9]+)(?:/?$|(?:-[^/]+))(?:/?$|(?:/page([0-9]+)?)) /vb4/showthread.php?t=$1&page=$2&%{QUERY_STRING}

		// populate the SEO url fragments
		$threadfragment = $this->get_main_fragment();
		$pagefragment = $this->get_page_fragment();
		$arguments = $this->get_qs_arguments();

		$handled = false;
		($hook = vBulletinHook::fetch_hook('friendlyurl_output_thread')) ? eval($hook) : false;

		if (!$handled)
		{
			switch ($this->registry->options['friendlyurl'])
			{
				case 1:
					return 'showthread.php?' . $threadfragment . $pagefragment . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');

				case 2:
					return 'showthread.php/' . $threadfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 3:
					return 'threads/' . $threadfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 0:
				default:
					return 'showthread.php?' . $this->registry->session->vars['sessionurl'] . 't=' . $this->id . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');
			}
		}
	}
}

/**
* Class for doing pretty magic upon member URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url_Member extends vB_Friendly_Url_Abstract
{
	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct(&$registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions)
	{
		$this->link = 'member';
		parent::__construct($registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions);
	}

	/**
    * Returns the page fragment of an seo url - ie "/page2"
    * Overides parent definition make sure page fragment is nothing for members
    *
    * @return    empty string
    */
    protected function get_page_fragment()
    {
    	return '';
    }

	/**
    * Returns an array with each query string parameter
    * Overides parent definition to prefent 'page' parameters from being ignored
    *
    * @return    array
    */
    protected function get_qs_arguments()
    {
        $arguments = array();

        // add session argument if settings require it
        if (!($this->urloptions & SEO_NOSESSION) AND $this->registry->session->visible)
        {
            $arguments[] = 's=' . $this->registry->session->vars['dbsessionhash'];
        }

        $ignorelist = isset(vB_Friendly_Url::$friendlies[THIS_SCRIPT]['ignorelist']) ? vB_Friendly_Url::$friendlies[THIS_SCRIPT]['ignorelist'] : array();

        // add any arguments that are not already displayed as part of friendly url
        if (!empty($this->pageinfo) && is_array($this->pageinfo))
        {
            foreach ($this->pageinfo AS $var => $value)
            {
                if (in_array($var, $ignorelist))
                {
                    continue;
                }
                $arguments[] = "$var=$value";
            }
        }

        return $arguments;
    }

	/**
	* Output final product
	*
	* @return	string
	*/
	public function output()
	{
		// Option 0
		// member.php?u=1234
		//
		// Option 1
		// member.php?1234-Username&x=2
		//
		// Option 2 (apache pathinfo)
		// member.php/1234-Username?x=2
		//
		// Option 3 (mod rewrite)
		// /members/1234-Username?pp=2
		// RewriteRule ^/vb4/members/([0-9]+) /vb4/member.php?u=$1%&{QUERY_STRING}


		// populate the SEO url fragments
		$namefragment = $this->get_main_fragment();
    $arguments = $this->get_qs_arguments();

		$handled = false;
		($hook = vBulletinHook::fetch_hook('friendlyurl_output_member')) ? eval($hook) : false;

		if (!$handled)
		{
			switch ($this->registry->options['friendlyurl'])
			{
				case 1:
					return 'member.php?' . $namefragment . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');

				case 2:
					return 'member.php/' . $namefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 3:
					return 'members/' . $namefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 0:
				default:
					return 'member.php?' . $this->registry->session->vars['sessionurl'] . "u=$this->id" . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');
			}
		}
	}
}

/**
* Class for doing pretty magic upon forumdisplay URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url_Forum extends vB_Friendly_Url_Abstract
{
	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct(&$registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions)
	{
		$this->link = 'forum';
		parent::__construct($registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions);
	}

	/**
	* Output final product
	*
	* @return	string
	*/
	public function output()
	{
		// Option 0
		// forumdisplay.php?f=1234
		//
		// Option 1
		// forumdisplay.php?1234-Title&x=2
		//
		// Option 2 (apache pathinfo)
		// forumdisplay.php/1234-Title?x=2
		//
		// Option 3 (mod rewrite)
		// /forums/1234-Title?pp=2
		// RewriteRule ^/vb4/forums/([0-9]+)(?:/?$|(?:-[^/]+))(?:/?$|(?:/page([0-9]+)?)) /vb4/forumdisplay.php?f=$1&page=$2&%{QUERY_STRING}

		// populate the SEO url fragments
		$forumfragment = $this->get_main_fragment();
   	$pagefragment = $this->get_page_fragment();
    $arguments = $this->get_qs_arguments();

		$handled = false;
		($hook = vBulletinHook::fetch_hook('friendlyurl_output_forum')) ? eval($hook) : false;

		if (!$handled)
		{
			switch ($this->registry->options['friendlyurl'])
			{
				case 1:
					return 'forumdisplay.php?' . $forumfragment . $pagefragment . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');

				case 2:
					return 'forumdisplay.php/' . $forumfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 3:
					return 'forums/' . $forumfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 0:
				default:
					return 'forumdisplay.php?' . $this->registry->session->vars['sessionurl'] . "f=$this->id" . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');
			}
		}
	}
}

/**
* Class for doing pretty magic upon blog URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url_Blog extends vB_Friendly_Url_Abstract
{
	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct(&$registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions)
	{
		$this->link = 'blog';
		parent::__construct($registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions);
	}

	/**
	* Output final product
	*
	* @return	string
	*/
	public function output()
	{
		// Option 0
		// blog.php?u=1234&p=2
		//
		// Option 1
		// blog.php?1234-Blog-Title/page2&pp=2
		//
		// Option 2 (apache pathinfo)
		// blog.php/1234-Blog-Title/page2?pp=2
		//
		// Option 3 (mod rewrite)
		// /blogs/1234-Blog-Title/page2?pp=2
		// RewriteRule ^/vb4/blogs/([0-9]+)(?:/?$|(?:-[^/]+))(?:/?$|(?:/page([0-9]+)?)) /vb4/blog.php?u=$1&page=$2&%{QUERY_STRING}

		// populate the SEO url fragments
		$blogfragment = $this->get_main_fragment();
		$pagefragment = $this->get_page_fragment();
		$arguments = $this->get_qs_arguments();

		$handled = false;
		($hook = vBulletinHook::fetch_hook('friendlyurl_output_blog')) ? eval($hook) : false;

		if (!$handled)
		{
			switch ($this->registry->options['friendlyurl'])
			{
				case 1:
					return 'blog.php?' . $blogfragment . $pagefragment . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');

				case 2:
					return 'blog.php/' . $blogfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 3:
					return 'blogs/' . $blogfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 0:
				default:
					return 'blog.php?' . $this->registry->session->vars['sessionurl'] . 'u=' . $this->id . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');
			}
		}
	}
}

/**
* Class for doing pretty magic upon blog URLs to make them human readable
*
* @package 		vBulletin
* @version		$Revision: 27657 $
* @date 		$Date: 2008-09-03 08:36:05 -0700 (Wed, 03 Sep 2008) $
*
*/
class vB_Friendly_Url_Entry extends vB_Friendly_Url_Abstract
{
	/**
	* Constructor
	*
	* @return	void
	*/
	public function __construct(&$registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions)
	{
		$this->link = 'entry';
		parent::__construct($registry, $linkinfo, $pageinfo, $primaryid, $primarytitle, $urloptions);
	}

	/**
	* Output final product
	*
	* @return	string
	*/
	public function output()
	{
		// Option 0
		// entry.php?b=1234&p=2
		//
		// Option 1
		// entry.php?1234-Entry-Title/page2&pp=2
		//
		// Option 2 (apache pathinfo)
		// entry.php/1234-Blog-Title/page2?pp=2
		//
		// Option 3 (mod rewrite)
		// /entries/1234-Blog-Title/page2?pp=2
		// RewriteRule ^/vb4/blogs/([0-9]+)(?:/?$|(?:-[^/]+))(?:/?$|(?:/page([0-9]+)?)) /vb4/blog.php?b=$1&page=$2&%{QUERY_STRING}

		// populate the SEO url fragments
		$entryfragment = $this->get_main_fragment();
		$pagefragment = $this->get_page_fragment();
		$arguments = $this->get_qs_arguments();

		$handled = false;
		($hook = vBulletinHook::fetch_hook('friendlyurl_output_entry')) ? eval($hook) : false;

		if (!$handled)
		{
			switch ($this->registry->options['friendlyurl'])
			{
				case 1:
					return 'entry.php?' . $entryfragment . $pagefragment . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');

				case 2:
					return 'entry.php/' . $entryfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 3:
					return 'entries/' . $entryfragment . $pagefragment . (!empty($arguments) ? '?' . implode($this->amp, $arguments) : '');

				case 0:
				default:
					return 'entry.php?' . $this->registry->session->vars['sessionurl'] . 'b=' . $this->id . (!empty($arguments) ? $this->amp . implode($this->amp, $arguments) : '');
			}
		}
	}
}

/*======================================================================*\
|| ####################################################################
|| # Downloaded: 12:24, Tue Dec 1st 2009
|| # CVS: $RCSfile$ - $Revision: 27657 $
|| ####################################################################
\*======================================================================*/