using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine
{
	public class GUI
	{

		public delegate void WindowFunction(int id);

		// Mock data:
		private Dictionary<string, int> m_functionCallCounts;
		public Dictionary<string, int> FunctionCallCounts
		{
			get
			{
				if (m_functionCallCounts == null)
				{
					m_functionCallCounts = new Dictionary<string, int>();
				}
				return m_functionCallCounts;
			}
		}

		public static void Label(UnityEngine.Rect position, System.String text)
		{
		}

		public static void Label(UnityEngine.Rect position, UnityEngine.Texture image)
		{
		}

		public static void Label(UnityEngine.Rect position, UnityEngine.GUIContent content)
		{
		}

		public static void Label(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
		}

		public static void Label(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
		}

		public static void Label(UnityEngine.Rect position, UnityEngine.GUIContent content, UnityEngine.GUIStyle style)
		{
		}

		public static void DrawTexture(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.ScaleMode scaleMode, System.Boolean alphaBlend)
		{
		}

		public static void DrawTexture(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.ScaleMode scaleMode)
		{
		}

		public static void DrawTexture(UnityEngine.Rect position, UnityEngine.Texture image)
		{
		}

		public static void DrawTexture(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.ScaleMode scaleMode, System.Boolean alphaBlend, System.Single imageAspect)
		{
		}

		public static void Box(UnityEngine.Rect position, System.String text)
		{
		}

		public static void Box(UnityEngine.Rect position, UnityEngine.Texture image)
		{
		}

		public static void Box(UnityEngine.Rect position, UnityEngine.GUIContent content)
		{
		}

		public static void Box(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
		}

		public static void Box(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
		}

		public static void Box(UnityEngine.Rect position, UnityEngine.GUIContent content, UnityEngine.GUIStyle style)
		{
		}

		public static System.Boolean Button(UnityEngine.Rect position, System.String text)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Button(UnityEngine.Rect position, UnityEngine.Texture image)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Button(UnityEngine.Rect position, UnityEngine.GUIContent content)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Button(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Button(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Button(UnityEngine.Rect position, UnityEngine.GUIContent content, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean RepeatButton(UnityEngine.Rect position, System.String text)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean RepeatButton(UnityEngine.Rect position, UnityEngine.Texture image)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean RepeatButton(UnityEngine.Rect position, UnityEngine.GUIContent content)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean RepeatButton(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean RepeatButton(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean RepeatButton(UnityEngine.Rect position, UnityEngine.GUIContent content, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextField(UnityEngine.Rect position, System.String text)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextField(UnityEngine.Rect position, System.String text, System.Int32 maxLength)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextField(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextField(UnityEngine.Rect position, System.String text, System.Int32 maxLength, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String PasswordField(UnityEngine.Rect position, System.String password, System.Char maskChar)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String PasswordField(UnityEngine.Rect position, System.String password, System.Char maskChar, System.Int32 maxLength)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String PasswordField(UnityEngine.Rect position, System.String password, System.Char maskChar, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String PasswordField(UnityEngine.Rect position, System.String password, System.Char maskChar, System.Int32 maxLength, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String PasswordFieldGetStrToShow(System.String password, System.Char maskChar)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextArea(UnityEngine.Rect position, System.String text)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextArea(UnityEngine.Rect position, System.String text, System.Int32 maxLength)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextArea(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.String TextArea(UnityEngine.Rect position, System.String text, System.Int32 maxLength, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void DoTextField(UnityEngine.Rect position, System.Int32 id, UnityEngine.GUIContent content, System.Boolean multiline, System.Int32 maxLength, UnityEngine.GUIStyle style)
		{
		}

		public static void SetNextControlName(System.String name)
		{
		}

		public static System.String GetNameOfFocusedControl()
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void FocusControl(System.String name)
		{
		}

		public static System.Boolean Toggle(UnityEngine.Rect position, System.Boolean value, System.String text)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Toggle(UnityEngine.Rect position, System.Boolean value, UnityEngine.Texture image)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Toggle(UnityEngine.Rect position, System.Boolean value, UnityEngine.GUIContent content)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Toggle(UnityEngine.Rect position, System.Boolean value, System.String text, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Toggle(UnityEngine.Rect position, System.Boolean value, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Boolean Toggle(UnityEngine.Rect position, System.Boolean value, UnityEngine.GUIContent content, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 Toolbar(UnityEngine.Rect position, System.Int32 selected, System.String[] texts)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 Toolbar(UnityEngine.Rect position, System.Int32 selected, UnityEngine.Texture[] images)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 Toolbar(UnityEngine.Rect position, System.Int32 selected, UnityEngine.GUIContent[] content)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 Toolbar(UnityEngine.Rect position, System.Int32 selected, System.String[] texts, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 Toolbar(UnityEngine.Rect position, System.Int32 selected, UnityEngine.Texture[] images, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 Toolbar(UnityEngine.Rect position, System.Int32 selected, UnityEngine.GUIContent[] contents, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 SelectionGrid(UnityEngine.Rect position, System.Int32 selected, System.String[] texts, System.Int32 xCount)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 SelectionGrid(UnityEngine.Rect position, System.Int32 selected, UnityEngine.Texture[] images, System.Int32 xCount)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 SelectionGrid(UnityEngine.Rect position, System.Int32 selected, UnityEngine.GUIContent[] content, System.Int32 xCount)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 SelectionGrid(UnityEngine.Rect position, System.Int32 selected, System.String[] texts, System.Int32 xCount, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 SelectionGrid(UnityEngine.Rect position, System.Int32 selected, UnityEngine.Texture[] images, System.Int32 xCount, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Int32 SelectionGrid(UnityEngine.Rect position, System.Int32 selected, UnityEngine.GUIContent[] contents, System.Int32 xCount, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single HorizontalSlider(UnityEngine.Rect position, System.Single value, System.Single leftValue, System.Single rightValue)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single HorizontalSlider(UnityEngine.Rect position, System.Single value, System.Single leftValue, System.Single rightValue, UnityEngine.GUIStyle slider, UnityEngine.GUIStyle thumb)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single VerticalSlider(UnityEngine.Rect position, System.Single value, System.Single topValue, System.Single bottomValue)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single VerticalSlider(UnityEngine.Rect position, System.Single value, System.Single topValue, System.Single bottomValue, UnityEngine.GUIStyle slider, UnityEngine.GUIStyle thumb)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single Slider(UnityEngine.Rect position, System.Single value, System.Single size, System.Single start, System.Single end, UnityEngine.GUIStyle slider, UnityEngine.GUIStyle thumb, System.Boolean horiz, System.Int32 id)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single HorizontalScrollbar(UnityEngine.Rect position, System.Single value, System.Single size, System.Single leftValue, System.Single rightValue)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single HorizontalScrollbar(UnityEngine.Rect position, System.Single value, System.Single size, System.Single leftValue, System.Single rightValue, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single VerticalScrollbar(UnityEngine.Rect position, System.Single value, System.Single size, System.Single topValue, System.Single bottomValue)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static System.Single VerticalScrollbar(UnityEngine.Rect position, System.Single value, System.Single size, System.Single topValue, System.Single bottomValue, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void BeginGroup(UnityEngine.Rect position)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, System.String text)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, UnityEngine.Texture image)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, UnityEngine.GUIContent content)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, UnityEngine.GUIStyle style)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, System.String text, UnityEngine.GUIStyle style)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
		}

		public static void BeginGroup(UnityEngine.Rect position, UnityEngine.GUIContent content, UnityEngine.GUIStyle style)
		{
		}

		public static void EndGroup()
		{
		}

		public static UnityEngine.Vector2 BeginScrollView(UnityEngine.Rect position, UnityEngine.Vector2 scrollPosition, UnityEngine.Rect viewRect)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Vector2 BeginScrollView(UnityEngine.Rect position, UnityEngine.Vector2 scrollPosition, UnityEngine.Rect viewRect, System.Boolean alwaysShowHorizontal, System.Boolean alwaysShowVertical)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Vector2 BeginScrollView(UnityEngine.Rect position, UnityEngine.Vector2 scrollPosition, UnityEngine.Rect viewRect, UnityEngine.GUIStyle horizontalScrollbar, UnityEngine.GUIStyle verticalScrollbar)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Vector2 BeginScrollView(UnityEngine.Rect position, UnityEngine.Vector2 scrollPosition, UnityEngine.Rect viewRect, System.Boolean alwaysShowHorizontal, System.Boolean alwaysShowVertical, UnityEngine.GUIStyle horizontalScrollbar, UnityEngine.GUIStyle verticalScrollbar)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void EndScrollView()
		{
		}

		public static void ScrollTo(UnityEngine.Rect position)
		{
		}

		public static UnityEngine.Rect Window(System.Int32 id, UnityEngine.Rect position, UnityEngine.GUI.WindowFunction func, System.String text)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Rect Window(System.Int32 id, UnityEngine.Rect position, UnityEngine.GUI.WindowFunction func, UnityEngine.Texture image)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Rect Window(System.Int32 id, UnityEngine.Rect position, UnityEngine.GUI.WindowFunction func, UnityEngine.GUIContent content)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Rect Window(System.Int32 id, UnityEngine.Rect position, UnityEngine.GUI.WindowFunction func, System.String text, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Rect Window(System.Int32 id, UnityEngine.Rect position, UnityEngine.GUI.WindowFunction func, UnityEngine.Texture image, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static UnityEngine.Rect Window(System.Int32 id, UnityEngine.Rect clientRect, UnityEngine.GUI.WindowFunction func, UnityEngine.GUIContent title, UnityEngine.GUIStyle style)
		{
			throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
		}

		public static void DragWindow(UnityEngine.Rect position)
		{
		}

		public static void DragWindow()
		{
		}

		public static void BringWindowToFront(System.Int32 windowID)
		{
		}

		public static void BringWindowToBack(System.Int32 windowID)
		{
		}

		public static void FocusWindow(System.Int32 windowID)
		{
		}

		public static void UnfocusWindow()
		{
		}

		public GUI()
		{
			//Mock Data:
			m_functionCallCounts = new Dictionary<string, int>();
			if (!m_functionCallCounts.ContainsKey("Void .ctor()"))
			{
				m_functionCallCounts.Add("Void .ctor()", 0);
			}
			m_functionCallCounts["Void .ctor()"]++;

		}

		public static System.Boolean changed
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static UnityEngine.Color backgroundColor
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static System.String tooltip
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static UnityEngine.Color color
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static UnityEngine.Matrix4x4 matrix
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static System.Boolean enabled
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static GUISkin skin
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static UnityEngine.Color contentColor
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}

		public static System.Int32 depth
		{
			get
			{
				throw new NotImplementedException("This function was automatically generated by Mockery and has no real implementation yet.");
			}
			set
			{
			}
		}
	}
}
