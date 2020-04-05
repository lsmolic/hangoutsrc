/**  --------------------------------------------------------  *
 *   Tweakable.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	[AttributeUsage(AttributeTargets.Field)]
	public class Tweakable : Attribute 
	{
		private readonly string mName;
		
		public string Name
		{
			get { return mName; }
		}
		
		public Tweakable(string name) 
		{
			mName = name;
		}
	}
}
