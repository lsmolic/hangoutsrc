/**  --------------------------------------------------------  *
 *   EditorGuiController.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/04/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client.Gui
{
	public class EditorGuiController : GuiController 
	{
		protected EditorGuiController(IGuiManager manager, string path)
			: base(manager, path)
		{
		}
	}
}
