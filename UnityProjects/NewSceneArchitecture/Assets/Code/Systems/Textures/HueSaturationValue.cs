/*
   Created by Vilas Tewari
   A Utility class for Hue Saturation Value Operations
*/


using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public static class HueSaturationValue
	{
		public static HSVA ColorToHSV(Color source)
		{
			return ColorToHSV(source.r, source.g, source.b, source.a);
		}

		public static HSVA ColorToHSV(HColor source)
		{
			return ColorToHSV((float)source.r / 255, (float)source.g / 255, (float)source.b / 255, (float)source.a / 255);
		}

		public static HSVA ColorToHSV(float r, float g, float b, float a)
		{
			HSVA hsv = new HSVA(0, 0, 0, 0);
			hsv.a = a;

			float maxc = Mathf.Max(r, Mathf.Max(g, b));
			float minc = Mathf.Min(r, Mathf.Min(g, b));

			// Set Hue
			if (maxc == minc)
			{
				hsv.h = 0;
			}
			else if (r == maxc)
			{
				hsv.h = 60f * ((g - b) / (maxc - minc)) % 360f;
			}
			else if (g == maxc)
			{
				hsv.h = 60f * ((b - r) / (maxc - minc)) + 120f;
			}
			else if (b == maxc)
			{
				hsv.h = 60f * ((r - g) / (maxc - minc)) + 240f;
			}

			if (hsv.h < 0)
				hsv.h = 360 + hsv.h;

			// Set Value
			hsv.v = maxc;

			// Set Saturation
			if (maxc == 0)
			{
				hsv.s = 0;
			}
			else
			{
				hsv.s = 1f - (minc / maxc);
			}
			return hsv;
		}

		public static Color HSVToColor(HSVA hsv)
		{
			float h = hsv.h;
			float s = hsv.s;
			float v = hsv.v;

			int hi = Mathf.FloorToInt(h / 60f) % 6;
			float f = (h / 60f) - Mathf.Floor(h / 60f);
			float p = v * (1f - s);
			float q = v * (1f - (f * s));
			float t = v * (1f - ((1f - f) * s));

			switch (hi)
			{
				case 0:
					return new Color(v, t, p, hsv.a);
				case 1:
					return new Color(q, v, p, hsv.a);
				case 2:
					return new Color(p, v, t, hsv.a);
				case 3:
					return new Color(p, q, v, hsv.a);
				case 4:
					return new Color(t, p, v, hsv.a);
				case 5:
					return new Color(v, p, q, hsv.a);
				default:
					Console.LogError("HSVToColor: error calculating Color " + hi);
					return new Color(v, t, p, hsv.a);
			}
		}

		public static HColor HSVToHColor(HSVA hsv)
		{
			float h = hsv.h;
			float s = hsv.s;
			float v = hsv.v;

			int hi = Mathf.FloorToInt(h / 60f) % 6;
			float f = (h / 60f) - Mathf.Floor(h / 60f);
			float p = v * (1f - s);
			float q = v * (1f - (f * s));
			float t = v * (1f - ((1f - f) * s));

			switch (hi)
			{
				case 0:
					return new HColor(v, t, p, hsv.a);
				case 1:
					return new HColor(q, v, p, hsv.a);
				case 2:
					return new HColor(p, v, t, hsv.a);
				case 3:
					return new HColor(p, q, v, hsv.a);
				case 4:
					return new HColor(t, p, v, hsv.a);
				case 5:
					return new HColor(v, p, q, hsv.a);
				default:
					Console.LogError("HSVToColor: error calculating Color " + hi);
					return new HColor(v, t, p, hsv.a);
			}
		}
	}
}