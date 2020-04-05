/**  --------------------------------------------------------  *
 *   INpc.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/25/2009
 *	 
 *   --------------------------------------------------------  *
 */



using UnityEngine;

namespace Hangout.Client
{
	/// <summary>
	/// Non-Player Character
	/// </summary>
	public interface INpc : IEntity
    {
		string Name { get; }
		bool Active { get; }
		GameObject DisplayObject { get; }

		HeadController HeadController { get; }
		BodyController BodyController { get; }
    }
}
