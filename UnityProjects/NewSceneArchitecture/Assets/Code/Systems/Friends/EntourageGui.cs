using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.FashionGame;
using UnityEngine;

namespace Hangout.Client.Gui
{
	public class EntourageGui : GuiController
	{
		private const string mResourcePath = "resources://GUI/Friends/EntourageGui.gui";

		private Window mMainWindow = null;

		private Label mMemberCountLabel = null;
		private Button mInviteFriendsButton = null;
		private ScrollFrame mEntourageListScrollFrame = null;
		private IGuiFrame mEntourageListingPrototypeFrame = null;

		public bool Showing
		{
			get { return mMainWindow.Showing; }
			set { mMainWindow.Showing = value; }
		}

		public EntourageGui(IGuiManager guiManager)
			: base(guiManager, mResourcePath)
		{
			foreach (IGuiElement element in this.AllElements)
			{
				if (element.Name == "EntourageGui" && element is Window)
				{
					mMainWindow = (Window)element;

					Button closeButton = mMainWindow.SelectSingleElement<Button>("MainFrame/EntourageListingsFrame/CancelButton");
					closeButton.AddOnPressedAction(
						delegate()
						{
							GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_CLOSE);
							mMainWindow.Showing = false;
						}
					);

					mMemberCountLabel = mMainWindow.SelectSingleElement<Label>("MainFrame/EntourageListingsFrame/MemberCountLabel");
					mMemberCountLabel.Text = String.Format(Translation.ENTOURAGE_MEMBER_COUNT, 0);
					mInviteFriendsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/EntourageListingsFrame/InviteFriendsButton");
					mInviteFriendsButton.Text = Translation.ENTOURAGE_INVITE_FRIENDS_BUTTON_TEXT;
					mInviteFriendsButton.AddOnPressedAction(delegate()
					{
						InviteFriendsToEntourage();
					});

					mEntourageListScrollFrame = mMainWindow.SelectSingleElement<ScrollFrame>("MainFrame/EntourageListingsFrame/EntourageListScrollFrame");
					mEntourageListingPrototypeFrame = mMainWindow.SelectSingleElement<IGuiFrame>("MainFrame/EntourageListingsFrame/EntourageListScrollFrame/EntourageListingPrototypeFrame");
					mEntourageListScrollFrame.RemoveChildWidget(mEntourageListingPrototypeFrame);
				}
			}
		}

		/// <summary>
		/// List<Pair<string, string>>, the first string is the facebook friend name, the second string is the facebook friend image url
		/// </summary>
		/// <param name="receivedFriends"></param>
		public void ListEntourage(List<Pair<string, string>> receivedFriends)
		{
			mEntourageListScrollFrame.ClearChildWidgets();
			ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();

			// Show the bonus
			int percentBonus = (int)(Rewards.GetEntourageExperienceBonusPercent(receivedFriends.Count) * 100);
			mMemberCountLabel.Text = String.Format(Translation.ENTOURAGE_MEMBER_COUNT, percentBonus);

			//sort by facebook friend name
			Comparison<Pair<string, string>> sortAlphabetically = new Comparison<Pair<string, string>>(SortNamesAlphabetically);
			receivedFriends.Sort(sortAlphabetically);


			foreach (Pair<string, string> friendNameAndImageUrl in receivedFriends)
			{
				IGuiFrame friendListing = (IGuiFrame)mEntourageListingPrototypeFrame.Clone();

				string facebookFriendPictureUrl = friendNameAndImageUrl.Second;
				clientAssetRepository.LoadAssetFromPath<ImageAsset>(facebookFriendPictureUrl,
				delegate(ImageAsset friendImageTexture)
				{
					Image friendImage = friendListing.SelectSingleElement<Image>("FriendImage");
					friendImage.Texture = friendImageTexture.Texture2D;
				});

				Label friendNameLabel = friendListing.SelectSingleElement<Label>("FriendName");
				friendNameLabel.Text = friendNameAndImageUrl.First;

				mEntourageListScrollFrame.AddChildWidget(friendListing, new HorizontalAutoLayout());
			}
		}

		private int SortNamesAlphabetically(Pair<string, string> first, Pair<string, string> second)
		{
			return string.Compare(first.First, second.First);
		}

		private void InviteFriendsToEntourage()
		{
			JSDispatcher jsd = new JSDispatcher();
			string inviteCampaign = "invite_entourage_gui";
			jsd.RequestFriendInviter(inviteCampaign, 
				delegate(string s)
				{
					EventLogger.Log(LogGlobals.CATEGORY_FACEBOOK, LogGlobals.FACEBOOK_INVITE, inviteCampaign, s);
					UnityEngine.Debug.Log("Invite to entourage result = " + s); 
				});
		}

	}
}
