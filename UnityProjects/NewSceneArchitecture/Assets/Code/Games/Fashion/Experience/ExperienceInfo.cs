/**  --------------------------------------------------------  *
 *   ExperienceInfo.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;

using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.FashionGame;

using PureMVC.Patterns;

using UnityEngine;

namespace Hangout.Client.FashionGame
{
	public class ExperienceInfo
	{
		private readonly ExperienceType mType;
		public ExperienceType Type
		{
			get { return mType; }
		}

		private readonly Vector3? mPosition;
		public Vector3? Position
		{
			get { return mPosition; }
		}

		private readonly uint? mMultipleEvent;
		public uint? MultipleEvent
		{
			get { return mMultipleEvent; }
		}

		public ExperienceInfo(ExperienceType type)
			: this (type, null, null)
		{
		}

		public ExperienceInfo(ExperienceType type, Vector3? position)
			: this(type, position, null)
		{
		}

		public ExperienceInfo(ExperienceType type, uint? multipleEvent)
			: this(type, null, multipleEvent)
		{
		}

		public ExperienceInfo(ExperienceType type, Vector3? position, uint? multipleEvent)
		{
			mType = type;
			mPosition = position;
			mMultipleEvent = multipleEvent;
		}
	}
}
