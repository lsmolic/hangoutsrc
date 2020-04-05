using UnityEngine;
using System;
using System.Collections;
using Hangout.Shared;

namespace Hangout.Client
{
	public interface ICameraManager : IScheduler
	{
		GameObject MainCamera { get; }
	}
}
