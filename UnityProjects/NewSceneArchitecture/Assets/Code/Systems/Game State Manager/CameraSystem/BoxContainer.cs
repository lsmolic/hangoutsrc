using System;
using UnityEngine;
using System.Collections;

/*	A Box ViewContainer is a cube space that confines a View	*/

namespace Hangout.Client
{
	public class BoxContainer : ViewContainer
	{

		public BoxContainer(Vector3 position, Vector3 scale)
			: base ()
		{
			ContainerTransform.position = position;
			ContainerTransform.localScale = scale;
		}
		public override bool IsPointInsideContainer( Vector3 point )
		{
			return AABox(Vector3.one, ContainerTransform.InverseTransformPoint(point));
		}

		public override Vector3 ClosestPointOnContainer( Vector3 point )
		{
			return ContainerTransform.TransformPoint(ClosestPointOnAABox(Vector3.one, ContainerTransform.InverseTransformPoint(point)));
		}
	}
}
