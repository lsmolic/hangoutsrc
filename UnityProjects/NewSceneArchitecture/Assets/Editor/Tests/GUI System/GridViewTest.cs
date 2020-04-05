/**  --------------------------------------------------------  *
 *   GridViewTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/14/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class GridViewTest
	{
		[Test]
		public void GridLayoutVerification()
		{
			List<IWidget> gridWidgets = new List<IWidget>();
			GridView grid = new GridView("Grid Layout Test", new FixedSize(100, 100), new List<KeyValuePair<IWidget, IGuiPosition>>(), null);			
			
			for(int i = 0; i < 4; ++i)
			{
				Button testButton = new Button("GridButton", new FixedSize(10, 10), null);
				gridWidgets.Add(testButton);
			}
			grid.SetPositions(gridWidgets, 2, 2, 0);
			
			for(int i = 0; i < 4; ++i)
			{
				int j = i / 2;
				Vector2 expected = new Vector2(50 * (i % 2), 50 * j);
				Assert.AreEqual(expected, grid.GetChildPosition(gridWidgets[i]));
			}
			
		}
	}
}
