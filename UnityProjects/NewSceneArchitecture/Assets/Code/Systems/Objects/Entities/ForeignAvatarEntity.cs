/**  --------------------------------------------------------  *
 *   ForeignAvatarEntity.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/27/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class ForeignAvatarEntity : AvatarEntity
	{
		public ForeignAvatarEntity(GameObject avatarObject, GameObject headObject)
			: base(avatarObject, headObject)
		{
		}
	}
}
