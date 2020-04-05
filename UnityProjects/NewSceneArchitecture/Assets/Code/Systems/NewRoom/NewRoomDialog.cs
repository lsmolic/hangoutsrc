/**  --------------------------------------------------------  *
 *   NewRoomDialog.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/13/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class NewRoomDialog : GuiController
	{
		private const string mGuiPath = "resources://GUI/NewRoom/NewRoomDialog.gui";

		// This list is the mapping from expected Frame name in the XML to the location
		// in mProgressFrames that frame should be put.
		private readonly Pair<string>[] mProgressFrameNames = new Pair<string>[6]
		{
			new Pair<string>("InitialViewMain", "InitialViewProgress"),
			new Pair<string>("PremadeRoomMain", "PremadeRoomProgress"),
			new Pair<string>("CustomRoomMain", "CustomRoomProgress"),
			new Pair<string>("AdjustRoomMain", "AdjustRoomProgress"),
			new Pair<string>("SetRoomLocationMain", "SetRoomLocationProgress"),
			new Pair<string>("SetRoomMetadataMain", "SetRoomMetadataProgress")
		};

		private readonly Window mMainWindow;
		private readonly ProgressView mProgressView;

		public NewRoomDialog(IGuiManager guiManager)
			: base(guiManager, mGuiPath)
		{
			Pair<IGuiFrame>[] progressSteps = new Pair<IGuiFrame>[mProgressFrameNames.Length];
			for(int i = 0; i < progressSteps.Length; ++i)
			{
				progressSteps[i] = new Pair<IGuiFrame>();
			}

			// Grab the references to the required elements
			foreach (IGuiElement element in this.AllElements)
			{
				if (element is Window && element.Name == "NewRoomDialog")
				{
					mMainWindow = (Window)element;
				}
				else if (element is IGuiFrame)
				{
					IGuiFrame frame = (IGuiFrame)element;
					for(int i = 0; i < mProgressFrameNames.Length; ++i)
					{
						if( frame.Name == mProgressFrameNames[i].First )
						{
							progressSteps[i].First = frame;
						}
						else if( frame.Name == mProgressFrameNames[i].Second )
						{
							progressSteps[i].Second = frame;
						}
					}
				}
				else
				{
					throw new Exception("Unexpected " + element.GetType().Name + "(" + element.Name + ") found in the root of GUI XML(" + mGuiPath + ")");
				}
			}

			// Check to make sure all the frames are in the XML
			for (int i = 0; i < progressSteps.Length; ++i)
			{
				string failName = null;
				if (progressSteps[i].First == null)
				{
					failName = mProgressFrameNames[i].First;
				}
				else if (progressSteps[i].Second == null)
				{
					failName = mProgressFrameNames[i].Second;
				}
				if( failName != null )
				{
					throw new Exception("Unable to find frame(" + failName + ") in the root of " + mGuiPath);
				}
			}
			
			if( mMainWindow == null )
			{
				throw new Exception("Unable to find Window(NewRoomDialog) in the root of " + mGuiPath);
			}
			mMainWindow.Showing = false;

			mProgressView = BuildProgressView(progressSteps, this.GetNamedStyle("NavbarButton"));
			SetupProgressButtons(progressSteps);
			mProgressView.GoToStep(0);
		}

		private void SetupProgressButtons(Pair<IGuiFrame>[] progressSteps)
		{
			Hangout.Shared.Action<IGuiFrame, string, int, int> setupProgressButton = delegate(IGuiFrame frame, string buttonName, int targetStep, int initialStep)
			{
				foreach (Button button in frame.SelectElements<Button>("**/" + buttonName))
				{
					mProgressView.SetupGoToStepButton(button, initialStep, targetStep);
				}
			};

			for( int i = 0; i < progressSteps.Length; ++i)
			{
				Pair<IGuiFrame> progressStep = progressSteps[i];
				setupProgressButton(progressStep.Second, "Step1Button", 0, i);
				setupProgressButton(progressStep.Second, "Step2aButton", 1, i);
				setupProgressButton(progressStep.Second, "Step2bButton", 2, i);
				setupProgressButton(progressStep.Second, "Step3Button", 3, i);
				setupProgressButton(progressStep.Second, "Step4Button", 4, i);
				setupProgressButton(progressStep.Second, "Step5Button", 5, i);
			}
		}

		private ProgressView BuildProgressView(IEnumerable<Pair<IGuiFrame>> progressSteps, IGuiStyle progressLabelStyle)
		{
			IGuiFrame mainFrame = mMainWindow.SelectSingleElement<IGuiFrame>("MainFrame");
			if (mainFrame == null)
			{
				throw new Exception("Cannot find the navigation frame at 'NewRoomDialog/MainFrame'");
			}

			IGuiFrame navigationFrame = mainFrame.SelectSingleElement<IGuiFrame>("NavigationFrame");
			if (navigationFrame == null)
			{
				throw new Exception("Cannot find the navigation frame at 'NewRoomDialog/MainFrame/NavigationFrame'");
			}

			IGuiFrame viewFrame = mainFrame.SelectSingleElement<IGuiFrame>("ViewFrame");
			if (viewFrame == null)
			{
				throw new Exception("Cannot find the view frame at 'NewRoomDialog/MainFrame/ViewFrame'");
			}

			IGuiFrame toolFrame = viewFrame.SelectSingleElement<IGuiFrame>("ToolFrame");
			if (toolFrame == null)
			{
				throw new Exception("Cannot find the tool frame at 'NewRoomDialog/MainFrame/ViewFrame/ToolFrame'");
			}

			IGuiFrame topLetterbox = viewFrame.SelectSingleElement<IGuiFrame>("LetterboxTop");
			if (topLetterbox == null)
			{
				throw new Exception("Cannot find the tool frame at 'NewRoomDialog/MainFrame/ViewFrame/LetterboxTop'");
			}
			
			IGuiFrame bottomLetterbox = viewFrame.SelectSingleElement<IGuiFrame>("LetterboxBottom");
			if (topLetterbox == null)
			{
				throw new Exception("Cannot find the tool frame at 'NewRoomDialog/MainFrame/ViewFrame/LetterboxBottom'");
			}

			if (viewFrame.GuiSize is ProceduralSize)
			{
				// The view frame's size covers the screen except for the NavigationFrame's area
				((ProceduralSize)viewFrame.GuiSize).SizeFunc = delegate(IGuiElement element)
				{
					Vector2 size = element.Parent.Size;
					size.x -= navigationFrame.Size.x;
					return size;
				};
			}

			if (toolFrame.GuiSize is ProceduralSize)
			{
				// The tool frame's size is the largest frame that will fit in the ViewFrame while 
				// still being the same aspect ratio as the screen.
				((ProceduralSize)toolFrame.GuiSize).SizeFunc = delegate(IGuiElement element)
				{
					float aspectRatio = (float)Screen.width / (float)Screen.height;
					Vector2 size = viewFrame.Size;
					size.y = size.x / aspectRatio;
					return size;
				};
			}

			foreach (IGuiElement child in viewFrame.Children)
			{
				if((child is IGuiFrame))
				{
					IGuiFrame frame = (IGuiFrame)child;

					if (frame.GuiSize is ProceduralSize)
					{
						if (frame.Name == "LetterboxTop" || frame.Name == "LetterboxBottom")
						{
							// The letterbox sizes are 1/2 of the size difference between the
							// tool frame and the view frame
							((ProceduralSize)frame.GuiSize).SizeFunc = delegate(IGuiElement element)
							{
								Vector2 size = viewFrame.Size;
								size.y = (size.y - toolFrame.Size.y) * 0.5f;
								return size;
							};
						}
					}
				}
			}

			IGuiPosition toolFramePosition = viewFrame.GetChildGuiPosition(toolFrame);
			if( toolFramePosition is ProceduralPosition )
			{
				((ProceduralPosition)toolFramePosition).PositionFunc = delegate(IGuiElement element)
				{
					return new Vector2(0.0f, topLetterbox.Size.y);
				};
			}

			ProgressView result = new ProgressView
			(
				"NewRoomDialogProgressView", 
				new FillParent(), 
				navigationFrame, 
				mainFrame.GetChildGuiPosition(navigationFrame),
				progressLabelStyle,
				viewFrame,
				mainFrame.GetChildGuiPosition(viewFrame)
			);
			mainFrame.ClearChildWidgets();
			mainFrame.AddChildWidget(result, new FillParent());

			IGuiPosition topLetterboxPosition = viewFrame.GetChildGuiPosition(topLetterbox);
			IGuiPosition bottomLetterboxPosition = viewFrame.GetChildGuiPosition(bottomLetterbox);
			foreach(Pair<IGuiFrame> progressStep in progressSteps)
			{
				IGuiFrame newContextFrame = new GuiFrame(progressStep.First.Name.Replace("Main", ""), new FillParent());
				
				newContextFrame.AddChildWidget(topLetterbox, topLetterboxPosition);
				newContextFrame.AddChildWidget(progressStep.First, toolFramePosition);
				newContextFrame.AddChildWidget(bottomLetterbox, bottomLetterboxPosition);

				result.AddStep(newContextFrame.Name, newContextFrame, progressStep.Second);
			}
			return result;
		}

		public bool Showing
		{
			get { return mMainWindow.Showing;  }
			set { mMainWindow.Showing = value; }
		}
	}
}
