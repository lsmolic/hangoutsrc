/**  --------------------------------------------------------  *
 *   Textbox.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/21/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client.Gui
{
	public class Textbox : Widget, ITextGuiElement
	{
		private bool mSingleLine = false;
		private bool mUserEditable = false;
		private string mText = "";

        private int? mMaxLength = null;

		public override object Clone()
		{
			return new Textbox(this);
		}

		public Textbox(Textbox copy)
			: this(copy.Name, copy.GuiSize, copy.Style, copy.Text, copy.SingleLine, copy.UserEditable, copy.MaxLength) { }
        
        public Textbox(string name,
                        IGuiSize size,
                        IGuiStyle style,
                        string text,
                        bool singleLine,
                        bool userEditable)
            : this(name, size, style, text, singleLine, userEditable, null) { }
        

		public Textbox(string name,
						IGuiSize size,
						IGuiStyle style,
						string text,
						bool singleLine,
						bool userEditable,
                        int? maxLength)
			: base(name, size, style)
		{
			if (mText == null)
			{
				throw new ArgumentNullException("text");
			}

			mText = text;
			mSingleLine = singleLine;
			mUserEditable = userEditable;
            mMaxLength = maxLength;
		}

		public string Text
		{
			get { return mText; }
            set //{ mText = value; }
            {
                if ((mMaxLength == null) || (value.Length <= mMaxLength))
                {
                    mText = value;
                }
            }
		}
		public bool SingleLine
		{
			get { return mSingleLine; }
			set { mSingleLine = value; }
		}
		public bool UserEditable
		{
			get { return mUserEditable; }
			set { mUserEditable = value; }
		}
        
        public int? MaxLength
        {
            get { return mMaxLength; }
            set
            { 
                mMaxLength = value; 
                Text = mText;
            }
        }

		private readonly List<Hangout.Shared.Action> mTextChangedCallbacks = new List<Hangout.Shared.Action>();
		public IReceipt AddTextChangedCallback(Hangout.Shared.Action onTextChanged)
		{
			if (onTextChanged == null)
			{
				throw new ArgumentNullException("onTextChanged");
			}

			mTextChangedCallbacks.Add(onTextChanged);
			return new Receipt(delegate()
			{
				mTextChangedCallbacks.Remove(onTextChanged);
			});
		}
        
		public override void Draw(IGuiContainer container, Vector2 position)
		{
			string resultText = "";
			Rect coords = new Rect(position.x, position.y, this.Size.x, this.Size.y);

			if (Text == null)
			{
				Text = "";
			}

			if (mSingleLine)
			{
				if (this.Style != null)
				{
					resultText = GUI.TextField(coords, Text, this.Style.GenerateUnityGuiStyle());
				}
				else
				{
					resultText = GUI.TextField(coords, Text);
				}
			}
			else
			{
				if (this.Style != null)
				{
					resultText = GUI.TextArea(coords, Text, this.Style.GenerateUnityGuiStyle());
				}
				else
				{
					resultText = GUI.TextArea(coords, Text);
				}
			}

			if (mUserEditable)
			{
				bool textChanged = Text != resultText;
				Text = resultText;

				if (textChanged)
				{
					foreach (Hangout.Shared.Action callback in mTextChangedCallbacks)
					{
						callback();
					}
				}
			}
		}
	}
}

