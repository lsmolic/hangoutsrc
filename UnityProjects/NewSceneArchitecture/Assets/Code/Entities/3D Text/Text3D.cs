/*
	Created by Vilas Tewari on 2009-07-20.
   
	Base class for all Scripts that create 3D Text
*/


using UnityEngine;
using System;
using System.Collections;

namespace Hangout.Client
{
	public class Text3D : IDisposable
	{
		// Test Member
		public Font m_font;
	
		private MeshRenderer m_meshRenderer;
		private TextMesh m_textMesh;
	
		private GameObject m_textGameObject;
		private Transform m_textTransform;
	
		/*
			Properties
		*/
		public string DisplayString {
			set { m_textMesh.text = value; }
			get { return m_textMesh.text; }
		}
		public Font DisplayFont {
			set { m_textMesh.font = value; }
			get { return m_textMesh.font; }
		}
		public Vector3 Center {
			get { return Bounds.center; }
			set { SetCenter( value ); }
		}
		public Bounds Bounds {
			get { return m_meshRenderer.bounds; }
		}
		public Vector3 Scale {
			get { return m_textTransform.localScale; }
			set { m_textTransform.localScale = value; }
		}
		public float Transparency {
			get { return m_meshRenderer.material.color.a; }
			set { 
					Color newColor = m_meshRenderer.material.color;
					newColor.a = value;
					m_meshRenderer.material.color = newColor;
				}
		}
		public Color Color {
			get { return m_meshRenderer.material.color; }
			set { m_meshRenderer.material.color = value; }
		}
		public Quaternion Rotation {
			get { return m_textTransform.rotation; }
			set { m_textTransform.rotation = value; }
		}
		public Transform Transform {
			get { return m_textTransform; }
		}
		public bool Visible {
			get { return m_textGameObject.active; }
			set { m_textGameObject.active = value; }
		}
	
		/*
			Constructor
		*/
		public Text3D( Font displayFont ) {
		
			m_textGameObject = new GameObject( "Text3D GameObject" ) as GameObject;
			m_textTransform = m_textGameObject.transform;
			m_textMesh = m_textGameObject.AddComponent( typeof(TextMesh) ) as TextMesh;
			m_meshRenderer = m_textGameObject.AddComponent( typeof(MeshRenderer) ) as MeshRenderer;
		
			/*
				Set Font and Material
			*/
			DisplayFont = displayFont;
			m_meshRenderer.material = displayFont.material;
			m_meshRenderer.material.shader = Shader.Find( "GUI/Text Shader Z" );
		}
	
		/*
			Set the center of the 3D Text
		*/
		private void SetCenter( Vector3 newCenter ) {
			Bounds bounds = Bounds;
			Vector3 center = m_textTransform.position - bounds.center + newCenter;
			m_textTransform.position = center;
		}

        public void Dispose()
        {
            UnityEngine.GameObject.Destroy(m_textGameObject);
        }
	}
}