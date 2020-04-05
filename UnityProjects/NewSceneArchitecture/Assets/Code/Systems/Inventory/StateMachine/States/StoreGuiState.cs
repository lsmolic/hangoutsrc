/**  --------------------------------------------------------  *
 *   StoreGuiState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/21/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client.Gui
{
	public abstract class StoreGuiState : State
	{
		private Window mGuiWindow;
		protected Window GuiWindow
		{
			get { return mGuiWindow; }
		}
		
		public StoreGuiState(Window guiWindow)
		{
			mGuiWindow = guiWindow;
		}

        public abstract void HandleSearchResults(XmlDocument xmlResponse);

        /// <summary>
        /// The coroutine to download asset using WWW class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        protected void ApplyImageToButton(string path, Button button, Image thumbnail)
        {
			GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().LoadAssetFromPath<ImageAsset>(path, delegate(ImageAsset imageAsset)
			{
				button.Text = "";
				//button.Image = imageAsset.Texture2D;
				thumbnail.Texture = imageAsset.Texture2D;
				button.Enabled = true;
            	button.Showing = true;
            });
        }

        protected void PositionWindowOverButton(Window window, IWidget widget, GridView grid)
        {
            int overlapPixels = 0;
			Vector2 widgetPosition = grid.GetChildPosition(widget);
            //If the selected button is in the middle of the grid
			if (widgetPosition.x != 0 && widgetPosition.x <= widget.Size.x * (grid.Columns - 1))
            {
				float xPosition = widgetPosition.x + ((widget.Size.x / 2) - (window.Size.x / 2));
                widgetPosition = new Vector2(xPosition, widgetPosition.y);
            }
            //If the button is in the right column.
            else if (widgetPosition.x != 0)
            {
				float xPosition = widgetPosition.x + widget.Size.x - window.Size.x + overlapPixels;
                widgetPosition = new Vector2(xPosition, widgetPosition.y);
            }
            //If the button is in the left column.
            else if (widgetPosition.x == 0)
            {
                float xPosition = widgetPosition.x - overlapPixels;
                widgetPosition = new Vector2(xPosition, widgetPosition.y);
            }

            //If button is in the middle row of the grid
			if (widgetPosition.y != 0 && widgetPosition.y <= widget.Size.y * (grid.Rows - 1))
            {
				float yPosition = widgetPosition.y + ((widget.Size.y / 2) - (window.Size.y / 2));
                widgetPosition = new Vector2(widgetPosition.x, yPosition);
            }
            //If the button is in the bottom row
            else if (widgetPosition.y != 0)
            {
				float yPosition = widgetPosition.y + widget.Size.y - window.Size.y + overlapPixels;
                widgetPosition = new Vector2(widgetPosition.x, yPosition);
            }
            //If the button is in the top row
            else if (widgetPosition.y == 0)
            {
                float yPosition = widgetPosition.y - overlapPixels;
                widgetPosition = new Vector2(widgetPosition.x, yPosition);
            }
            Vector2 gridPos = grid.GetScreenPosition();
            window.Manager.SetTopLevelPosition(window, new FixedPosition(gridPos + widgetPosition));
            window.Showing = true;
            window.InFront = true;
        }
    
        /// <summary>
        /// Urls from payment items come in the form http://assetBase/urlstuff.   We need
        /// to convert it to assetBaseUrl + urlStuff to work with our deployed assets
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string ConvertPaymentItemsUrl(string url)
        {
            string newUrl = url.Replace("http://www.assetbase.net/", "assets://");
            return newUrl;
        }

    }
}