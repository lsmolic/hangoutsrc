/*
   Created by Vilas Tewari on 2009-07-29.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class UvShell
	{
		/* The space within (0,0) and (1,1) that this UV shell is confined to
		Atlas.x and .y define the BOTTOM left corner */
		private Rect mAtlas;

		private int[] mVertexIndices; // The indices of the vert that belong to this UV shell
		private Vector2[] mInitialUvPositions; // The initial position of all he uvs in this shell

		private Vector2 mShellPosition;
		private Vector2 mShellOffset;
		private Vector2 mShellCenter;
		private Vector2 mShellScale;

		private float mShellRotation;
		private float cosTheta;
		private float sinTheta;

		private Vector2 mAtlasPosition;
		private Vector2 mAtlasDimesions;

		/*
			Properties
		*/
		public Vector2 Position
		{
			get { return mShellPosition; }
			set { mShellPosition = new Vector2(value.x, -value.y); }
		}

		public Vector2 Offset
		{
			get { return mShellOffset; }
			set { mShellOffset = value; }
		}

		public Vector2 Scale
		{
			get { return mShellScale; }
			set { mShellScale = value; }
		}

		public float Rotation
		{
			get { return mShellRotation; }
			set
			{
				mShellRotation = value;
				cosTheta = Mathf.Cos(mShellRotation);
				sinTheta = Mathf.Sin(mShellRotation);
			}
		}

		public Rect Atlas
		{
			set
			{
				mAtlas = value;
				mAtlasPosition = new Vector2(value.x, value.y);
				mAtlasDimesions = new Vector2(value.width, value.height);
			}
			get { return mAtlas; }
		}

		public UvShell(Mesh mesh, int subMeshIndex)
		{
			/*
				Find out which verts belong to subMeshIndex one
				Also save the corresponding uvs for those verts
			*/
			SetVertexIndicesAndUvs(mesh, subMeshIndex);

			/* Set the UV shell position to zero */
			mShellPosition = Vector2.zero;

			/* Assume the uvs are constrained to the entire 0,0 to 1,1 uv space */
			mAtlas = new Rect(0, 0, 1f, 1f);

			Offset = Vector2.zero;
			Scale = new Vector2(1f, 1f);
			Rotation = 0;
		}

		public void UpdateUvs(Vector2[] uvs)
		{
			// Update the position of the UVs that belong to this shell
			for (int x = 0; x < mVertexIndices.Length; x++)
			{
				// Move to Shell Center
				Vector2 newPosition = mInitialUvPositions[x] - mShellCenter;

				newPosition.x *= mShellScale.x;
				newPosition.y *= mShellScale.y;

				newPosition.x = cosTheta * newPosition.x + newPosition.y * sinTheta;
				newPosition.y = -sinTheta * newPosition.x + newPosition.y * cosTheta;

				newPosition += mShellCenter + mShellPosition + mShellOffset;
				newPosition = Vector2.Scale(newPosition, mAtlasDimesions);
				uvs[mVertexIndices[x]] = newPosition + mAtlasPosition;
			}
		}

		private void SetVertexIndicesAndUvs(Mesh mesh, int subMeshIndex)
		{
			int[] triangles = mesh.GetTriangles(subMeshIndex);
			List<int> vertIndices = new List<int>();
			List<Vector2> uvList = new List<Vector2>();

			/* Extract a set of verts that belong to subMeshIndex by looking at the triangles */
			for (int x = 0; x < triangles.Length; ++x)
			{
				int vertexIndex = triangles[x];
				if (!vertIndices.Contains(vertexIndex))
				{
					vertIndices.Add(vertexIndex);
					uvList.Add(mesh.uv[vertexIndex]);
				}
			}
			/* Store vert indices and initial uv positions */
			mVertexIndices = vertIndices.ToArray();
			mInitialUvPositions = uvList.ToArray();

			CalcualteShellCenter();
		}

		private void CalcualteShellCenter()
		{
			mShellCenter = Vector2.zero;

			if (mInitialUvPositions.Length > 0)
			{
				float uMin = 1;
				float vMin = 1;
				float uMax = 0;
				float vMax = 0;

				for (int x = 0; x < mInitialUvPositions.Length; ++x)
				{
					uMin = Mathf.Min(mInitialUvPositions[x].x, uMin);
					vMin = Mathf.Min(mInitialUvPositions[x].y, vMin);

					uMax = Mathf.Max(mInitialUvPositions[x].x, uMax);
					vMax = Mathf.Max(mInitialUvPositions[x].y, vMax);
				}

				mShellCenter.x = (uMax - uMin) / 2;
				mShellCenter.y = 1 - ((vMax - vMin) / 2);
			}
		}
	}
}
