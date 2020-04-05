/**  --------------------------------------------------------  *
 *   GridView.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/12/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client.Gui
{
	public class GridView : GuiFrame, IPagination
	{
		private float mGridElementHeight = 0.0f;
		private float mGridElementWidth = 0.0f;
		private int mRows = 0;
		public int Rows
		{
			get { return mRows; }
		}
		private int mColumns = 0;
		public int Columns
		{
			get { return mColumns; }
		}
		private int mCurrentPage = 0;
		public int CurrentPage
		{
			get { return mCurrentPage; }
		}
		private List<IWidget> mGridWidgets = new List<IWidget>();
		private readonly List<KeyValuePair<IWidget, IGuiPosition>> mActiveWidgets = new List<KeyValuePair<IWidget, IGuiPosition>>();
		public List<KeyValuePair<IWidget, IGuiPosition>> ActiveWidgets
		{
			get { return mActiveWidgets; }
		}

		public GridView(string name, IGuiSize size, IEnumerable<KeyValuePair<IWidget, IGuiPosition>> widgets, IGuiStyle style )
			: base(name, size, widgets, style)
		{
		}

		public void SetPositions(IEnumerable<IWidget> widgets, int rows, int columns, int startingIndex)
		{
			if (widgets == null)
			{
				throw new ArgumentNullException("widgets");
			}

			mGridWidgets = new List<IWidget>(widgets);

			if (mGridWidgets.Count == 0)
			{
				return;
			}	
			
			if ( rows < 1 || columns < 1 )
			{
				throw new ArgumentOutOfRangeException("Rows(" + rows + ") and columns(" + columns + ") must be greater than 0.");
			}

			if (mGridWidgets.Count < 1)
			{
				throw new ArgumentException("mGridWidgets is empty.");
			}

			if (startingIndex < 0 || startingIndex > mGridWidgets.Count)
			{
				throw new ArgumentOutOfRangeException("startingIndex(" + startingIndex + ") must be positive and less than mGridWidgets.Count(" + mGridWidgets.Count + ".");
			}

			if (columns > 1)
			{
				mGridElementWidth = (this.InternalSize.x) / columns;
			}
			else
			{
				mGridElementWidth = (this.InternalSize.x);
			}

			if (rows > 1)
			{
				mGridElementHeight = (this.InternalSize.y) / rows;
			}
			else
			{
				mGridElementHeight = (this.InternalSize.y);
			}
			
			mRows = rows;
			mColumns = columns;
			Vector2 gridPosition = Vector2.zero;
			int endIndex = startingIndex + (rows * columns);
			if (endIndex > mGridWidgets.Count)
			{
				endIndex = mGridWidgets.Count;
			}
			mActiveWidgets.Clear();
			
			int i = 0;
			for (int index = startingIndex; index < endIndex; ++index)
			{
				float xPosition = (i % columns) * mGridElementWidth;
				float yPosition = (i / columns) * mGridElementHeight;
				gridPosition = new Vector2(xPosition, yPosition);
				IGuiPosition elementPosition = new FixedPosition(gridPosition);
				KeyValuePair<IWidget, IGuiPosition> elementPair = new KeyValuePair<IWidget, IGuiPosition>(mGridWidgets[index], elementPosition);
				mActiveWidgets.Add(elementPair);
				i++;
			}
			SetCurrentPage(startingIndex);
		}

		public void SetPositionsWithBorderPadding(IEnumerable<IWidget> widgets, int rows, int columns, int startingIndex, int horizontalFramePadding, int verticalFramePadding, int innerButtonPadding)
		{
			if (widgets == null)
			{
				throw new ArgumentNullException("widgets");
			}

			mGridWidgets = new List<IWidget>(widgets);

			if (mGridWidgets.Count == 0)
			{
				return;
			}

			if (rows < 1 || columns < 1)
			{
				throw new ArgumentOutOfRangeException("Rows(" + rows + ") and columns(" + columns + ") must be greater than 0.");
			}

			if (mGridWidgets.Count < 1)
			{
				throw new ArgumentException("mGridWidgets is empty.");
			}

			if (startingIndex < 0 || startingIndex > mGridWidgets.Count)
			{
				throw new ArgumentOutOfRangeException("startingIndex(" + startingIndex + ") must be positive and less than mGridWidgets.Count(" + mGridWidgets.Count + ".");
			}

			mRows = rows;
			mColumns = columns;
			Vector2 gridPosition = Vector2.zero;
			int endIndex = startingIndex + (rows * columns);
			if (endIndex > mGridWidgets.Count)
			{
				endIndex = mGridWidgets.Count;
			}
			mActiveWidgets.Clear();

			int i = 0;
			for (int index = startingIndex; index < endIndex; ++index)
			{
				IWidget widget = mGridWidgets[index];
				float xPosition = (i % columns) * (widget.Size.x + innerButtonPadding) + horizontalFramePadding;
				float yPosition = (i / columns) * (widget.Size.y + innerButtonPadding) + verticalFramePadding;
				gridPosition = new Vector2(xPosition, yPosition);
				IGuiPosition elementPosition = new FixedPosition(gridPosition);
				KeyValuePair<IWidget, IGuiPosition> elementPair = new KeyValuePair<IWidget, IGuiPosition>(mGridWidgets[index], elementPosition);
				mActiveWidgets.Add(elementPair);
				i++;
			}
			SetCurrentPage(startingIndex);
		}

		public override void Draw(IGuiContainer container, Vector2 position)
		{
			Rect coords = new Rect(position.x, position.y, this.Size.x, this.Size.y);
			AutoLayoutUpdate();

			if (this.Style != null)
			{
				GUI.BeginGroup(coords, this.Style.GenerateUnityGuiStyle());
			}
			else
			{
				GUI.BeginGroup(coords);
			}

			try
			{
				//Draw the widgets in the GuiFrame.
				foreach (KeyValuePair<IWidget, IGuiPosition> widget in Widgets)
				{
					if (widget.Key.Showing)
					{
						Vector2 widgetPosition = widget.Value.GetPosition(widget.Key);
						widget.Key.Draw(this, widgetPosition);
					}
				}

				//Draw only the active widgets in the grid.
				foreach (KeyValuePair<IWidget, IGuiPosition> widget in mActiveWidgets)
				{
					if (widget.Key.Showing)
					{
						Vector2 widgetPosition = widget.Value.GetPosition(widget.Key);
						widget.Key.Draw(this, widgetPosition);
					}
				}

			}
			finally
			{
				GUI.EndGroup();
			}
		}

        //TODO: was this intended to be a new implementation of this? 
        //this class inherits from another that has "GetChildPosition"
		public new Vector2 GetChildPosition(IGuiElement childElement)
		{
			foreach (KeyValuePair<IWidget, IGuiPosition> child in mActiveWidgets)
			{
				if (child.Key == childElement)
				{
					return child.Value.GetPosition(childElement);
				}
			}
			throw new ArgumentException("Child Element not found in children.");
		}
		
		public void SetPagination(int itemsPerPage, int pageNumber)
		{	
			//Items per page is determined by the rows and columns.  So skip that bologna and use this func.
			SetPage(pageNumber);
			mCurrentPage = pageNumber + 1;
		}
		
		private void SetPage(int pageNumber)
		{
			mCurrentPage = pageNumber;
			int startingIndex = mRows * mColumns * pageNumber;
			SetPositions(mGridWidgets, mRows, mColumns, startingIndex);
		}
		
		//This is an internal function used after the positions are set to make sure the current page is up to date.
		private void SetCurrentPage(int currentStartingIndex)
		{
			mCurrentPage = (currentStartingIndex / (mRows * mColumns)) + 1;
		}
		
		public void NextPage()
		{
			if (mCurrentPage != GetTotalPages())
			{
				++mCurrentPage;
				//The math is based on currentPage - 1, however, it doesn't make sense that you would be on page 0.
				//So I use the logical visual concept for mCurrentPage and subtract one everywhere for the math. 
				//It makes it easier for the Gui Controller.
				int startingIndex = mRows * mColumns * (mCurrentPage - 1);
				SetPositions(mGridWidgets, mRows, mColumns, startingIndex);
			}
		}
		
		public void PreviousPage()
		{
			if (mCurrentPage - 1 > 0)
			{
				--mCurrentPage;
				//The math is based on currentPage - 1, however, it doesn't make sense that you would be on page 0.
				//So I use the logical visual concept for mCurrentPage and subtract one everywhere for the math. 
				//It makes it easier for the Gui Controller.
				int startingIndex = mRows * mColumns * (mCurrentPage - 1);
				SetPositions(mGridWidgets, mRows, mColumns, startingIndex);
			}
		}
		
		public void LastPage()
		{
			mCurrentPage = GetTotalPages();
			//The math is based on currentPage - 1, however, it doesn't make sense that you would be on page 0.
			//So I use the logical visual concept for mCurrentPage and subtract one everywhere for the math. 
			//It makes it easier for the Gui Controller.
			int startingIndex = mRows * mColumns * (mCurrentPage - 1);
			SetPositions(mGridWidgets, mRows, mColumns, startingIndex);
		}
		
		public void FirstPage()
		{
			int startingIndex = 0;
			SetPositions(mGridWidgets, mRows, mColumns, startingIndex);
		}
		
		public int GetTotalPages()
		{
			if ( mRows == 0 || mColumns == 0)
			{
				return 0;
			}
			//The math is based on currentPage - 1, however, it doesn't make sense that you would ever be on page 0.
			//So I use the logical visual concept for mCurrentPage and add one when calculating the currentPage. 
			//It makes it easier for the Gui Controller code.
			int totalPages = mGridWidgets.Count / (mRows * mColumns);
			if (mGridWidgets.Count % (mRows * mColumns) != 0)
			{
				++totalPages;
			}
			return totalPages;
		}
	}
}
