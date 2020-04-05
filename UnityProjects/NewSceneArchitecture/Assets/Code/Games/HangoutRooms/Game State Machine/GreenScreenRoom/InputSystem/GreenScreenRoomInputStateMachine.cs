
namespace Hangout.Client
{
	public class GreenScreenRoomInputStateMachine : StateMachine
	{
		private GreenScreenRoomDefaultMovementState mGreenScreenRoomDefaultMovementState;
		private GreenScreenRoomNoMovementState mGreenScreenRoomNoMovementState;
		private InactiveState mInactiveState;

		public override void OnRemove()
		{
			if (this.CurrentState != mGreenScreenRoomNoMovementState)
			{
				this.TransitionToState(mGreenScreenRoomNoMovementState);
			}
		}
		
		public GreenScreenRoomInputStateMachine()
		{
			mGreenScreenRoomDefaultMovementState = new GreenScreenRoomDefaultMovementState();
			mGreenScreenRoomNoMovementState = new GreenScreenRoomNoMovementState();
			mInactiveState = new InactiveState();
			
			mGreenScreenRoomDefaultMovementState.AddTransition(mGreenScreenRoomNoMovementState);
			mGreenScreenRoomDefaultMovementState.AddTransition(mInactiveState);
			
			mGreenScreenRoomNoMovementState.AddTransition(mGreenScreenRoomDefaultMovementState);
			mGreenScreenRoomNoMovementState.AddTransition(mInactiveState);

			mInactiveState.AddTransition(mGreenScreenRoomDefaultMovementState);
			mInactiveState.AddTransition(mGreenScreenRoomNoMovementState);

			this.EnterInitialState(mInactiveState);
		}
		
		public void MovementEnabled()
		{
			if (this.CurrentState != mGreenScreenRoomDefaultMovementState)
			{
				this.TransitionToState(mGreenScreenRoomDefaultMovementState);
			}
		}
		
		public void MovementDisabled()
		{
			if (this.CurrentState != mGreenScreenRoomNoMovementState)
			{
				this.TransitionToState(mGreenScreenRoomNoMovementState);
			}
		}
	}
}
