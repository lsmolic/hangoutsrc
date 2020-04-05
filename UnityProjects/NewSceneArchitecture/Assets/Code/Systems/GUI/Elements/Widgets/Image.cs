/**  --------------------------------------------------------  *
 *   Image.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/14/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;


namespace Hangout.Client.Gui
{
	public class Image : Widget
	{
		public override object Clone()
		{
			return new Image(this);
		}

		private Texture mTexture;
		
		public Image( Image copy )
		: base(copy.Name, copy.GuiSize, copy.Style)
		{
			mTexture = copy.Texture;
		}

		public Image(string name,
					 IGuiStyle style,
					 Texture texture )
		: base(name, new FixedSize(texture.width, texture.height), style) 
		{
			mTexture = texture;
		}

		public Image(string name,
					 Texture texture)
			: base(name, new FixedSize(texture.width, texture.height), null)
		{
			mTexture = texture;
		}

        public Image(string name,
                     IGuiStyle style,
                     IGuiSize size)
            : base(name, size, style)
        {
            mTexture = null;
        }

		public Image(string name,
			 IGuiStyle style,
			 IGuiSize size,
			 Texture2D texture)
			: base(name, size, style)
		{
			// TODO: Nobody destroys this texture that we just created. This essentially creates a leak here.
			mTexture = TextureUtility.ResizeTexture(texture, (int)size.GetSize(this).x, (int)size.GetSize(this).y);
		}

		public Texture Texture
		{
			get { return mTexture; }
			set { mTexture = value; }
		}
		
		public override void Draw(IGuiContainer container, Vector2 position) 
		{
			Rect coords = new Rect(position.x, position.y, this.ExternalSize.x, this.ExternalSize.y);
			if( this.Style != null )
			{
				GUI.Label(coords, mTexture, this.Style.GenerateUnityGuiStyle());
			}
			else 
			{
				GUI.Label(coords, mTexture);
			}
		}
	}	
}

