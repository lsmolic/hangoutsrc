/**  --------------------------------------------------------  *
 *   StaticPositionFollowTargetCameraStrategy.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/01/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

namespace Hangout.Client
{
	public interface ICameraStrategy 
	{
		void Activate();
		void Deactivate();
	}
}
