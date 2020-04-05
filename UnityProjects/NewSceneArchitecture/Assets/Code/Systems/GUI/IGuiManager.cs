/**  --------------------------------------------------------  *
 *   IGuiManager.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using Hangout.Shared;
using UnityEngine;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	/// <summary>
	/// IGuiManager is responsible for drawing registered ITopLevels, managing default styles and GUI Logging
	/// </summary>
	public interface IGuiManager : IGuiContainer
	{
		void RegisterTopLevel(ITopLevel topLevel, IGuiPosition position);
		
		void SetTopLevelPosition(ITopLevel topLevel, IGuiPosition position);
        
		IGuiPosition GetTopLevelPosition(ITopLevel topLevel);

        void UnregisterTopLevel(ITopLevel topLevel);

		/// <summary>
		/// Gets the default style for the explicit Element type (not by interface type)
		/// </summary>
		IGuiStyle GetDefaultStyle(System.Type elementType);

		/// <summary>
		/// Is the given point blocked by any GUI?
		/// </summary>
		ICollection<ITopLevel> OccludingTopLevels(Vector2 screenSpacePoint);

		ILogger Logger { get; }
	}
}