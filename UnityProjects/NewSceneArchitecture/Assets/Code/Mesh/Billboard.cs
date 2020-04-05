/*
	Created by Vilas Tewari on 2009-07-21.
   
	A Class that create a single Z facing mesh billboard
*/

using System;
using UnityEngine;
using System.Collections.Generic;
using Hangout.Shared;


namespace Hangout.Client
{
	public class Billboard : IDisposable
	{
		private Mesh mMesh;
		private MeshFilter mMeshFilter;
		private MeshRenderer mMeshRenderer;

		private GameObject mBillboardGameObject;
		private Transform mBillboardTransform;

		private ITask mBillboard = null;
		public void BillboardToCamera(Camera camera)
		{
			if( camera == null )
			{
				throw new ArgumentNullException("camera");
			}
			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;

			if( mBillboard != null )
			{
				mBillboard.Exit();
			}
			mBillboard = scheduler.StartCoroutine(KeepBillboarded(camera));
		}
		private IEnumerator<IYieldInstruction> KeepBillboarded(Camera camera)
		{
			while(camera)
			{
				mBillboardTransform.LookAt(camera.transform);
				yield return new YieldUntilNextFrame();
			}
		}
		public void SetTexture(Texture2D texture)
		{
			if( texture == null )
			{
				throw new ArgumentNullException("texture");
			}
			texture.Apply();
			mMeshRenderer.material.mainTexture = texture;
		}
		public UnityEngine.GameObject BillboardGameObject
		{
			get { return mBillboardGameObject; }
		}
		public Vector3 Center
		{
			get { return mBillboardTransform.position; }
			set { mBillboardTransform.position = value; }
		}
		public Bounds Bounds
		{
			get { return mMeshRenderer.bounds; }
		}
		public Vector3 Scale
		{
			get { return mBillboardTransform.localScale; }
			set { mBillboardTransform.localScale = value; }
		}
		public float Transparency
		{
			get { return mMeshRenderer.material.color.a; }
			set
			{
				Color newColor = mMeshRenderer.material.color;
				newColor.a = value;
				mMeshRenderer.material.color = newColor;
			}
		}
		
		public Color Color
		{
			get { return mMeshRenderer.material.color; }
			set { mMeshRenderer.material.color = value; }
		}

		public Transform Transform
		{
			get { return mBillboardTransform; }
		}

		public bool Visible
		{
			get { return mBillboardGameObject.active; }
			set { mBillboardGameObject.active = value; }
		}

		public Billboard()
		{
			mMesh = new Mesh();
			mBillboardGameObject = new GameObject("Billboard 3D") as GameObject;
			mBillboardTransform = mBillboardGameObject.transform;
			mMeshFilter = mBillboardGameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
			mMeshRenderer = mBillboardGameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
			mMeshFilter.mesh = mMesh;

			
			// Setup Verts, UVs, Tris
			Vector3[] billboardVerts = new Vector3[8];
			float margin = 0.4f;
			billboardVerts[0] = new Vector3(-1, 1, 0);
			billboardVerts[1] = new Vector3(-1 + margin, 1, 0);
			billboardVerts[2] = new Vector3(1 - margin, 1, 0);
			billboardVerts[3] = new Vector3(1, 1, 0);
			billboardVerts[4] = new Vector3(1, -1, 0);
			billboardVerts[5] = new Vector3(1 - margin, -1, 0);
			billboardVerts[6] = new Vector3(-1 + margin, -1, 0);
			billboardVerts[7] = new Vector3(-1, -1, 0);

			mMesh.vertices = billboardVerts;

			Vector2[] uvs = new Vector2[8];
			for (int x = 0; x < uvs.Length; ++x)
			{
				uvs[x] = new Vector2(billboardVerts[x].x, billboardVerts[x].y) / 2 + new Vector2(0.5f, 0.5f);
				uvs[x].x = 1.0f - uvs[x].x;
			}
			mMesh.uv = uvs;

			mMesh.triangles = new int[18] 
			{ 
				0,7,1, 
				1,7,6,
				1,6,2,
				2,6,5,
				2,5,3,
				3,5,4,
			};
			mMesh.RecalculateNormals();
			mMesh.RecalculateBounds();

			MeshCollider meshCollider = mBillboardGameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
			meshCollider.sharedMesh = mMesh;

			SetMaterial(new Material(Shader.Find("Transparent/Diffuse")));
		}

		public void SetMaterial(Material mat)
		{
			mMeshRenderer.material = mat;
		}

		public void Dispose()
		{
			UnityEngine.GameObject.Destroy(mBillboardGameObject);
			if (mBillboard != null)
			{
				mBillboard.Exit();
				mBillboard = null;
			}
		}

		public void XPartScale(Vector3 scale) 
		{
			float xScale = scale.x;
			float yScale = scale.y;
			float margin = 0.4f * scale.y;
			// Setup Verts, UVs, Tris
			Vector3[] billboardVerts = new Vector3[8];
			billboardVerts[0] = new Vector3(-xScale, yScale, 0);
			billboardVerts[1] = new Vector3(-xScale + margin, yScale, 0);
			billboardVerts[2] = new Vector3(xScale - margin, yScale, 0);
			billboardVerts[3] = new Vector3(xScale, yScale, 0);
			billboardVerts[4] = new Vector3(xScale, -yScale, 0);
			billboardVerts[5] = new Vector3(xScale - margin, -yScale, 0);
			billboardVerts[6] = new Vector3(-xScale + margin, -yScale, 0);
			billboardVerts[7] = new Vector3(-xScale, -yScale, 0);

			mMesh.vertices = billboardVerts;
		}
	}
}