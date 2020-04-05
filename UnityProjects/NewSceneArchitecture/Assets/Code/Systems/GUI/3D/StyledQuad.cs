/**  --------------------------------------------------------  *
 *   StyledQuad.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using UnityEngine;

namespace Hangout.Client.Gui
{
	/// <summary>
	/// Creates a 3d Quad that textures itself with a IGuiStyle normal texture (including proper NinePartScale)
	/// </summary>
	public class StyledQuad : IDisposable
	{
		private static readonly int[] mTriangles = new System.Int32[]
		{
			1, 0, 4, 1, 4, 5, 2, 1, 5, 2, 5, 6, 3, 2, 6, 3, 6, 7,
			5, 4, 8, 5, 8, 9, 6, 5, 9, 6, 9, 10, 7, 6, 10, 7, 10, 11,
			9, 8, 12, 9, 12, 13, 10, 9, 13, 10, 13, 14, 11, 10, 14, 11, 14, 15
		};

		private readonly GuiMargin mNinePartScale;
		private readonly GameObject mDisplayObject;

		public Transform Transform
		{
			get { return mDisplayObject.transform; }
		}

		private Vector2 mSize;
		public Vector2 Size
		{
			get { return mSize; }
			set 
			{ 
				mSize = value;
				Resize();
			}
		}

		private readonly Vector2[] mUv;

		public StyledQuad(string name, IGuiStyle style)
		{
			mDisplayObject = new GameObject(name);
			mDisplayObject.AddComponent<MeshFilter>();
			Renderer renderer = mDisplayObject.AddComponent<MeshRenderer>();
			renderer.material = new Material(Shader.Find("GUI/Flat Color"));

			Texture2D texture = style.Normal.Background;
			GuiMargin ninePartScale = style.NinePartScale;

			float left = ninePartScale.Left / texture.width;
			float right = 1.0f - (ninePartScale.Right / texture.width);

			float top = ninePartScale.Top / texture.height;
			float bottom = 1.0f - (ninePartScale.Bottom / texture.height);

			float scalar = 1.0f / (float)(texture.width > texture.height ? texture.width : texture.height);
			mNinePartScale = ninePartScale * scalar;
			mSize = new Vector2(texture.width, texture.height) * scalar;

			mUv = new Vector2[16];
			mUv[0] = new Vector2(0.0f, 0.0f);
			mUv[1] = new Vector2(left, 0.0f);
			mUv[2] = new Vector2(right, 0.0f);
			mUv[3] = new Vector2(1.0f, 0.0f);

			mUv[4] = new Vector2(0.0f, top);
			mUv[5] = new Vector2(left, top);
			mUv[6] = new Vector2(right, top);
			mUv[7] = new Vector2(1.0f, top);

			mUv[8] = new Vector2(0.0f, bottom);
			mUv[9] = new Vector2(left, bottom);
			mUv[10] = new Vector2(right, bottom);
			mUv[11] = new Vector2(1.0f, bottom);

			mUv[12] = new Vector2(0.0f, 1.0f);
			mUv[13] = new Vector2(left, 1.0f);
			mUv[14] = new Vector2(right, 1.0f);
			mUv[15] = new Vector2(1.0f, 1.0f);

			renderer.material.mainTexture = texture;

			Resize();
		}

		private void Resize()
		{
			Vector3[] vertices = new Vector3[16];
			vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
			vertices[1] = new Vector3(mNinePartScale.Left, 0.0f, 0.0f);
			vertices[2] = new Vector3(mSize.x - mNinePartScale.Right, 0.0f, 0.0f);
			vertices[3] = new Vector3(mSize.x, 0.0f, 0.0f);

			vertices[4] = new Vector3(0.0f, 0.0f, mNinePartScale.Top);
			vertices[5] = new Vector3(mNinePartScale.Left, 0.0f, mNinePartScale.Top);
			vertices[6] = new Vector3(mSize.x - mNinePartScale.Right, 0.0f, mNinePartScale.Top);
			vertices[7] = new Vector3(mSize.x, 0.0f, mNinePartScale.Top);

			vertices[8] = new Vector3(0.0f, 0.0f, mSize.y - mNinePartScale.Bottom);
			vertices[9] = new Vector3(mNinePartScale.Left, 0.0f, mSize.y - mNinePartScale.Bottom);
			vertices[10] = new Vector3(mSize.x - mNinePartScale.Right, 0.0f, mSize.y - mNinePartScale.Bottom);
			vertices[11] = new Vector3(mSize.x, 0.0f, mSize.y - mNinePartScale.Bottom);

			vertices[12] = new Vector3(0.0f, 0.0f, mSize.y);
			vertices[13] = new Vector3(mNinePartScale.Left, 0.0f, mSize.y);
			vertices[14] = new Vector3(mSize.x - mNinePartScale.Right, 0.0f, mSize.y);
			vertices[15] = new Vector3(mSize.x, 0.0f, mSize.y);

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.uv = mUv;

			mesh.triangles = mTriangles;

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.Optimize(); 
			mDisplayObject.GetComponent<MeshFilter>().mesh = mesh;
		}

		public void Dispose()
		{
			GameObject.Destroy(mDisplayObject);
		}
	}
}