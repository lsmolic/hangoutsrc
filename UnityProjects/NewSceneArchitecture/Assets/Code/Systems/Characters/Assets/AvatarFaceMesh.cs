/*
   Created by Vilas Tewari on 2009-08-18.

	This class handles merging, Rigging and skinning of the Avatar's meshes ( cuts )
*/

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class AvatarFaceMesh : AvatarMesh
	{
		public Vector2[] mGridSize; // Grid sizes of various UV shells indexed by material index
		private Vector2[] mCellSize; // Cellsizes of various UV grids indexed by material index 
		private int[] mNumberOfCells; // Number of cells sizes of various UV Grids indexed by material index
		
		private int[][] mFrameSequence; // Array of frameSequences indexed by material index
		private int[] mCurrentFrames; // Array of frameIndex being played, indexed by material index. -1 means no frame is playing
		
		private Dictionary<string, int> mUvShellNameToIndex;
		
		private UvShell[] mUvShells;
		private Mesh mMesh;
		private Vector2[] mNewUvs;
		
		private bool mUvsAreDirty;
		
		private MeshFilter mMeshFilter;
		private MeshRenderer mMeshRenderer;
		
		private bool mIsPlaying;
		
		private float mLastUpdateTime = 0;
		private float mUpdateTimeInterval = 0.033f; //seconds between update
		
		/*
			Properties
		*/
		public bool IsPlaying
		{
			get{ return mIsPlaying; }
		}
		public int UvShellCount
		{
			get{ return mUvShells.Length; }
		}
		
		public AvatarFaceMesh( AvatarTexture faceTexture, Transform headTransform )
			: base( faceTexture, headTransform )
		{
			mMeshFilter = headTransform.gameObject.GetComponent( typeof( MeshFilter )) as MeshFilter;
			mMeshRenderer = headTransform.gameObject.GetComponent( typeof( MeshRenderer )) as MeshRenderer;
			
			mUvShells = new UvShell[0];
			mCurrentFrames = new int[0];
			mFrameSequence = new int[0][];
			mGridSize = new Vector2[0];
			mCellSize = new Vector2[0];
			mNumberOfCells = new int[0];
			
			mUvShellNameToIndex = new Dictionary<string, int>();
			mUvShells = new UvShell[0];
			
			mIsPlaying = false;
		}
		
		public void SetMesh( Asset asset )
		{
			FaceMeshAsset avatarFace = asset as FaceMeshAsset;
			if (avatarFace == null)
			{
				throw new Exception("Could not convert asset from Repo to FaceMeshAsset.");
			}
			
			mMesh = avatarFace.Mesh;
			mUvShellNameToIndex.Clear();
			
			/*
				Create a list of UV shells ( A submesh maps to one UV shell )
				and populate the corresponding frameSequence and currentFrame arrays
			*/	
			int uvShellCount = avatarFace.UvShellCount;
			mUvShells = new UvShell[uvShellCount];
			mCurrentFrames = new int[uvShellCount];
			mFrameSequence = new int[uvShellCount][];
			mGridSize = new Vector2[uvShellCount];
			mCellSize = new Vector2[uvShellCount];
			mNumberOfCells = new int[uvShellCount];
			for( int x = 0; x < uvShellCount; x++ )
			{
				mUvShells[x] = new UvShell( mMesh, x );
				mCurrentFrames[x] = -1; // OFF
				mGridSize[x] = avatarFace.GetUvGridDimensions(x);
				mCellSize[x] = new Vector2 (1.0f / mGridSize[x].x, 1.0f / mGridSize[x].y);
				mNumberOfCells[x] = (int) ( mGridSize[x].x * mGridSize[x].y );
				mUvShellNameToIndex.Add( avatarFace.GetUvShellName(x), x );
			}
			
			/*
				Combine all submeshes into one mesh ( i.e. create a new mesh )
			*/
			Mesh newMesh = new Mesh();
			newMesh.vertices = mMesh.vertices;
			newMesh.triangles = mMesh.triangles;
			newMesh.uv = mMesh.uv;
			newMesh.normals = mMesh.normals;
			newMesh.RecalculateBounds();
			mMesh = newMesh;
			mMeshFilter.mesh = newMesh;
			
			/*
				Set the atlas space for the UV shells
			*/
			for( int x = 0; x < mUvShells.Length; ++x )
				mUvShells[x].Atlas = mTexturePalette.GetTextureZoneUvArea( avatarFace.GetUvShellTextureZoneName(x));
			
			/* Update Uvs */
			mNewUvs = mMesh.uv;
			for( int x = 0; x < mUvShells.Length; ++x )
				UpdateUvShell( mUvShells[x] );
			mMesh.uv = mNewUvs;
			mUvsAreDirty = false;
			
			/* Update Material */
			Material avatarMaterial = AvatarEntity.AvatarMaterial;
			avatarMaterial.mainTexture = mMeshRenderer.materials[0].mainTexture;
			mMeshRenderer.materials = new Material[]{ avatarMaterial };
		}
		
		/*
			Set Uv Shell properties
		*/
		public void SetUvShellOffset( Vector2 offset, int index )
		{
			mUvShells[index].Offset = offset;
			UpdateUvShell( mUvShells[index] );
		}
		public void SetUvShellRotation( float rotation, int index )
		{
			mUvShells[index].Rotation = rotation;
			UpdateUvShell( mUvShells[index] );
		}
		public void SetUvShellScale( Vector2 scale, int index )
		{
			mUvShells[index].Scale = scale;
			UpdateUvShell( mUvShells[index] );
		}
		
		/*
			Get UvShell properties
		*/
		public Vector2 GetUvShellOffset( int index )
		{
			return mUvShells[index].Offset;
		}
		public float GetUvShellRotation( int index )
		{
			return mUvShells[index].Rotation;
		}
		public Vector2 GetUvShellScale( int index )
		{
			return mUvShells[index].Scale;
		}

		public override void Dispose()
		{
			base.Dispose();
			Mesh.Destroy(mMesh);
		}
		
		/*
			Control Animations
		*/
		public void Play( string uvShellName, int[] sequence )
		{
			if (sequence == null)
			{
				throw new ArgumentNullException("sequence");
			}
			int uvShellIndex = GetUvShellIndex( uvShellName );
			if(IndexIsValid(uvShellIndex))
			{
				string sequenceString = "";
				mFrameSequence[uvShellIndex] = sequence;
				foreach(int i in sequence)
				{
					sequenceString += i + ", ";
				}
				
				mCurrentFrames[uvShellIndex] = 0;	
			}
		}
		public void Stop( int uvShellIndex )
		{
			if(IndexIsValid(uvShellIndex))
				mCurrentFrames[uvShellIndex] = -1;
		}
		
		public bool IndexIsValid( int x )
		{
			return ( x >= 0 && x < mUvShells.Length );
		}
		
		public int GetUvShellIndex( string shellName )
		{
			if( mUvShellNameToIndex.ContainsKey( shellName ))
				return mUvShellNameToIndex[shellName];
			else
				return -1;
		}
		
			
		/// <summary>
		/// Call this function to Actually Update the Uvs on the Mesh Object
		/// </summary>
		public void UpdateUvs()
		{
			if( Time.time - mLastUpdateTime > mUpdateTimeInterval )
			{
				mLastUpdateTime = Time.time;
				mIsPlaying = false;
				
				for( int x = 0; x < mUvShells.Length; ++x )
				{
					// If this shell's frame sequence is still playing
					if( mCurrentFrames[x] != -1 ) 
					{
						// And the frame we are on is within the frame sequence
						if (mCurrentFrames[x] < mFrameSequence[x].Length)
						{
							SetUvShellAtFrame(x, mFrameSequence[x][mCurrentFrames[x]]);
							mIsPlaying = true;
							++mCurrentFrames[x];
						}
						else // end playing sequence
						{
							mCurrentFrames[x] = -1;
						}
					}
				}
				if( mUvsAreDirty )
				{
					mMesh.uv = mNewUvs;
					mUvsAreDirty = false;
				}
			}
		}
		
		/*
			Move a UvShell at a frame location based on the Uvgrid
		*/
		private void SetUvShellAtFrame( int shellIndex, int frameNumber )
		{
			int playFrame = frameNumber % mNumberOfCells[shellIndex];
			int gridWidth = (int) mGridSize[shellIndex].x;
			int uIndex = playFrame % gridWidth;
			int vIndex = playFrame / gridWidth;
			Vector2 cellSize = mCellSize[shellIndex];

			Vector2 newShellPosition = new Vector2( uIndex * cellSize.x, vIndex * cellSize.y );
			mUvShells[shellIndex].Position = newShellPosition;
			UpdateUvShell( mUvShells[shellIndex] );
		}
		
		/*
			Update a UvShell Object and put the result in mNewUvs
		*/
		private void UpdateUvShell( UvShell shell )
		{
			shell.UpdateUvs( mNewUvs );
			mUvsAreDirty = true;
		}
	}
}