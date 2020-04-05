/**  --------------------------------------------------------  *
 *   HorizontalAutoLayout.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/03/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class HorizontalAutoLayout : IAutoLayout
	{
		private IDictionary<IGuiElement, Vector2> mCachedPositions = new Dictionary<IGuiElement, Vector2>();

		public void UpdatePosition(IGuiElement element, Vector2 newPosition)
		{
			throw new Exception("AutoLayout widgets cannot be moved.");
		}

		public Vector2 GetPosition(IGuiElement element)
		{
			if (!mCachedPositions.ContainsKey(element))
			{
				throw new Exception("NextPosition was never assigned for this element (" + element.Name + ")");
			}

			return mCachedPositions[element];
		}

		/// Valid location is defined as an attemptedReservation that doesn't overlap any reservedSpaces
		private static bool IsValidLocation(IEnumerable<Rect> reservedSpaces, Rect attemptedReservation)
		{
			foreach (Rect reservedSpace in reservedSpaces)
			{
				if (RectUtility.Overlaps(attemptedReservation, reservedSpace))
				{
					return false;
				}
			}
			return true;
		}

		// This will need to be implemented with a Quad-tree (or some analog) if
		//  this naive, O(n-squared) search implementation turns out to be too slow.
		public Rect NextPosition(IWidget widget, IEnumerable<Rect> reservedSpaces, Vector2 parentSize, IList<KeyValuePair<IWidget, IAutoLayout>> autolayoutWidgets)
		{
			
			Rect attemptedReservation = new Rect(0.0f, 0.0f, widget.ExternalSize.x, widget.ExternalSize.y);
			List<Rect> validLocations = new List<Rect>();

			// Best case is if this widget fits in the initial guess location (top left)
			if (IsValidLocation(reservedSpaces, attemptedReservation))
			{
				validLocations.Add(attemptedReservation);
			}

			// If widget doesn't fit in the initial guess, try to place it to the
			//  right of any reserved space.
			if (validLocations.Count == 0)
			{
				Rect parentSpace = new Rect(0.0f, 0.0f, parentSize.x, parentSize.y);
				foreach (Rect reservedSpace in reservedSpaces)
				{
					attemptedReservation = new Rect(reservedSpace.x + reservedSpace.width,
													reservedSpace.y,
													widget.ExternalSize.x,
													widget.ExternalSize.y);
					if (RectUtility.Contains(parentSpace, attemptedReservation) &&
						IsValidLocation(reservedSpaces, attemptedReservation))
					{ // n-squared
						validLocations.Add(attemptedReservation);
					}
				}
			}

			// If the widget didn't fit to the right of any reserved space, try under them
			// This search doesn't check against the parent space, so the reserved space can 
			//  go off the bottom of that space
			if (validLocations.Count == 0)
			{
				foreach (Rect reservedSpace in reservedSpaces)
				{
					attemptedReservation = new Rect(reservedSpace.x,
													reservedSpace.y + reservedSpace.height,
													widget.ExternalSize.x,
													widget.ExternalSize.y);
					if (IsValidLocation(reservedSpaces, attemptedReservation))
					{ // n-squared
						validLocations.Add(attemptedReservation);
					}
				}
			}

			// Find the best validLocation: Highest on Y and then furthest to the left.
			List<Rect> bestOnY = new List<Rect>();
			float lowestYValue = Mathf.Infinity;
			foreach (Rect validLocation in validLocations)
			{
				if (validLocation.y < lowestYValue)
				{
					lowestYValue = validLocation.y;
				}
			}

			foreach (Rect validLocation in validLocations)
			{
				if (validLocation.y == lowestYValue)
				{
					bestOnY.Add(validLocation);
				}
			}

			// Out of the valid locations that are best on Y, find the one that's furthest to the left
			float lowestXValue = Mathf.Infinity;
			foreach (Rect validLocation in bestOnY)
			{
				if (validLocation.x < lowestXValue)
				{
					lowestXValue = validLocation.x;
					attemptedReservation = validLocation;
				}
			}

			mCachedPositions[widget] = new Vector2(attemptedReservation.x, attemptedReservation.y);
			if (widget.Style != null)
			{
				mCachedPositions[widget] += new Vector2(widget.Style.ExternalMargins.Left, widget.Style.ExternalMargins.Top);
			}
			return attemptedReservation;
		}
	}
}
