/**  --------------------------------------------------------  *
 *   EditorGuiManager.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	/// <summary>
	/// Manager that will draw windows as Unity tools (and not in-game)
	/// </summary>
	public class EditorGuiManager : IGuiManager
	{
		public bool IsPointInGui(Vector3 point)
		{
			return false;
		}

		private readonly ILogger mLogger;
		public ILogger Logger
		{
			get { return mLogger; }
		}
		public string ContainerName 
		{ 
			get { return this.GetType().Name; } 
		}

		public EditorGuiManager()
		{
			mLogger = new Logger();
			mLogger.AddReporter(new DebugLogReporter());
		}

		public T SelectSingleElement<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectSingleElement<T>(this);
		}
		
		public IEnumerable<T> SelectElements<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectElements<T>(this);
		}

		private readonly static Vector2 mDefaultWindowPosition = new Vector2(256.0f, 256.0f);
		/// Dictionary of editor windows and topLevel position
		private IDictionary<EditorWindowWrapper, Vector2> mRegisteredGuiList = new Dictionary<EditorWindowWrapper, Vector2>();

		public IEnumerable<IGuiElement> Children
		{
			get
			{
				foreach (EditorWindowWrapper wrapper in mRegisteredGuiList.Keys)
				{
					yield return wrapper.InnerTopLevel;
				}
			}
		}

		/// <summary>
		/// The default style for editor elements is the default unity style (null)
		/// </summary>
		/// <returns>null</returns>
		public IGuiStyle GetDefaultStyle(System.Type elementType)
		{
			return null;
		}

		public void RegisterTopLevel(ITopLevel topLevel, IGuiPosition p)
		{
			EditorWindowWrapper wrapper = (EditorWindowWrapper)EditorWindow.GetWindow(typeof(EditorWindowWrapper));
			wrapper.Manager = this;
			wrapper.InnerTopLevel = topLevel;

			mRegisteredGuiList.Add(wrapper, mDefaultWindowPosition);
		}

		/// <summary>
		///	Editor mode TopLevels ignore position (Unity manages them). This function is just a stub
		/// </summary>
		public void SetTopLevelPosition(ITopLevel topLevel, IGuiPosition position)
		{
		}

        /// <summary>
        ///	Editor mode TopLevels ignore position (Unity manages them). This function is just a stub
        /// </summary>
        public IGuiPosition GetTopLevelPosition(ITopLevel topLevel)
        {
            return null;
        }

		public void UnregisterTopLevel(ITopLevel topLevel)
		{
			foreach (EditorWindowWrapper wrapper in mRegisteredGuiList.Keys)
			{
				if (topLevel == wrapper.InnerTopLevel)
				{
					mRegisteredGuiList.Remove(wrapper);
					wrapper.Close();
					return;
				}
			}
		}
		public void UpdateChildPosition(IGuiElement topLevel, Vector2 newPosition)
		{
			foreach (EditorWindowWrapper wrapper in mRegisteredGuiList.Keys)
			{
				if (topLevel == wrapper.InnerTopLevel)
				{
					mRegisteredGuiList[wrapper] = newPosition;
					return;
				}
			}

			throw new System.Exception("Unable to update editor window position for topLevel (" + topLevel.Name + "), topLevel not found in manager.");
		}

		public Vector2 GetPosition(EditorWindowWrapper wrapper)
		{
			if (mRegisteredGuiList.ContainsKey(wrapper))
			{
				return mRegisteredGuiList[wrapper];
			}

			return Vector2.zero;
		}
		
		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			throw new Exception("EditorGuiManager does not use IGuiPositions.");
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			throw new Exception("EditorGuiManager does not use IGuiPositions.");
		}

		public ICollection<ITopLevel> OccludingTopLevels(Vector2 screenSpacePoint)
		{
			// The editor gui manager doesn't have occluding windows
			return new List<ITopLevel>();
		}
	}
}