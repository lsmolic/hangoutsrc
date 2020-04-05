/*
 * This is Pherg's now.  No more bitta-ownership.
 * 10/07/09
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class Motor : IDisposable
	{
		private float mRotationalVelocity = 5.0f;
		private float mMovementDistanceThreshold = 0.025f;
		
		private Rigidbody mRigidBody = null;
		
		private Vector3 mCurrentVelocity = Vector3.zero;
		public Vector3 CurrentVelocity
		{
			get { return mCurrentVelocity; }
		}
		
		public Vector3 Position
		{
			get { return mRigidBody.transform.position; }
		}
		
		private IScheduler mScheduler;
		
		private ITask mClickWalkTask = null;
		
		private ITask mMoveInDirectionTask = null;
		
		//private Vector3 mMoveDirection = Vector3.zero;
		private bool mMoveLeft = false;
		private bool mMoveRight = false;
		private bool mMoveUp = false;
		private bool mMoveDown = false;
		
		private LocalAvatarEntity mLocalAvatarEntity;
		
		private IReceipt mCollisionReceipt = null;
		
		public Motor(LocalAvatarEntity localAvatarEntity)
		{
			mLocalAvatarEntity = localAvatarEntity;
			GameObject gameObjectToMotorize = localAvatarEntity.UnityGameObject;
			if (gameObjectToMotorize == null)
			{
				throw new ArgumentNullException("gameObjectToMotorize");
			}

			mRigidBody = gameObjectToMotorize.GetComponent(typeof(Rigidbody)) as Rigidbody;
			if (mRigidBody == null)
			{
				mRigidBody = gameObjectToMotorize.AddComponent(typeof(Rigidbody)) as Rigidbody;
			}
			
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			
			mCollisionReceipt = CollisionUtility.ListenForCollisions(mRigidBody, StopMovingWhenHitGround);
		}
		
		private void StopMovingWhenHitGround(Collision col)
		{
			if (col.gameObject.layer == GameFacade.WALL_LAYER )
			{
				if (mClickWalkTask != null)
				{
					mClickWalkTask.Exit();
					mRigidBody.velocity = Vector3.zero;
				}
			}
		}
		
		public void AnimateCharacterToPoint(Vector3 moveToPosition)
		{
			//Arrow Key take precedence over mouse click.
			if (mMoveInDirectionTask != null)
			{
				return;
			}
			if (mClickWalkTask != null)
			{
				mClickWalkTask.Exit();
			}
			float distanceToMoveSquared = Vector3.SqrMagnitude(mRigidBody.position - moveToPosition);
			if (distanceToMoveSquared > mMovementDistanceThreshold)
			{
				mClickWalkTask = mScheduler.StartCoroutine(WalkToPosition(moveToPosition));
			}
			else
			{
				StopMovement();
			}	
		}
		
		public void StopMovement()
		{
			if (mClickWalkTask != null)
			{
				mClickWalkTask.Exit();
			}
			if (mMoveInDirectionTask != null)
			{
				mMoveInDirectionTask.Exit();
				mMoveInDirectionTask = null;
			}
			
			mRigidBody.velocity = Vector3.zero;
		}
		
		private IEnumerator<IYieldInstruction> WalkToPosition(Vector3 moveToPosition)
		{
 			moveToPosition.y = mRigidBody.position.y;
 			Vector3 startPosition = mRigidBody.position;
 			Vector3 moveDirection = moveToPosition - mRigidBody.position;
 			moveDirection.Normalize();
			
			float totatalSquareDistance = Vector3.SqrMagnitude(mRigidBody.position - moveToPosition);
			float traveledSquareDistance = 0.0f;
			
			yield return new FixedYieldWhile(delegate()
			{
				MoveInDirection(moveDirection);
				RotateTowardsMovementDirection(moveDirection);
				
				traveledSquareDistance = Vector3.SqrMagnitude(mRigidBody.position - startPosition);
				return traveledSquareDistance < totatalSquareDistance;
			});
			
			yield return new YieldUntilFixedUpdate();
			StopMovement();
			mRigidBody.position = moveToPosition;
		}
		
		public void WalkLeftRelationalToTransform(Transform transform)
		{
			mMoveLeft = true;
			if (mMoveInDirectionTask == null)
			{
				mMoveInDirectionTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(MoveInDirectionRelationalToTransform(transform));
			}
		}

		public void WalkRightRelationalToTransform(Transform transform)
		{
			mMoveRight = true;
			if (mMoveInDirectionTask == null)
			{
				mMoveInDirectionTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(MoveInDirectionRelationalToTransform(transform));
			}
		}


		public void WalkForwardRelationalToTransform(Transform transform)
		{
			mMoveUp = true;
			if (mMoveInDirectionTask == null)
			{
				mMoveInDirectionTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(MoveInDirectionRelationalToTransform(transform));
			}
		}

		public void WalkBackwardRelationalToTransform(Transform transform)
		{
			mMoveDown = true;
			if (mMoveInDirectionTask == null)
			{
				mMoveInDirectionTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(MoveInDirectionRelationalToTransform(transform));
			}
		}
		public void StopMovementLeft()
		{
			mMoveLeft = false;
			CheckIfMovementIsFinished();
		}
		
		public void StopMovementRight()
		{
			mMoveRight = false;
			CheckIfMovementIsFinished();
		}
		
		public void StopMovementForward()
		{
			mMoveUp = false;
			CheckIfMovementIsFinished();
		}
		
		public void StopMovementBackward()
		{
			mMoveDown = false;
			CheckIfMovementIsFinished();
		}

		private void CheckIfMovementIsFinished()
		{
			if (mMoveLeft == false && mMoveRight == false && mMoveUp == false && mMoveDown == false)
			{
				StopMovement();
			}
		}

		private IEnumerator<IYieldInstruction> MoveInDirectionRelationalToTransform(Transform transform)
		{
			if (mClickWalkTask != null)
			{
				mClickWalkTask.Exit();
			}
			
			while (true)
			{
				Vector3 resultDirection = Vector3.zero;
				if (mMoveLeft)
				{
					Vector3 transformLeft = transform.right * -1.0f;
					resultDirection += ProjectDirectionOnToxz(transformLeft);
				}
				
				if (mMoveRight)
				{
					Vector3 transformRight = transform.right;
					resultDirection += ProjectDirectionOnToxz(transformRight);
				}

				if (mMoveUp)
				{
					Vector3 transformForward = transform.forward;
					resultDirection += ProjectDirectionOnToxz(transformForward);
				}

				if (mMoveDown)
				{
					Vector3 transformBackward = transform.forward * -1.0f;
					resultDirection += ProjectDirectionOnToxz(transformBackward);
				}
				
				resultDirection.Normalize();
				MoveInDirection(resultDirection);
				
				RotateTowardsMovementDirection(resultDirection);
				
				yield return new YieldUntilFixedUpdate();
			}
		}
		
		private Vector3 ProjectDirectionOnToxz(Vector3 direction)
		{
			direction.y = 0.0f;
			direction.Normalize();
			return direction;
		}
		
		private void MoveInDirection(Vector3 direction)
		{
			mRigidBody.AddForce(direction * mLocalAvatarEntity.MaxWalkSpeed - mRigidBody.velocity, ForceMode.VelocityChange);
		}
		
		private void RotateTowardsMovementDirection(Vector3 movementDirection)
		{
			Vector3 startLookAtVector = mRigidBody.transform.forward;
			Vector3 destinationLookAtVector = movementDirection;
			float rotationalVelocity = mRotationalVelocity;
			
			if (destinationLookAtVector == Vector3.zero)
			{
				destinationLookAtVector = startLookAtVector;
			}
			
			if (Vector3.Dot(startLookAtVector, destinationLookAtVector) < 0.69)
			{
				rotationalVelocity *= 1.5f;
			}

			float rotationInRadians = ((rotationalVelocity * Time.fixedDeltaTime));

			mRigidBody.transform.forward = Vector3.RotateTowards(mRigidBody.transform.forward, destinationLookAtVector, rotationInRadians, 1.0f);
		}

		private float ExponentialLerp(float start, float end, float value)
		{
			float returnValue = ((value - start) / (end - start));
			
			return Mathf.Pow(2.0f, returnValue - (end - start));
		}
		
		public void Dispose()
		{
			StopMovement();
			mCollisionReceipt.Exit();
		}
	}
}