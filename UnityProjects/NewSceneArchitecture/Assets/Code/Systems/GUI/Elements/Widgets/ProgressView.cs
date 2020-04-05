/**  --------------------------------------------------------  *
 *   ProgressView.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/19/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client.Gui
{
	// TODO: Allow this class to be created from XML. Right now it's only
	//  available to be created procedurally.
	public class ProgressView : Widget, IGuiContainer
	{
		private class ProgressStep
		{
			private readonly IGuiFrame mProgressFrame;
			public IGuiFrame ProgressFrame
			{
				get { return mProgressFrame; }
			}
			private readonly IGuiFrame mContextFrame;
			public IGuiFrame ContextFrame
			{
				get { return mContextFrame; }
			}
			private readonly string mName;
			public string Name
			{
				get { return mName; }
			}
			private bool mEnabled = false; // Is this ProgressStep clickable?
			public bool Enabled
			{
				get { return mEnabled; }
				set 
				{
					mEnabled = value; 

					foreach(Button button in this.LinksToThisStep)
					{
						button.Enabled = mEnabled;
					}

					foreach (Button button in this.LinksFromThisStep)
					{
						button.Enabled = mEnabled;
					}
				}
			}

			private readonly List<Button> mLinksToThisStep = new List<Button>();
			public List<Button> LinksToThisStep
			{
				get { return mLinksToThisStep; }
			}

			private readonly List<Button> mLinksFromThisStep = new List<Button>();
			public List<Button> LinksFromThisStep
			{
				get { return mLinksFromThisStep; }
			}
			
			public ProgressStep(string name, IGuiFrame contextFrame, IGuiFrame progressFrame)
			{
				mName = name;
				mContextFrame = contextFrame;
				mProgressFrame = progressFrame;
			}
			public ProgressStep(ProgressStep copy)
			{
				mName = copy.Name;
				mContextFrame = (IGuiFrame)copy.ContextFrame.Clone();
				mProgressFrame = (IGuiFrame)copy.ProgressFrame.Clone();
			}
		}

		// TODO: Make these hard-coded values data driven
		private const float mProgressLabelHeight = 32.0f; // Height of the non-active progress steps

		// This is the position and size of the non-active progress step labels
		private readonly static IGuiSize mProgressLabelSize = new FillParent(Layout.Horizontal, new Vector2(0.0f, mProgressLabelHeight));
		private readonly IGuiStyle mProgressLabelStyle;

		// This is the position and size of widgets that get set as the active context
		private static readonly FillParent mContextSizePosition = new FillParent();

		// The progress frame contains each of the frames that describe the sequence
		// For example if there were steps for: 1. start, 2. edit and 3. save, the
		// frames that contain those names would be the ProgressFrame's children.
		private readonly Pair<IGuiFrame, IGuiPosition> mProgressFrame;

		// The context frame is the area for the current progress frame's display.
		private readonly Pair<IGuiFrame, IGuiPosition> mContextFrame;

		private readonly List<ProgressStep> mSteps = new List<ProgressStep>();


		public override object Clone()
		{
			return new ProgressView(this);
		}

		public ProgressView(ProgressView copy)
			: this
			(
				copy.Name, 
				copy.GuiSize, 
				(IGuiFrame)copy.mProgressFrame.First.Clone(),
				copy.mProgressFrame.Second,
				copy.mProgressLabelStyle,
				(IGuiFrame)copy.mContextFrame.First.Clone(),
				copy.mContextFrame.Second
			)
		{
			foreach( ProgressStep progressStep in copy.mSteps )
			{
				mSteps.Add(new ProgressStep(progressStep));
			}
		}

		public ProgressView(string name, 
							IGuiSize size,
							IGuiFrame progressFrame,
							IGuiPosition progressFramePosition,
							IGuiStyle progressLabelsStyle,
							IGuiFrame contextFrame,
							IGuiPosition contextFramePosition)
			: base(name, size, null)
		{
			if( progressFrame == null )
			{
				throw new ArgumentNullException("progressFrame");
			}
			if( progressFramePosition == null )
			{
				throw new ArgumentNullException("progressFramePosition");
			}
			if( contextFrame == null )
			{
				throw new ArgumentNullException("contextFrame");
			}
			if( contextFramePosition == null )
			{
				throw new ArgumentNullException("contextFramePosition");
			}

			mProgressFrame = new Pair<IGuiFrame, IGuiPosition>(progressFrame, progressFramePosition);
			mContextFrame = new Pair<IGuiFrame, IGuiPosition>(contextFrame, contextFramePosition);
			mProgressLabelStyle = progressLabelsStyle;

			mProgressFrame.First.Parent = this;
			mContextFrame.First.Parent = this;
		}

		/// <summary>
		/// Inserts a step into this ProgressView
		/// </summary>
		/// <param name="contextFrame">Frame that will be displayed in the ProgressView's context frame when this step is active</param>
		/// <param name="progressFrame">Frame that will be displayed in the ProgressView's progress frame when this step is active</param>
		/// <returns>Index of the step</returns>
		public int AddStep(string name, IGuiFrame contextFrame, IGuiFrame progressFrame)
		{
			mSteps.Add(new ProgressStep(name, contextFrame, progressFrame));
			return mSteps.Count - 1;
		}

		/// <summary>
		/// Overwrites a step at the given index
		/// </summary>
		/// <param name="stepNum">Index to overwrite</param>
		/// <param name="stepName">Name of the new step</param>
		/// <param name="contextFrame">Frame that will be displayed in the ProgressView's context frame when the new step is active</param>
		/// <param name="progressFrame">Frame that will be displayed in the ProgressView's progress frame when the new step is active</param>
		public void ReplaceStep(int stepNum, string name, IGuiFrame contextFrame, IGuiFrame progressFrame)
		{
			mSteps.RemoveAt(stepNum);
			mSteps.Insert(stepNum, new ProgressStep(name, contextFrame, progressFrame));
		}

		public void GoToStep(int index)
		{
			// Set the active step's context frame up
			mContextFrame.First.ClearChildWidgets();
			mContextFrame.First.AddChildWidget(mSteps[index].ContextFrame, mContextSizePosition);

			// Setup the progress bar
			mProgressFrame.First.ClearChildWidgets();
			float verticalOffset = 0.0f;
			for (int i = 0; i < mSteps.Count; ++i)
			{
				FixedPosition nextStepPosition = new FixedPosition(0.0f, verticalOffset);
				if( i == index )
				{
					// Draw the active step as whatever frame has been provided
					mProgressFrame.First.AddChildWidget(mSteps[i].ProgressFrame, nextStepPosition);
					mSteps[i].Enabled = true;
					verticalOffset += mSteps[i].ProgressFrame.Size.y;
				}
				else
				{
					// Draw the non-active steps as label-buttons
					string stepLabel = (i + 1) + ". " + mSteps[i].Name;
					Button progressLabelButton = new Button("ProgressLabel" + i, mProgressLabelSize, mProgressLabelStyle, stepLabel);
					SetupGoToStepButton(progressLabelButton, index, i);
					progressLabelButton.Enabled = mSteps[i].Enabled;
					mProgressFrame.First.AddChildWidget(progressLabelButton, nextStepPosition);
					verticalOffset += mProgressLabelHeight;
				}
			}
		}

		/// <summary>
		/// Set up a button that will change the current step of the progress view
		/// </summary>
		/// <param name="button">The button to set up</param>
		/// <param name="buttonIndex">The step the button is located on (optional)</param>
		/// <param name="targetIndex">The step that this button should transition to</param>
		public void SetupGoToStepButton(Button button, int? buttonIndex, int targetIndex)
		{
			if (!mSteps[targetIndex].LinksToThisStep.Contains(button))
			{
				button.Enabled = mSteps[targetIndex].Enabled;

				button.AddOnPressedAction(delegate()
				{
					this.GoToStep(targetIndex);
					mSteps[targetIndex].LinksToThisStep.Add(button);
				});

				if( buttonIndex != null )
				{
					mSteps[(int)buttonIndex].LinksFromThisStep.Add(button);
				}
			}
		}

		public override void Draw(IGuiContainer parent, Vector2 position)
		{
			mProgressFrame.First.Draw(this, mProgressFrame.Second.GetPosition(mProgressFrame.First));
			mContextFrame.First.Draw(this, mContextFrame.Second.GetPosition(mContextFrame.First));
		}

		public void UpdateChildPosition(IGuiElement element, Vector2 position)
		{
			if( element == mProgressFrame.First )
			{
				mProgressFrame.Second.UpdatePosition(mProgressFrame.First, position);
			}
			else if( element == mContextFrame.First )
			{
				mContextFrame.Second.UpdatePosition(mContextFrame.First, position);
			}
			else
			{
				throw new ArgumentException("Element(" + element.Name + ") is not a child of ProgressView(" + this.Name + ")", "element");
			}
		}

		public T SelectSingleElement<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectSingleElement<T>(this);
		}

		public IEnumerable<T> SelectElements<T>(string guiPath) where T : IGuiElement
		{
			GuiPath path = new GuiPath(guiPath);
			return path.SelectElements<T>(this);
		}

		public IEnumerable<IGuiElement> Children
		{
			get
			{
				yield return mProgressFrame.First;
				yield return mContextFrame.First;
			}
		}

		public Vector2 GetChildPosition(IGuiElement childElement)
		{
			return GetChildGuiPosition(childElement).GetPosition(childElement);
		}

		public IGuiPosition GetChildGuiPosition(IGuiElement childElement)
		{
			IGuiPosition result;
			if (childElement == mProgressFrame.First)
			{
				result = mProgressFrame.Second;
			}
			else if (childElement == mContextFrame.First)
			{
				result = mContextFrame.Second;
			}
			else
			{
				throw new Exception("Attempted to get a position for a child(" + childElement.Name + ") that isn't part of the IGuiContainer(" + this.ContainerName + ")");
			}
			return result;
		}

		public string ContainerName
		{
			get { return this.Name; }
		}
	}
}
