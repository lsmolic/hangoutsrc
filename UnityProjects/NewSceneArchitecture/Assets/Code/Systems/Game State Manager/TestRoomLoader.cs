using UnityEngine;
using System.Collections;

/*
This is the only script on the only game object in the initial scene.
It is in charge of building out all the objects in the game's backend
to run.  After it builds those it will build the initial game state 
and start the game.
*/

//Deprecated

/*public class TestRoomLoader : MonoBehaviour {
	
	private IScheduler mIScheduler;
	private IGameStateManager mIGameStateManager;
	private ICameraManager mICameraManager;
	private CameraTestStrat mCameraTestStrat;
	private ITask mCameraManagerTestITask;
	
	
	private void Awake() {
		BuildManagers();
		Init();		
	}
	
	//Create all managers and objects needed in the backend architecture.
	private void BuildManagers() {
		GameStateManager gameStateManager = new GameStateManager();
		mIGameStateManager = (IGameStateManager) gameStateManager;
		
		GameObject schedulerGameObject = new GameObject();
		schedulerGameObject.name = "Scheduler";
		Scheduler scheduler = schedulerGameObject.AddComponent( typeof(Scheduler) ) as Scheduler;
		mIScheduler = (IScheduler) scheduler;
		
		GameObject cameraGameObject = new GameObject();
		cameraGameObject.name = "Camera";
		Camera camera = cameraGameObject.AddComponent( typeof(Camera) ) as Camera;
		CameraManagerMediator cameraManager = new CameraManagerMediator( mIScheduler, camera );
		mICameraManager = cameraManager;
	}
	
	//Create the initial game state and execute the game.
	private void Init() {
		State defaultState = new State();
		defaultState.AddActivateCallback( Activate );
		defaultState.AddDeactivateCallback( Deactivate );

		mIGameStateManager.AddState( defaultState );
	}
		
	private void CameraManagerTest() {
		if ( Time.time > 4.0f ) {
			mICameraManager.AddStrategy ( mCameraTestStrat );
			mIScheduler.RemoveUpdateTask( mCameraManagerTestITask);
		}
	}
	
	private void Activate ( ){
		Debug.Log("State activated." );
		DefaultCamera defaultCamera = new DefaultCamera();
		mICameraManager.AddStrategy( defaultCamera );
		mCameraTestStrat = new CameraTestStrat();
		mCameraManagerTestITask = mIScheduler.AddUpdateTask( CameraManagerTest );
		//defaultCamera.AddActivateCallback( defaultCamera.Update );
	}
	
	private void Deactivate ( ){
		Debug.Log("State Deactivated." );
	}
}
*/