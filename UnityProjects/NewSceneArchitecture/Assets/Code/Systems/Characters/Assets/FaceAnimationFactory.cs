/* Pherg 11/13/09
 * Builds a couple dictionaries for building moods from an xml file.
 */

using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class FaceAnimationFactory
	{
		private static string SIMPLE_UV_ANIMATION_LOOKUP_PATH = "assets://Animations/Avatar_uvanimations.xml";
		private static string COMPLEX_UV_ANIMATION_LOOKUP_PATH = "assets://Animations/Avatar_complexuvanimations.xml";
		private static string AVATAR_MOOD_TO_XML_LOOKUP_PATH = "assets://Animations/Avatar_moods.xml";
		
		private XmlDocument mSimpleUvAnimationsXml = null;
		private XmlDocument mComplexUvAnimationsXml = null;
		private XmlDocument mMoodToXml = null;
		
		private Dictionary<string, UvAnimation> mUvAnimationLookUpTable = new Dictionary<string, UvAnimation>();
		private Dictionary<MoodAnimation, XmlNode> mMoodToFaceAnimationNode = new Dictionary<MoodAnimation,XmlNode>();

		public FaceAnimationFactory(Hangout.Shared.Action finishedLoadingFaceAnimationAssets)
		{
			// TODO: Put in a better loading handler which processes all three of these in parallel. 
			ClientAssetRepository clientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			
			if (mSimpleUvAnimationsXml != null && mComplexUvAnimationsXml != null && mMoodToXml != null)
			{
				SetupSimpleUvAnimations(mSimpleUvAnimationsXml);
				SetupComplexUvAnimations(mComplexUvAnimationsXml);
				SetupMoodNameToXmlLookUp(mMoodToXml);
			}
			else
			{
				// The simple uvs must be created first since the complex uvs rely on them to be constructed.
				clientAssetRepository.LoadAssetFromPath<XmlAsset>(SIMPLE_UV_ANIMATION_LOOKUP_PATH, delegate(XmlAsset simpleUvAnimationsXmlAsset)
				{
					mSimpleUvAnimationsXml = simpleUvAnimationsXmlAsset.XmlDocument;
					SetupSimpleUvAnimations(simpleUvAnimationsXmlAsset.XmlDocument);
					clientAssetRepository.LoadAssetFromPath<XmlAsset>(COMPLEX_UV_ANIMATION_LOOKUP_PATH, delegate(XmlAsset complexUvAnimationsXmlAsset)
					{
						mComplexUvAnimationsXml = complexUvAnimationsXmlAsset.XmlDocument;
						SetupComplexUvAnimations(complexUvAnimationsXmlAsset.XmlDocument);
						clientAssetRepository.LoadAssetFromPath<XmlAsset>(AVATAR_MOOD_TO_XML_LOOKUP_PATH, delegate(XmlAsset moodToXml)
						{
							mMoodToXml = moodToXml.XmlDocument;
							SetupMoodNameToXmlLookUp(moodToXml.XmlDocument);
							finishedLoadingFaceAnimationAssets();
						});
					});
				});
			}
		}
		
		public FaceAnimation GetFaceAnimation(string moodName)
		{
			MoodAnimation mood = (MoodAnimation)Enum.Parse(typeof(MoodAnimation), moodName);
			return GetFaceAnimation(mood);
		}

		public FaceAnimation GetFaceAnimation(MoodAnimation moodName)
		{
			XmlNode moodNode;
			if (mMoodToFaceAnimationNode.TryGetValue(moodName, out moodNode))
			{
				return ConstructMood(moodNode);
			}
			else
			{
				Console.LogError("Could not create face animation for MoodAnimation: " + moodName.ToString());
				return null;
			}
		}

		private void SetupMoodNameToXmlLookUp(XmlDocument moodXml)
		{
			foreach (XmlNode faceAnimationNode in moodXml.SelectNodes("MoodAnimations/FaceAnimation"))
			{
				string moodName = XmlUtility.GetStringAttribute(faceAnimationNode, "name");
				MoodAnimation mood = (MoodAnimation)Enum.Parse(typeof(MoodAnimation), moodName);
				mMoodToFaceAnimationNode.Add(mood, faceAnimationNode);
			}
		}

		private void SetupSimpleUvAnimations(XmlNode rootNode)
		{
			/* foreach ComplexUvAnimation Range */
			foreach (XmlNode uvAnimationNode in rootNode.SelectNodes("descendant::SimpleUvAnimation"))
			{
				SimpleUvAnimation newUva = SimpleUvAnimation.Parse(uvAnimationNode);
				string uvAnimationId = XmlUtility.GetStringAttribute(uvAnimationNode, "assetId");
				mUvAnimationLookUpTable.Add(uvAnimationId, newUva);
			}
		}
		
		private void SetupComplexUvAnimations(XmlNode rootNode)
		{
			/* foreach ComplexUvAnimation Range */
			foreach (XmlNode complexUvAnimationNode in rootNode.SelectNodes("ComplexUvAnimations/Animation"))
			{
				ComplexUvAnimation newCuva = ParseComplexUvAnimation(complexUvAnimationNode);
				if (newCuva == null)
				{
					Console.LogError("Complex Uv Animation could not be created from node: " + complexUvAnimationNode);
				}
				else
				{
					string complexUvAnimationId = XmlUtility.GetStringAttribute(complexUvAnimationNode, "assetId");
					mUvAnimationLookUpTable.Add(complexUvAnimationId, newCuva);
				}
			}
		}
		
		private FaceAnimation ConstructMood(XmlNode moodRootNode)
		{
			string moodName = XmlUtility.GetStringAttribute(moodRootNode, "name");
			List<UvAnimation> moodUvAnimations = new List<UvAnimation>();
			foreach(XmlNode uvAnimationNode in moodRootNode.SelectNodes("Animation"))
			{
				string animationName = XmlUtility.GetStringAttribute(uvAnimationNode, "assetId");
				UvAnimation uvAnimation;
				if (mUvAnimationLookUpTable.TryGetValue(animationName, out uvAnimation))
				{
					moodUvAnimations.Add(uvAnimation);
				}
				else
				{
					Console.LogError("UvAnimation named: " + animationName + " could not be found in UvAnimation Lookup table.");
				}
			}
			return new FaceAnimation(moodName, moodUvAnimations[0], moodUvAnimations);
		}

		private ComplexUvAnimation ParseComplexUvAnimation(XmlNode complexUvAnimationXmlNode)
		{
			string name = XmlUtility.GetStringAttribute(complexUvAnimationXmlNode, "name");
			XmlNodeList complexFrameNodes = complexUvAnimationXmlNode.SelectNodes("ComplexFrame");
			if (complexFrameNodes.Count == 0)
			{
				Console.LogError("ComplexUvAnimation " + name + " does not have any ComplexFrames defined in node: " + complexUvAnimationXmlNode.OuterXml);
				return null;
			}

			ComplexFrame[] frames = new ComplexFrame[complexFrameNodes.Count];

			/* Foreach ComplexFrame */
			int complexFrameCount = 0;
			foreach (XmlNode complexFrameNode in complexFrameNodes)
			{
				int repeatFrame = XmlUtility.GetIntAttribute(complexFrameNode, "repeatFrame");
				float waitAfterFinished = XmlUtility.GetFloatAttribute(complexFrameNode, "waitAfterFinished");

				XmlNodeList simpleAnimationNodes = complexFrameNode.SelectNodes("SimpleAnimation");
				if (simpleAnimationNodes.Count == 0)
				{
					Console.LogError("ComplexFrame does not have any Simple animations defined in name: " + complexFrameNode.OuterXml);
					return null;
				}

				/* Foreach Animation Frame */
				List<SimpleUvAnimation> simpleAnimations = new List<SimpleUvAnimation>();
				foreach (XmlNode simpleAnimation in simpleAnimationNodes)
				{
					string uvAnimationId = XmlUtility.GetStringAttribute(simpleAnimation, "assetId");
					UvAnimation uvAnimation;
					if (mUvAnimationLookUpTable.TryGetValue(uvAnimationId, out uvAnimation))
					{
						SimpleUvAnimation simpleUvAnimation = (SimpleUvAnimation) uvAnimation;
						if (simpleUvAnimation == null)
						{
							Console.Log("Could not assign uvAnimation as SimpleAnimation from key: " + uvAnimationId);
						}
						else
						{
							simpleAnimations.Add(simpleUvAnimation);
						}
					}
					else
					{
						Console.LogError("Unable to pull simple uv animation from dictionary using key: " + uvAnimationId);
					}
					
				}
				frames[complexFrameCount] = new ComplexFrame(repeatFrame, waitAfterFinished, simpleAnimations.ToArray());
				++complexFrameCount;
			}

			return new ComplexUvAnimation(name, frames);
		}
	}
}
