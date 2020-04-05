using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Hangout.Server;

public class WordListTest
{
	public delegate void Printer (string s);
	public static Printer Print = System.Console.WriteLine;

	public static Random rand = new Random();


	public static void Main()
	{
		int iters = 1000;
		int msgLength = 100;

		Print("-- Begin filter benchmark --");
		Print("Number of messages scanned: " + iters);
		Print("Message length: " + msgLength);

		string [] words = {
			"wtf",
			"wop",
			"whore",
			"whoar",
			"wetback",
			"wank",
			"vagina",
			"twaty",
			"twat",
			"titty",
			"titties",
			"tits",
			"testicles",
			"teets",
			"spunk",
			"spic",
			"snatch",
			"smut",
			"sluts",
			"slut",
			"sleaze",
			"slag",
			"shiz",
			"shitty",
			"shittings",
			"shitting",
			"shitters",
			"shitter",
			"shitted",
			"shits",
			"shitings",
			"shiting",
			"shitfull",
			"shited",
			"shit",
			"shemale",
			"sheister",
			"sh!t",
			"scrotum",
			"screw",
			"schlong",
			"retard",
			"qweef",
			"queer",
			"queef",
			"pussys",
			"pussy",
			"pussies",
			"pusse",
			"punk",
			"prostitute",
			"pricks",
			"prick",
			"pr0n",
			"pornos",
			"pornography",
			"porno",
			"porn",
			"pissoff",
			"pissing",
			"pissin",
			"pisses",
			"pissers",
			"pisser",
			"pissed",
			"piss",
			"pimp",
			"phuq",
			"phuks",
			"phukking",
			"phukked",
			"phuking",
			"phuked",
			"phuk",
			"phuck",
			"phonesex",
			"penis",
			"pecker",
			"orgasms",
			"orgasm",
			"orgasims",
			"orgasim",
			"niggers",
			"nigger",
			"nigga",
			"nerd",
			"muff",
			"mound",
			"motherfucks",
			"motherfuckings",
			"motherfucking",
			"motherfuckin",
			"motherfuckers",
			"motherfucker",
			"motherfucked",
			"motherfuck",
			"mothafucks",
			"mothafuckings",
			"mothafucking",
			"mothafuckin",
			"mothafuckers",
			"mothafucker",
			"mothafucked",
			"mothafuckaz",
			"mothafuckas",
			"mothafucka",
			"mothafuck",
			"mick",
			"merde",
			"masturbate",
			"lusting",
			"lust",
			"loser",
			"lesbo",
			"lesbian",
			"kunilingus",
			"kums",
			"kumming",
			"kummer",
			"kum",
			"kuksuger",
			"kuk",
			"kraut",
			"kondums",
			"kondum",
			"kock",
			"knob",
			"kike",
			"kawk",
			"jizz",
			"jizm",
			"jiz",
			"jism",
			"jesus h christ",
			"jesus fucking christ",
			"jerk-off",
			"jerk",
			"jap",
			"jackoff",
			"jacking off",
			"jackass",
			"jack-off",
			"jack off",
			"hussy",
			"hotsex",
			"horny",
			"horniest",
			"hore",
			"hooker",
			"honkey",
			"homo",
			"hoer",
			"hell",
			"hardcoresex",
			"hard on",
			"h4x0r",
			"h0r",
			"guinne",
			"gook",
			"gonads",
			"goddamn",
			"gazongers",
			"gaysex",
			"gay",
			"gangbangs",
			"gangbanged",
			"gangbang",
			"fux0r",
			"furburger",
			"fuks",
			"fuk",
			"fucks",
			"fuckme",
			"fuckings",
			"fucking",
			"fuckin",
			"fuckers",
			"fucker",
			"fucked",
			"fuck",
			"fu",
			"foreskin",
			"fistfucks",
			"fistfuckings",
			"fistfucking",
			"fistfuckers",
			"fistfucker",
			"fistfucked",
			"fistfuck",
			"fingerfucks",
			"fingerfucking",
			"fingerfuckers",
			"fingerfucker",
			"fingerfucked",
			"fingerfuck",
			"fellatio",
			"felatio",
			"feg",
			"feces",
			"fcuk",
			"fatso",
			"fatass",
			"farty",
			"farts",
			"fartings",
			"farting",
			"farted",
			"fart",
			"fags",
			"fagots",
			"fagot",
			"faggs",
			"faggot",
			"faggit",
			"fagging",
			"fagget",
			"fag",
			"ejaculation",
			"ejaculatings",
			"ejaculating",
			"ejaculates",
			"ejaculated",
			"ejaculate",
			"dyke",
			"dumbass",
			"douche bag",
			"dong",
			"dipshit",
			"dinks",
			"dink",
			"dildos",
			"dildo",
			"dike",
			"dick",
			"damn",
			"damn",
			"cyberfucking",
			"cyberfuckers",
			"cyberfucker",
			"cyberfucked",
			"cyberfuck",
			"cyberfuc",
			"cunts",
			"cuntlicking",
			"cuntlicker",
			"cuntlick",
			"cunt",
			"cunnilingus",
			"cunillingus",
			"cunilingus",
			"cumshot",
			"cums",
			"cumming",
			"cummer",
			"cum",
			"crap",
			"cooter",
			"cocksucks",
			"cocksucking",
			"cocksucker",
			"cocksucked",
			"cocksuck",
			"cocks",
			"cock",
			"cobia",
			"clits",
			"clit",
			"clam",
			"circle jerk",
			"chink",
			"cawk",
			"buttpicker",
			"butthole",
			"butthead",
			"buttfucker",
			"buttfuck",
			"buttface",
			"butt hair",
			"butt fucker",
			"butt breath",
			"butt",
			"butch",
			"bung hole",
			"bum",
			"bullshit",
			"bull shit",
			"bucket cunt",
			"browntown",
			"browneye",
			"brown eye",
			"boner",
			"bonehead",
			"blowjobs",
			"blowjob",
			"blow job",
			"bitching",
			"bitchin",
			"bitches",
			"bitchers",
			"bitcher",
			"bitch",
			"bestiality",
			"bestial",
			"belly whacker",
			"beaver",
			"beastility",
			"beastiality",
			"beastial",
			"bastard",
			"balls",
			"asswipe",
			"asskisser",
			"assholes",
			"asshole",
			"asses",
			"ass lick",
			"ass"
			};

		List<string> wordList = new List<string> (words);

		TrieFilter myTrie = new TrieFilter(wordList);

		Regex wordsRegex = new Regex(String.Join("|",wordList.ToArray()), RegexOptions.Compiled);

		string testMsg = "";
		string filterResult = "";

		while(testMsg.Length < msgLength)
		{
			//Worst case, whole message is bad
			//testMsg += wordList[rand.Next(wordList.Count)];

			//Random 16-bit characters
			//testMsg += (char)rand.Next(1<<16);

			testMsg += (char)(rand.Next(26)+97);

			if(rand.Next(100) < 5)
			{
				testMsg += wordList[rand.Next(wordList.Count)];
			}
		}

		Print("Test message:");
		Print(testMsg);

		// ----- Run the tests! -----

		Print("\n-- START TRIE TEST --");
		Print("Filtering " + iters + " messages...");

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

		for(int i=0;i<iters;i++)
		{
			filterResult = myTrie.replaceAllWords(testMsg);
		}

		Print("Filtered message sample:\n" + filterResult);

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
			filterResult = wordsRegex.Replace(testMsg, "****");
		}

		Print("Filtered message sample:\n" + filterResult);

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
			filterResult = testMsg;
 			foreach (string naughtyWord in wordList)
 			{
 				filterResult = Regex.Replace(filterResult, naughtyWord, "****");
 			}
		}
		
		Print("Filtered message sample:\n" + filterResult);

        stopWatch.Stop();
        ts = stopWatch.Elapsed;

        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Print("Time: " + elapsedTime);
		Print("Messages/sec: " + (iters / ts.TotalSeconds));
		
		
	}
}
