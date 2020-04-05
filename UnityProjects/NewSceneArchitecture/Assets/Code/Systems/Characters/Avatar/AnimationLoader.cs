/**  --------------------------------------------------------  *
 *   AnimationLoader.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

namespace Hangout.Client
{
	public static class AnimationLoader
	{
		public static AnimationSequence LoadAnimationsFromRig( string name, UnityEngine.Animation animationComponent)
		{
			AnimationSequence result = new AnimationSequence(name);
			foreach( AnimationState animationState in animationComponent )
			{
				string[] splitName = animationState.clip.name.Split(':');
				if( splitName.Length == 0 )
				{
					throw new Exception( "AnimationState on " + animationComponent.gameObject.name + " does not follow the naming convention. (ex. 'Walk:Default:Loop')" );
				}
				
				if( splitName[0] == name )
				{					
					if( splitName[2] == "Start" )
					{
						result.AddFirstListing
						(
							new AnimationSequence.Listing
							(
								animationState.clip, 
								new AnimationLoopCount(1)
							)
						);
					}
					else if( splitName[2] == "End" )
					{
						result.AddLastListing
						(
							new AnimationSequence.Listing
							(
								animationState.clip, 
								new AnimationLoopCount(1)
							)
						);
					}
					else
					{
						result.AddListing
						(
							new AnimationSequence.Listing
							(
								animationState.clip, 
								new AnimationLoopCount(1)
							)
						);
					}
				}
			}
			
			if(result == null )
			{
				throw new Exception("Unable to find animation (" + name + ") on rig");
			}
			return result;
		}
		
// 		public static IDictionary<string, Emote> LoadEmotes(UnityEngine.Animation avatarAnimationComponent)
// 		{
// 			Dictionary<string, Emote> result = new Dictionary<string, Emote>(); 
// 			
// 			// Build the emote states
// 			XmlDocument emoteSettings = XmlUtility.LoadXmlDocument("resources://Settings/Emote.settings");
// 			foreach( XmlNode emoteNode in emoteSettings.SelectNodes("//Emote") )
// 			{
// 				if( emoteNode.Attributes["name"] == null )
// 				{
// 					throw new Exception("No name attribute was found on an emote node in Emote.settings.xml");
// 				}
// 				string emoteName = emoteNode.Attributes["name"].InnerText;
// 				if( result.ContainsKey(emoteName) )
// 				{
// 					throw new Exception("Cannot register 2 emotes with the same name (" + emoteName + ")");
// 				}
// 				
// 				AnimationSequence sequence = new AnimationSequence(emoteName); 
// 				foreach( XmlNode animationNode in emoteNode.SelectNodes("Animation") )
// 				{
// 					if( animationNode.Attributes["name"] == null )
// 					{
// 						throw new Exception("No name attribute was found on an animation node in Emote.settings.xml (" + emoteName + ")");
// 					}
// 					string name = emoteNode.Attributes["name"].InnerText;
// 					
// 					AnimationState state = avatarAnimationComponent[animationNode.Attributes["name"].InnerText];
// 					
// 					if ( state == null )
// 					{
// 						throw new Exception("Avatar rig does not contain an animation named: " + animationNode.Attributes["name"].InnerText + ".  You may need to reimport the avatar fbx.");
// 					}
// 					
// 					AnimationLoopCount loop = null;
// 					if( animationNode.Attributes["repeat"] != null )
// 					{	
// 						if( animationNode.Attributes["repeat"].InnerText == "infinite" )
// 						{
// 							loop = new AnimationLoopCount();
// 						}
// 						else
// 						{
// 							string[] minMax = animationNode.Attributes["repeat"].InnerText.Split(' ');
// 							if( minMax.Length != 2 )
// 							{
// 								throw new Exception("Error loading Emote.settings.xml: 'repeat' attributes must be in the form (min, max). This happend while loading Emote (" + name + ")");
// 							}
// 							
// 							try
// 							{
// 								loop = new AnimationLoopCount
// 								(
// 									uint.Parse(minMax[0]),
// 									uint.Parse(minMax[1])
// 								);
// 							}
// 							catch(FormatException e)
// 							{
// 								throw new Exception("Error loading Emote.settings.xml: 'repeat' attributes must be in the form ((int)min, (int)max). This happend while loading Emote (" + name + ")", e);
// 							}
// 						}
// 					}
// 					else
// 					{
// 						loop = new AnimationLoopCount(1);
// 					}
// 					
// 					sequence.AddListing( new AnimationSequence.Listing(state.clip, loop) );
// 				}
// 				
// 				result.Add(emoteName, new Emote(emoteName, sequence));
// 			}
// 			
// 			return result;
// 		}
	}	
}
