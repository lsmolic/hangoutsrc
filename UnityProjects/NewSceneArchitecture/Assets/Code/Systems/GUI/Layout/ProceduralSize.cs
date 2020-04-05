/**  --------------------------------------------------------  *
 *   ProceduralSize.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/17/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class ProceduralSize : IGuiSize
	{
		private Hangout.Shared.Func<Vector2, IGuiElement> mSizeFunc = null;

		/// <summary>
		/// This function must be set before GetSize is called or it will throw an exception.
		/// </summary>
		public Hangout.Shared.Func<Vector2, IGuiElement> SizeFunc
		{
			get { return mSizeFunc; }
			set { mSizeFunc = value; }
		}

		public ProceduralSize(Hangout.Shared.Func<Vector2, IGuiElement> sizeFunc)
		{
			if( sizeFunc == null )
			{
				throw new ArgumentNullException("sizeFunc");
			}
			mSizeFunc = sizeFunc;
		}

		public ProceduralSize()
		{
		}

		public Vector2 GetSize(IGuiElement element)
		{
			if( element == null )
			{
				throw new ArgumentNullException("element");
			}
			if( mSizeFunc == null )
			{
				throw new Exception("The procedural size associated with Element(" + element.Name + ") hasn't been set up. (SizeFunc is still null)");
			}
			return mSizeFunc(element);
		}
	}
}
