/**  --------------------------------------------------------  *
 *   AutoLayout.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/03/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public interface IAutoLayout : IGuiPosition
	{
		/// <summary>
		/// Get the best position to reserve for a widget.
		/// </summary>
		/// <param name="widget">The widget that will be placed.</param>
		/// <param name="reservedSpace">The invalid placement locations.</param>
		/// <param name="parentSize">The limits of the space for the widget.</param>
		/// <param name="autolayoutWidgets">All the widgets that use IAutoLayout in the parent.</param>
		/// <returns>The space this IAutoLayout is reserving for the given widget.</returns>
		Rect NextPosition(IWidget widget, IEnumerable<Rect> reservedSpace, Vector2 parentSize, IList<KeyValuePair<IWidget, IAutoLayout>> autolayoutWidgets);
	}
}