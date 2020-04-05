/*	A container is a space that confines a point	*/
	
using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public abstract class ViewContainer : IEntity
	{
		// Optimization
		private Transform mContainerTransform;
		protected Transform ContainerTransform
		{
			get { return mContainerTransform; }
		}

		public ViewContainer()
		{
			GameObject containerGameObject = new GameObject("ViewContainer");
			mContainerTransform = containerGameObject.transform;
		}
		
		public virtual Vector3 ConstrainToContainer( Vector3 point )
		{
			// if point is inside space, return point
			if ( IsPointInsideContainer( point ) )
			{
				return point;
			}
			// Else project it on to space surface
			else
			{
				return ClosestPointOnContainer( point );
			}
		}

		protected Vector3 ClosestPointOnAABox(Vector3 size, Vector3 point)
		{
			Vector3 pointOnCube = new Vector3();
			pointOnCube.x = Mathf.Clamp(point.x, -size.x / 2, size.x / 2);
			pointOnCube.y = Mathf.Clamp(point.y, -size.y / 2, size.y / 2);
			pointOnCube.z = Mathf.Clamp(point.z, -size.z / 2, size.z / 2);
			return pointOnCube;
		}
		
		protected bool AABox(Vector3 size, Vector3 point)
		{
			Rect topDown = new Rect(-size.x / 2, size.z / 2, size.x, size.z);
			Rect side = new Rect(-size.z / 2, size.y / 2, size.z, size.y);
			if (topDown.Contains(point) && side.Contains(point))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public abstract bool IsPointInsideContainer( Vector3 point );
		public abstract Vector3 ClosestPointOnContainer( Vector3 point );

		#region IEntity Members

		public GameObject UnityGameObject
		{
			get { return mContainerTransform.gameObject; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			GameObject.Destroy(mContainerTransform.gameObject);
		}

		#endregion
	}
}