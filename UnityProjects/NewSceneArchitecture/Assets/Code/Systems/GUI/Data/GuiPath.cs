/**  --------------------------------------------------------  *
 *   GuiPath.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class GuiPath 
	{
		private readonly List<string> mLocations;
		
		public GuiPath(string pathStr) 
		{
			mLocations = new List<string>(pathStr.Split(new char[1]{'/'}, StringSplitOptions.RemoveEmptyEntries));
			
			if( mLocations.Count == 0 ) 
			{
				throw new ArgumentException("Unable to use path(" + pathStr + "), no valid tolkens found.", "pathStr");
			}
		}
		
		private static string PathListToString(IEnumerable<string> pathList) 
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
			foreach(string location in pathList) 
			{
				sb.Append("/");
				sb.Append(location);
			}
			
			return sb.ToString();
		}
		
		public override string ToString() 
		{
			return PathListToString(mLocations);
		}
		
		/// Traverse the heiarchy of IGuiContainers to find the specified path element
		public IGuiElement[] SelectElements(IGuiContainer root) 
		{ 
			if( root == null ) 
			{
				throw new ArgumentNullException("root");
			}
			
			List<IGuiElement> results = new List<IGuiElement>(); 
			List<string> toTraverse = new List<string>(mLocations); 
			if( toTraverse.Count != 0 ) 
			{
				if( toTraverse[0] == "**" ) 
				{
					toTraverse.RemoveAt(0);
					foreach( IGuiContainer container in GetAllElementsOfType<IGuiContainer>(root) ) 
					{
						if( container is IGuiElement ) 
						{
							results.AddRange(RecurseFindInContainer(container, toTraverse));
						}
					}
				} 
				else 
				{
					results = RecurseFindInContainer(root, toTraverse);
				}
			}
			
			return results.ToArray(); 
		}
		
		private static IEnumerable<T> GetAllElementsOfType<T>(IGuiContainer root) 
		{
			List<T> results = new List<T>(); 			
			
			if( root is T ) 
			{
				results.Add((T)root);
			}
			
			foreach( IGuiElement child in root.Children ) 
			{
				if( child is IGuiContainer ) 
				{
					results.AddRange( GetAllElementsOfType<T>((IGuiContainer)child) );
				} 
				else if ( child is T ) 
				{
					results.Add((T)child);
				}
			}
			
			return results;
		}
		
		/// Select all elements with a given type
		/// TODO: BUG: Long paths are not workin' out so goodly.  FIGURE OUT LATER.  Work around, use 
		/// shorter queries via lower level elements in the xml hierarchy. Le Pherg.
		public IEnumerable<T> SelectElements<T>(IGuiContainer root) where T : IGuiElement 
		{
			List<T> results = new List<T>(); 
			
			foreach( IGuiElement element in this.SelectElements(root) ) 
			{
				if( element is T ) 
				{
					results.Add((T)element);
				}
			}
			
			return results;
		}
		
		public IGuiElement SelectSingleElement(IGuiContainer root) 
		{ 
			// can be optimized if rewritten to not use SelectElements
			IGuiElement[] elements = SelectElements(root); 
			if( elements.Length == 0 ) 
			{ 
				return null; 
			} 
			return elements[0]; 
		}

		public T SelectSingleElement<T>(IGuiContainer root) where T : IGuiElement
		{
			// can be optimized if rewritten to not use SelectElements
			foreach( IGuiElement element in SelectElements(root)) 
			{
				if( element is T )
				{
					return (T)element;
				}
			}
			return default(T);
		} 
		
		private List<IGuiElement> RecurseFindInContainer(IGuiContainer root, List<string> toTraverse) 
		{
			/*Debug.Log
			(
				"Scanning " + root.ContainerName + "\n" + 
				Functionals.Reduce<string, string>
				(
					delegate(string a, string b)
					{ 
						return a + "\n" + b; 
					}, 
					toTraverse
				)
			);*/

			List<IGuiElement> result = new List<IGuiElement>(); 
			
			// is this level the leaf of the path? (Base Case)
			if( toTraverse.Count == 1 ) 
			{
				result.AddRange(GetAllMatchingChildrenInContainer(root, toTraverse[0]));
			} 
			else if( toTraverse.Count > 1 ) 
			{ 
				List<string> subPath = new List<string>(toTraverse);
				subPath.RemoveAt(0);

				if (toTraverse[0] == "..")
				{
					if (root is IGuiElement)
					{
						// Instead of going down 1 level, we have to go up 1
						IGuiElement rootElement = (IGuiElement)root;
						if (rootElement.Parent != null && rootElement.Parent is IGuiContainer)
						{
							result.AddRange(RecurseFindInContainer((IGuiContainer)rootElement.Parent, subPath));
						}
					}
				}
				else
				{
					foreach (IGuiElement child in root.Children)
					{
						if (child != null && (child is IGuiContainer) && NameMatches(child.Name, toTraverse[0]))
						{
							result.AddRange(RecurseFindInContainer((IGuiContainer)child, subPath));
						}
					}
				}
			}
			
			return result;
		}
		
		IEnumerable<IGuiElement> GetAllMatchingChildrenInContainer(IGuiContainer container, string pattern) 
		{
			List<IGuiElement> result = new List<IGuiElement>(); 
			
			foreach( IGuiElement child in container.Children ) 
			{ 
				if( child != null && NameMatches(child.Name, pattern) ) 
				{ 
					result.Add(child); 
				} 
			}
			
			return result;
		}
		
		
		private static bool NameMatches(string elementName, string pattern) 
		{
			bool match = pattern == "*" || elementName == pattern;
			return match;
		}
	}	
}
