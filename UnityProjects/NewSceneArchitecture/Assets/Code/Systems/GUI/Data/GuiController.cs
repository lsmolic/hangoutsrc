/**  --------------------------------------------------------  *
 *   GuiController.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	/// <summary>
	/// This is a utility class that provides a bunch of basic functions for loading and setting up a GUI using the XmlGuiFactory.
	/// </summary>
	public class GuiController : IDisposable
	{
		private readonly IGuiManager mManager;
		public IGuiManager Manager
		{
			get { return mManager; }
		}

		private readonly string mPath;

		private readonly IDictionary<string, IGuiStyle> mStyles = new Dictionary<string, IGuiStyle>();
		private readonly IEnumerable<IGuiElement> mTopLevelElements;

		public GuiController(IGuiManager manager, string guiPath)
		{
			if( manager == null )
			{
				throw new ArgumentNullException("manager");
			}
			mManager = manager;

			if( guiPath == null )
			{
				throw new ArgumentNullException("guiPath");
			}
			mPath = guiPath;

			XmlGuiFactory factory = new XmlGuiFactory(mPath, mManager);

			factory.RegisterOnBuildWidgetProcessor(delegate(IWidget newWidget)
			{
				if( newWidget is Button )
				{
					((Button)newWidget).AddOnPressedAction(delegate()
					{
						GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_BUTTON_PRESS);
					});
				}
			});

			mTopLevelElements = factory.ConstructAllElements();
			foreach( IGuiStyle style in factory.ConstructAllStyles() )
			{
				if( style == null )
				{
					continue;
				}
				mStyles.Add(style.Name, style);
			}
		}
		
		public ITopLevel MainGui
		{
			get
			{
				foreach (ITopLevel topLevel in AllGuis)
				{
					return topLevel;
				}
				throw new Exception("There are no TopLevel GUI Elements in the GUI at: " + mPath);
			}
		}

		public IEnumerable<ITopLevel> AllGuis
		{
			get
			{
				foreach (IGuiElement element in mTopLevelElements)
				{
					if (element is ITopLevel)
					{
						yield return (ITopLevel)element;
					}
				}
			}
		}

		public IEnumerable<IGuiElement> AllElements
		{
			get{ return mTopLevelElements; }
		}

		public IEnumerable<IGuiStyle> AllStyles
		{
			get{ return mStyles.Values; }
		}

		public IGuiStyle GetNamedStyle(string styleName)
		{
			IGuiStyle style;
			if( mStyles.TryGetValue(styleName, out style) )
			{
				return style;
			}
			throw new Exception("There is no style named '" + styleName + "' in the GUI XML that was loaded from '" + mPath + "'");
		}

		public virtual void Dispose()
		{
			foreach(ITopLevel gui in this.AllGuis)
			{
				gui.Close();
			}
		}
	}
}
