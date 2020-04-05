/* Pherg 11/16/09
 * Base class for Simple and Complex UvAnimations
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client
{	
	public class UvAnimation
	{
		private readonly string mName;
		public string Name
		{
			get { return mName; }
		}
		
		public UvAnimation(string name)
		{
			mName = name;
		}
	}
}
