/**  --------------------------------------------------------  *
 *   Entity.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public interface IEntity : IDisposable
	{
		GameObject UnityGameObject { get; }
	}	
}
