using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Hangout.Server;

public class FilterTest
{
	public delegate void Printer (string s);
	public static Printer Print = System.Console.WriteLine;


	public static void Main()
	{
		int iters = 1000;
		int dictSize = 1000;
		int msgLength = 100;

		Print("-- Begin filter benchmark --");
		Print("Number of messages scanned: " + iters);
		Print("Wordlist size: " + dictSize);
		Print("Message length: " + msgLength);

		List<string> wordList = new List<string> ();

		
		for(int i=0;i<(dictSize*2);i+=2)
		{
			wordList.Add("" + i);
		}
		

		TrieFilter myTrie = new TrieFilter(wordList);

		Regex wordsRegex = new Regex(String.Join("|",wordList.ToArray()), RegexOptions.Compiled);

		string testMsg = "";

		for(int i=0;i<(msgLength/5);i++)
		{
			testMsg = testMsg + "13579";
		}

		
		// ----- Run the tests! -----

		Print("\n-- START TRIE TEST --");
		Print("Filtering " + iters + " messages...");

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

		for(int i=0;i<iters;i++)
		{
			myTrie.replaceAllWords(testMsg);
		}

        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;

        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Print("Time: " + elapsedTime);
		Print("Messages/sec: " + (iters / ts.TotalSeconds));

		// -----
		
		Print("\n-- START OPTIMIZED REGEX TEST --");
		Print("Filtering " + iters + " messages...");

		stopWatch = new Stopwatch();
		stopWatch.Start();

		for(int i=0;i<iters;i++)
		{
			wordsRegex.Replace(testMsg, "****");
		}

        stopWatch.Stop();
        ts = stopWatch.Elapsed;

        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Print("Time: " + elapsedTime);
		Print("Messages/sec: " + (iters / ts.TotalSeconds));

		// -----

		Print("\n-- START PER-WORD REGEX TEST --");
		Print("Filtering " + iters + " messages...");

		stopWatch = new Stopwatch();
		stopWatch.Start();

		for(int i=0;i<iters;i++)
		{
 			foreach (string naughtyWord in wordList)
 			{
 				Regex.Replace(testMsg, naughtyWord, "****");
 			}
		}

        stopWatch.Stop();
        ts = stopWatch.Elapsed;

        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Print("Time: " + elapsedTime);
		Print("Messages/sec: " + (iters / ts.TotalSeconds));
		
	}
}
