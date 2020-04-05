/* Pherg 10/13/09 */

//This is the state to put state machines in when they unregister or are constructed without
//any specific state to transition into.  This makes sure the current state Exits so cleanup
//happens.


namespace Hangout.Client
{
	public class InactiveState : State
	{
		public InactiveState()
		{
		}

		public override void EnterState()
		{
		}

		public override void ExitState()
		{
		}
	}
}
