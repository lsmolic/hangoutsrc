using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Hangout.Server
{

public class TrieFilter
{
	private class TrieNode
	{
		private Dictionary<char,TrieNode> mChildren = new Dictionary<char,TrieNode>();
		public bool completesWord = false;
		public TrieNode parent = null;

		public void insertChild(char letter, TrieNode child)
		{
			mChildren.Add(letter, child);
			mChildren[letter].parent = this;
		}
		
		public TrieNode getChild(char letter)
		{
			if (mChildren.ContainsKey(letter))
			{
				return mChildren[letter];
			}
			else
			{
				return null;
			}
		}
	}
	
	private TrieNode mTop;
	public const char BEEPCHAR = '*';
	
	
	public TrieFilter(List<string> wordList)
	{
		mTop = new TrieNode();
		
		foreach(string word in wordList)
		{
			addWord(word);
		}
	}
	
	private void addWord(string word)
	{
		TrieNode currNode = mTop;
		
		word = word.ToLower();

		foreach (char c in word)
		{
			if (currNode.getChild(c) == null)
			{
				currNode.insertChild(c, new TrieNode());
			}
			currNode = currNode.getChild(c);
		}
		currNode.completesWord = true;
	}
	
	public bool isWord(string word)
	{
		TrieNode currNode = mTop;

		word = word.ToLower();
		
		foreach (char c in word)
		{
			currNode = currNode.getChild(c);
			if (currNode == null)
			{
				return false;
			}
		}
		return currNode.completesWord;
	}
	
	public bool containsAnyWords(string msg)
	{
		Queue<TrieNode> currCandidates = new Queue<TrieNode>();
		Queue<TrieNode> newCandidates = new Queue<TrieNode>();
		Queue<TrieNode> swapSpace;
		TrieNode node;

		msg = msg.ToLower();

		foreach (char c in msg)
		{
		    currCandidates.Enqueue(mTop);

			while(currCandidates.Count > 0)
			{
				node = currCandidates.Dequeue();
				node = node.getChild(c);
				if(node != null)
				{
					if(node.completesWord)
					{
						return true;
					}
					else
					{
						newCandidates.Enqueue(node);
					}
				}
			}

			swapSpace = currCandidates;
			currCandidates = newCandidates;
			newCandidates = swapSpace;
		}

		return false;
	}

	public string replaceAllWords(string msg)
	{
		bool b;
		return replaceAllWords(msg, out b);
	}

	public string replaceAllWords(string msg, out bool madeReplacement)
	{
		madeReplacement = false;
		Queue<TrieNode> candidates = new Queue<TrieNode>();
		TrieNode node;

		uint numCands;
		uint i;

		uint currMsgPos = 0;

		char [] cleanMsg = msg.ToCharArray();

		msg = msg.ToLower();

		foreach (char c in msg)
		{
		    candidates.Enqueue(mTop);
			numCands = (uint)candidates.Count;

			for(i=0;i<numCands;++i)
			{
				node = candidates.Dequeue();
				node = node.getChild(c);

				if(node != null)
				{
					if(node.completesWord)
					{
						// Nuke it
						uint scanBackPos = currMsgPos;
						TrieNode scanBackNode = node;

						madeReplacement = true;
						while(scanBackNode != mTop)
						{
							cleanMsg[scanBackPos] = BEEPCHAR;
							scanBackNode = scanBackNode.parent;
							--scanBackPos;
						}
					}
					// Enqueue this even if we just nuked a word, because
					// the word we just nuked might be a prefix of another
					candidates.Enqueue(node);
				}
			}

			++currMsgPos;
		}

		return new String(cleanMsg);
	}
}
}
