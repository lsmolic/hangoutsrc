/**  --------------------------------------------------------  *
 *   IGuiStyle.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public interface IGuiStyle
	{
		UnityEngine.GUIStyle GenerateUnityGuiStyle(); /// Overload for enabled = true
		UnityEngine.GUIStyle GenerateUnityGuiStyle(bool enabled);

		IEnumerable<System.Type> DefaultFor(); /// Get a list of all the types this is a default for

		string Name { get; set; }
		StyleState Normal { get; set; }
		StyleState Hover { get; set; }
		StyleState Active { get; set; }
		StyleState Disabled { get; set; }
		GuiMargin InternalMargins { get; set; }
		GuiMargin ExternalMargins { get; set; }
		GuiMargin NinePartScale { get; set; }
		Font DefaultFont { get; set; }
		GuiAnchor DefaultTextAnchor { get; set; }
		bool WordWrap { get; set; }
		bool ClipText { get; set; }
	}
}