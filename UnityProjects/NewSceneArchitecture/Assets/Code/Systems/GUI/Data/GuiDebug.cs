/**  --------------------------------------------------------  *
 *   GuiDebug.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/02/2009
 *	 
 *   --------------------------------------------------------  *
 */



using System.Collections.Generic;
using System.Text;


namespace Hangout.Client.Gui
{
	/// <summary>
	/// Collection of Gui Debugging Tools
	/// </summary>
	public class GuiDebug
	{

		public static string GetStructureDescription(IGuiElement root)
		{
			StringBuilder sb = new StringBuilder();

			foreach (string levelString in GetLevelStrings(root, 0))
			{
				sb.AppendLine(levelString);
			}

			return sb.ToString();
		}

		private static IEnumerable<string> GetLevelStrings(IGuiElement root, uint level)
		{
			List<string> results = new List<string>();

			StringBuilder thisLevel = new StringBuilder();
			for (uint i = 0; i < level; ++i)
			{
				thisLevel.Append("\t");
			}
			if (root == null)
			{
				thisLevel.Append("NULL");
				results.Add(thisLevel.ToString());
				return results;
			}
			else
			{
				thisLevel.Append(root.Name);
				results.Add(thisLevel.ToString());
			}

			if (root is IGuiContainer)
			{
				IGuiContainer rootContainer = (IGuiContainer)root;
				foreach (IGuiElement child in rootContainer.Children)
				{
					results.AddRange(GetLevelStrings(child, level + 1));
				}
			}

			return results;
		}
	}
}
