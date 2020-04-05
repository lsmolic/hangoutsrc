/**  --------------------------------------------------------  *
 *   DebugUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/01/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Hangout.Client
{
	public static class DebugUtility
	{
		/// <summary>
		/// Make sure this function is called OnGUI
		/// </summary>
		public static void PrintErrorToScreen(string error)
		{
			Color originalColor = GUI.skin.label.normal.textColor;

			GUI.skin.label.wordWrap = true;

			// Shadow
			GUI.skin.label.normal.textColor = Color.Lerp(Color.black, Color.white, 0.125f);
			GUILayout.BeginArea(new Rect(0.0f, 0.0f, Screen.width, Screen.height));
			try
			{
				GUILayout.Space(1.0f);
				GUILayout.BeginHorizontal();
				GUILayout.Space(1.0f);
				GUILayout.Label(error);
				GUILayout.EndHorizontal();
			}
			finally
			{
				GUILayout.EndArea();
			}

			// Text
			GUI.skin.label.normal.textColor = Color.Lerp(Color.red, Color.white, 0.25f);
			GUILayout.BeginArea(new Rect(0.0f, 0.0f, Screen.width, Screen.height));
			try
			{
				GUILayout.Label(error);
			}
			finally
			{
				GUILayout.EndArea();
			}

			GUI.skin.label.normal.textColor = originalColor;
		}

		private const uint CIRCLE_SEGMENTS = 32;
		public static void DrawCicrleXZ(Vector3 position, float radius, Color color)
		{
			float pi2 = 2.0f * Mathf.PI;
			float step = pi2 / (float)CIRCLE_SEGMENTS;

			for(float t = 0.0f; t <= pi2; t += step)
			{
				Vector3 a = new Vector3(Mathf.Cos(t), 0.0f, Mathf.Sin(t)) * radius;
				Vector3 b = new Vector3(Mathf.Cos(t - step), 0.0f, Mathf.Sin(t - step)) * radius;

				a += position;
				b += position;

				Debug.DrawLine(a, b, color);
			}
		}
	}
}
