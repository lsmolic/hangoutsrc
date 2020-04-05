using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public interface IGameStateManager 
	{
		void AddState( State state );
		void RemoveState( State state );
	}	
}
