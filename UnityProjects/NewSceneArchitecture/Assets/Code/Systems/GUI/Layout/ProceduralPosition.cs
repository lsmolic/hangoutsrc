/**  --------------------------------------------------------  *
 *   ProceduralPosition.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/18/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class ProceduralPosition : IGuiPosition
	{
        private Hangout.Shared.Func<Vector2, IGuiElement> mPositionFunc = null;

		/// <summary>
		/// This function must be set before GetPosition is called or it will throw an exception.
		/// </summary>
        public Hangout.Shared.Func<Vector2, IGuiElement> PositionFunc
		{
			get { return mPositionFunc; }
			set { mPositionFunc = value; }
		}

		private Hangout.Shared.Action<IGuiElement, Vector2> mMoveFunc = null;

		/// <summary>
		/// Set this to handle widget movement calls (optional)
		/// </summary>
        public Hangout.Shared.Action<IGuiElement, Vector2> MoveFunc
		{
			get { return mMoveFunc; }
			set { mMoveFunc = value; }
		}

		public void UpdatePosition(IGuiElement element, Vector2 newPosition)
		{
			if( mMoveFunc != null )
			{
				mMoveFunc(element, newPosition);
			}
		}

		public Vector2 GetPosition(IGuiElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if( mPositionFunc == null )
			{
				throw new Exception("The procedural position associated with Element(" + element.Name + ") hasn't been set up. (PositionFunc is still null)");
			}
			return mPositionFunc(element);
		}
	}
}
