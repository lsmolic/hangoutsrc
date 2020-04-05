/**  --------------------------------------------------------  *
 *   GuiBuilder.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/30/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;

using Hangout.Client;
using Hangout.Client.Gui;
using Hangout.Shared;

public class GuiBuilder : AppEntry
{
	private readonly static string GUI_PATH = "resources://GUI/Tools/GUI Builder/GUI Builder.gui";
	private readonly static string GUI_CACHE_DIR = Application.dataPath + "/../GuiBuilder/";
	private readonly static string GUI_CACHE_USER_DATA_FILE = GUI_CACHE_DIR + "UserData";

	private readonly static string USER_DATA_ROOT_XML_NODE_NAME = "UserData";
	private readonly static string LAST_OPEN_GUI_XML_NODE_NAME = "LastOpenedGui";

	private RuntimeGuiManager mManager = new RuntimeGuiManager(new Logger(new DebugLogReporter()));

	private GuiController mGuiBuilder = null;
	private ITopLevel mToolbar = null;
	private Label mLoadedGuiLabel = null;

	private ITopLevel mLoadGuiDialog = null;
	private Textbox mLoadGuiFilterText = null;

	private ITopLevel mFileErrorWindow = null;
	private ITopLevel mOpenWindowsList = null;

	private readonly List<ITopLevel> mLoadedGuis = new List<ITopLevel>();

	private void OnGUI()
	{
		mManager.Draw();
	}

	private void LoadGuis()
	{
		mGuiBuilder = new GuiController(mManager, GUI_PATH);
		foreach (ITopLevel topLevel in mGuiBuilder.AllGuis)
		{
			switch (topLevel.Name)
			{
				case "LoadGuiDialog":
					SetupLoadGuiDialog(topLevel);
					break;

				case "ToolbarWindow":
					SetupToolbar(topLevel);
					break;

				case "GuiFileErrorWindow":
					SetupFileErrorWindow(topLevel);
					break;

				case "OpenWindowsList":
					SetupOpenWindowsList(topLevel);
					break;
			}
		}
	}

	private bool mOpenWindowsListShowing = true;
	private Button mWindowListingPrototype = null;
	private IGuiFrame mWindowListFrame = null;
	private void SetupOpenWindowsList(ITopLevel topLevel)
	{
		mOpenWindowsList = topLevel; 
		((Window)mOpenWindowsList).InFront = true;
		mWindowListFrame = mOpenWindowsList.SelectSingleElement<IGuiFrame>("MainFrame/WindowList");
		Button hideShowButton = mOpenWindowsList.SelectSingleElement<Button>("MainFrame/HideShowButton");
		hideShowButton.AddOnPressedAction(delegate()
		{
			Vector2 currentPosition = mManager.GetTopLevelPosition(mOpenWindowsList).GetPosition(mOpenWindowsList);
			float shiftAmount = mWindowListFrame.ExternalSize.x - (hideShowButton.ExternalSize.x * 0.5f);
			if (!mOpenWindowsListShowing)
			{
				shiftAmount *= -1.0f;
			}

			mManager.SetTopLevelPosition
			(
				mOpenWindowsList,
				new FixedPosition
				(
					currentPosition - new Vector2(shiftAmount, 0.0f)
				)
			);

			mOpenWindowsListShowing = !mOpenWindowsListShowing;
		});

		mWindowListingPrototype = mWindowListFrame.SelectSingleElement<Button>("WindowListingPrototype");
		mWindowListFrame.RemoveChildWidget(mWindowListingPrototype);
	}

	private void SetupLoadedGuiWindowList()
	{
		mWindowListFrame.ClearChildWidgets();
		float nextButtonPosition = 0.0f;
		foreach(ITopLevel topLevel in mLoadedGuis)
		{
			Button windowListing = (Button)mWindowListingPrototype.Clone();
			windowListing.Text = topLevel.Name;
			mWindowListFrame.AddChildWidget(windowListing, new FixedPosition(4.0f, nextButtonPosition));
			nextButtonPosition += windowListing.ExternalSize.y + 2.0f;

			ITopLevel closureTopLevel = topLevel;
			windowListing.AddOnPressedAction(delegate()
			{
				closureTopLevel.Showing = !closureTopLevel.Showing;
				if( !closureTopLevel.Showing && !mHiddenWindows.Contains(closureTopLevel.Name) )
				{
					mHiddenWindows.Add(closureTopLevel.Name);
				}
				else if( closureTopLevel.Showing && mHiddenWindows.Contains(closureTopLevel.Name) )
				{
					mHiddenWindows.Remove(closureTopLevel.Name);
				}

			});
		}
	}

	private void SetupFileErrorWindow(ITopLevel topLevel)
	{
		mFileErrorWindow = topLevel;
		((Window)mFileErrorWindow).InFront = true;
		mFileErrorWindow.Showing = false;

		Button reloadGuiButton = mFileErrorWindow.SelectSingleElement<Button>("MainFrame/ReloadGuiButton");
		reloadGuiButton.AddOnPressedAction(delegate()
		{
			mFileErrorWindow.Showing = false;
			LoadUserGui(GetLastGuiPath());
		});
	}

	private void ShowError(string errorMessage)
	{
		mFileErrorWindow.Showing = true;
		ITextGuiElement errorText = mFileErrorWindow.SelectSingleElement<ITextGuiElement>("MainFrame/ErrorFrame/ErrorMessage");
		errorText.Text = errorMessage;
	}

	private void SetupToolbar(ITopLevel topLevel)
	{
		mToolbar = topLevel;
		((Window)mToolbar).InFront = true;
		mLoadedGuiLabel = mToolbar.SelectSingleElement<Label>("MainFrame/LoadedFileLabel");

		Button openGuiButton = mToolbar.SelectSingleElement<Button>("MainFrame/OpenButton");
		openGuiButton.AddOnPressedAction(ShowLoadGuiDialog);
	}

	private Button mLoadGuiButtonPrototype = null;
	private IGuiFrame mGuiListFrame = null;
	private readonly List<FileInfo> mAllGuisOnDisk = new List<FileInfo>();
	private readonly List<string> mHiddenWindows = new List<string>();

	private void SetupLoadGuiDialog(ITopLevel topLevel)
	{
		mLoadGuiDialog = topLevel;
		((Window)mLoadGuiDialog).InFront = true;
		mLoadGuiDialog.Showing = false;
		mLoadGuiFilterText = mLoadGuiDialog.SelectSingleElement<Textbox>("MainFrame/FilterText");

		mGuiListFrame = mLoadGuiDialog.SelectSingleElement<IGuiFrame>("MainFrame/GuiList");
		mLoadGuiButtonPrototype = mGuiListFrame.SelectSingleElement<Button>("OpenGuiPrototype");
		mGuiListFrame.RemoveChildWidget(mLoadGuiButtonPrototype);

		// Keep the load file button disabled until it points to a valid file
		mLoadGuiFilterText.AddTextChangedCallback(LayoutGuiListings);

		LayoutGuiListings();
	}

	private void LayoutGuiListings()
	{
		float currentPosition = 0.0f;
		List<Regex> filters = new List<Regex>();
		foreach (string filterWord in mLoadGuiFilterText.Text.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries))
		{
			filters.Add(new Regex(filterWord, RegexOptions.IgnoreCase | RegexOptions.Compiled));
		}
		mGuiListFrame.ClearChildWidgets();
		foreach(FileInfo guiFileInfo in mAllGuisOnDisk)
		{
			bool match = true;
			foreach (Regex filter in filters)
			{
				if( !filter.IsMatch(guiFileInfo.Name) )
				{
					match = false;
					break;
				}
			}

			if( match )
			{
				Button guiListingButton = (Button)mLoadGuiButtonPrototype.Clone();
				guiListingButton.Text = guiFileInfo.Name;
				mGuiListFrame.AddChildWidget(guiListingButton, new FixedPosition(4.0f, currentPosition));
				currentPosition += guiListingButton.ExternalSize.y;

				string path = guiFileInfo.FullName;
				guiListingButton.AddOnPressedAction(delegate()
				{
					ClearUserGuis();
					LoadUserGui(path);
					SetLastLoadedGui(path);
					mHiddenWindows.Clear();
					mLoadGuiDialog.Showing = false;
				});
			}
		}
	}

	private static bool IsValidGuiPath(string path)
	{
		bool valid = true;
		try
		{
			// Make sure this is the only line in this Try statement, we're catching anything it throws
			XmlUtility.LoadXmlDocument(path);
		}
		catch (Exception)
		{
			valid = false;
		}
		return valid;
	}

	protected override void Awake()
	{
		base.Awake();
		GameFacade.Instance.RegisterMediator(new SchedulerMediator(this.Scheduler));
		Camera.mainCamera.backgroundColor = new Color(0.33f, 0.33f, 0.33f);

		LoadGuis();

		if (!Directory.Exists(GUI_CACHE_DIR))
		{
			Directory.CreateDirectory(GUI_CACHE_DIR);
		}

		string lastOpenedGui = GetLastGuiPath();

		if (lastOpenedGui != null && IsValidGuiPath(lastOpenedGui))
		{
			LoadUserGui(lastOpenedGui);
		}
		else
		{
			ShowLoadGuiDialog();
		}
	}

	private void ShowLoadGuiDialog()
	{
		mLoadGuiDialog.Showing = true;
		mAllGuisOnDisk.Clear();
		mAllGuisOnDisk.AddRange
		(
			Functionals.Map<string, FileInfo>
			(
				delegate(string path)
				{
					return new FileInfo(path);
				},
				Directory.GetFiles(Application.dataPath + "/Resources", "*.gui.xml", SearchOption.AllDirectories)
			)
		);

		mAllGuisOnDisk.Sort(delegate(FileInfo a, FileInfo b)
		{
			return a.Name.CompareTo(b.Name);
		});

		LayoutGuiListings();
	}

	private string GetLastGuiPath()
	{
		string result = null;
		try
		{
			XmlDocument userDataXml = new XmlDocument();
			if (File.Exists(GUI_CACHE_USER_DATA_FILE))
			{
				userDataXml.Load(GUI_CACHE_USER_DATA_FILE);
			}

			XmlNode lastOpenedGuiNode = userDataXml.SelectSingleNode
			(
				USER_DATA_ROOT_XML_NODE_NAME + "/" + LAST_OPEN_GUI_XML_NODE_NAME
			);
			if (lastOpenedGuiNode != null && IsValidGuiPath(lastOpenedGuiNode.InnerText))
			{
				result = lastOpenedGuiNode.InnerText;
			}
		}
		catch (XmlException)
		{
			// if the file's corrupted, just reset the cache
			File.Delete(GUI_CACHE_USER_DATA_FILE);
		}
		return result;
	}

	private void SetLastLoadedGui(string path)
	{
		XmlDocument userDataXml = new XmlDocument();
		XmlElement lastOpenGuiNode = null;
		if (!File.Exists(GUI_CACHE_USER_DATA_FILE))
		{
			XmlElement root = userDataXml.CreateElement(USER_DATA_ROOT_XML_NODE_NAME);
			userDataXml.AppendChild(root);

			lastOpenGuiNode = userDataXml.CreateElement(LAST_OPEN_GUI_XML_NODE_NAME);
			root.AppendChild(lastOpenGuiNode);
		}
		else
		{
			userDataXml.Load(GUI_CACHE_USER_DATA_FILE);
			lastOpenGuiNode = (XmlElement)userDataXml.SelectSingleNode
			(
				USER_DATA_ROOT_XML_NODE_NAME + "/" + LAST_OPEN_GUI_XML_NODE_NAME
			);
		}

		lastOpenGuiNode.InnerText = path;

		userDataXml.Save(GUI_CACHE_USER_DATA_FILE);
	}


	private readonly List<FileWatcher> mCurrentGuiMonitors = new List<FileWatcher>();
	private string mCurrentGuiPath = null;
	private void ReloadCurrentGui()
	{
		LoadUserGui(mCurrentGuiPath);
	}

	private void LoadUserGui(string guiPath)
	{
		mCurrentGuiPath = guiPath;

		string path;

		if (ProtocolUtility.HasProtocol(guiPath))
		{
			path = ProtocolUtility.ConvertToFilePath(guiPath) + ".xml"; 
		}
		else
		{
			path = guiPath;
		}

		FileInfo fileInfo = new FileInfo(path);
		string filePath = "file://" + path;

		mLoadedGuiLabel.Text = fileInfo.Name;

		ClearUserGuis();

		try
		{
			GuiController userGuis = new GuiController(mManager, filePath);
			foreach(ITopLevel topLevel in userGuis.AllGuis)
			{
				mManager.SetTopLevelPosition(topLevel, new FixedPosition(0.0f, 0.0f, GuiAnchor.CenterCenter, GuiAnchor.CenterCenter));

				if( mHiddenWindows.Contains(topLevel.Name) )
				{
					topLevel.Showing = false;
				}
			}
			mLoadedGuis.AddRange(userGuis.AllGuis);
		}
		catch (GuiConstructionException ex)
		{
			Debug.LogError(ex);
			ShowError(ex.Message);
		}

		mCurrentGuiMonitors.Clear();
		MonitorIncludes(path);

		SetupLoadedGuiWindowList();
	}

	private void MonitorIncludes(string initialPath)
	{
		List<string> pathsToScanForIncludes = new List<string>();
		pathsToScanForIncludes.Add(initialPath);
		mCurrentGuiMonitors.Add(new FileWatcher(new FileInfo(initialPath), ReloadCurrentGui));

		while( pathsToScanForIncludes.Count != 0 )
		{
			XmlDocument currentDocumentToScan = new XmlDocument();
			currentDocumentToScan.Load(pathsToScanForIncludes[0]);
			pathsToScanForIncludes.RemoveAt(0);

			foreach(XmlNode includeNode in currentDocumentToScan.SelectNodes("//Include/@path"))
			{
				string path = ProtocolUtility.ConvertToFilePath(includeNode.InnerText) + ".xml";
				pathsToScanForIncludes.Add(path);
				mCurrentGuiMonitors.Add(new FileWatcher(new FileInfo(path), ReloadCurrentGui));
			}
		}
	}

	private void ClearUserGuis()
	{
		foreach (ITopLevel topLevel in mLoadedGuis)
		{
			mManager.UnregisterTopLevel(topLevel);
		}
		mLoadedGuis.Clear();
	}
}

