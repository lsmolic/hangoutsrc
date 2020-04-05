/*
   Created by Vilas Tewari on 2009-08-27.

	An Object that encapsulates frame sequences to be played by a UvAnimator
	Each frame sequence has a UvShellIndex associated with it.
	This index is used to identify which UvShell the frameSequence targets
*/

using UnityEngine;
using System.Collections;
using System.Xml;

namespace Hangout.Client
{
	public class SimpleUvAnimation : UvAnimation
	{
		/*
			The various animation frames that compose this SimpleUvAnimation
		*/
		private readonly int[][] mFrameSequences;
		
		/*
			What UvShell is a frameSequence targeting, indexed by it's corresponding index in the mFrameSequences Array
		*/
		private readonly string[] mFrameSequenceTarget;
		
		/*
			Properties
		*/
		public int UvShellTargetCount
		{
			get{ return mFrameSequenceTarget.Length; }
		}
		
		public SimpleUvAnimation( string name, string[] frameSequenceTarget, int[][] frameSequences )
		: base(name)
		{
			if ( frameSequenceTarget.Length == frameSequences.Length )
			{
				mFrameSequenceTarget = frameSequenceTarget;
				mFrameSequences = frameSequences;
			}
			else
				Console.LogError( "frameSequenceTarget and frameSequences Arrays must have equal length" );
		}
		
		public string GetUvShellTarget( int x )
		{
			return mFrameSequenceTarget[x];
		}
		public int[] GetFrameSequence( int x )
		{
			return mFrameSequences[x];
		}
		
		/*
			Create a SimpleUvAnimation from XML
		*/
		public static SimpleUvAnimation Parse( XmlNode uvAnimationXmlNode )
		{
			string name = XmlUtility.GetStringAttribute( uvAnimationXmlNode, "name" );
			XmlNodeList frameSequenceNodes = uvAnimationXmlNode.SelectNodes("descendant::FrameSequence");
			if( frameSequenceNodes.Count == 0 )
			{
				Console.LogError( "SimpleUvAnimation " + name + " does not have any FrameSequences defined");
				return null;
			}
			
			int[][] frameSequences = new int[frameSequenceNodes.Count][];
			string[] frameSequenceTarget = new string[frameSequenceNodes.Count];
			
			/* Foreach FrameSequence */
			int sequenceCount = 0;
			foreach( XmlNode frameSequenceNode in frameSequenceNodes )
			{
				string sequenceTarget = XmlUtility.GetStringAttribute( frameSequenceNode, "target" );
				frameSequenceTarget[sequenceCount] = sequenceTarget;
				
				XmlNodeList frameNodes = frameSequenceNode.SelectNodes("descendant::Frame");
				if( frameNodes.Count == 0 )
				{
					Console.LogError("FrameSequence for target " + sequenceTarget + " does not have any Frames defined");
					return null;
				}
				
				/* Foreach Animation Frame */
				frameSequences[sequenceCount] = new int[frameNodes.Count];
				int frameCount = 0;
				foreach( XmlNode frameNode in frameNodes )
				{
					frameSequences[sequenceCount][frameCount] = XmlUtility.GetIntAttribute( frameNode, "value" );
					++frameCount;
				}
				++sequenceCount;
			}
			return new SimpleUvAnimation( name, frameSequenceTarget, frameSequences );
		}
	}
}