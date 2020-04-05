using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Text.RegularExpressions;

namespace Hangout.Server
{
	public class TextFilter
	{
		// The filter that catches bad words
		private TrieFilter mBadWordFilter = null;

		// The filter that catches strings that include bad words but aren't actually bad
		private TrieFilter mExclusionFilter = null;
		private bool mHasBeenCalled = false;
		private static TextFilter mInstance = null;

		public static TextFilter Instance
		{
            get
            {
                if (mInstance == null)
                {
                    mInstance = new TextFilter();
                }
                return mInstance;
            }
		}
		private TextFilter()
		{
			GetApproveDenyWords();
		}

		private bool IsIgnoredChar(char c)
		{
			return c == ' ';
		}

		private string CondenseString(string str)
		{
			List<char> result = new List<char>();

			foreach (char c in str)
			{
				if (!IsIgnoredChar(c))
				{
					result.Add(Char.ToLower(c));
				}
			}
			return new String(result.ToArray());
		}

		private string ApplyCondensedFilter(string original, char [] filter)
		{
			List<char> result = new List<char>();
			uint filterPos = 0;
			bool midBeep = false;
			foreach (char c in original)
			{
				if (IsIgnoredChar(c))
				{
					if(midBeep)
					{
						result.Add(TrieFilter.BEEPCHAR);
					}
					else
					{
						result.Add(c);
					}
				}
				else
				{
					if (filter[filterPos] == TrieFilter.BEEPCHAR)
					{
						result.Add(TrieFilter.BEEPCHAR);
						midBeep = true;
					}
					else
					{
						result.Add(c);
						midBeep = false;
					}
					++filterPos;
				}
			}
			return new String(result.ToArray());
		}

		public string ReplaceNaughtyWords(string textInQuestion)
		{
			bool madeReplacement;

			string condensed = CondenseString(textInQuestion);

			string filtered = mBadWordFilter.replaceAllWords(condensed, out madeReplacement);
			
			if (madeReplacement)
			{
				string excluded = mExclusionFilter.replaceAllWords(condensed, out madeReplacement);
				
				char [] finalCondensed = filtered.ToCharArray();

				if (madeReplacement)
				{
					// Found exclusion(s) in the original message.
					// For each excluded char, restore it if it was nuked.
					for (int i=0; i<finalCondensed.Length; ++i)
					{
						if ((condensed[i] != filtered[i]) && (excluded[i] == TrieFilter.BEEPCHAR))
						{
							finalCondensed[i] = condensed[i];
						}
					}
				}

				return ApplyCondensedFilter(textInQuestion, finalCondensed);
			}
			else
			{
				return textInQuestion;
			}
		}

		private void GetApproveDenyWords()
		{
			string webServiceUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
			if(String.IsNullOrEmpty(webServiceUrl))
			{
				throw new Exception("App/Web config does not contain a definition for 'WebServicesBaseUrl'");
			}
			WebServiceRequest getApproveDenyWordsRequest = new WebServiceRequest(webServiceUrl, "Logging", "GetApproveDenyWordList");
			getApproveDenyWordsRequest.GetWebResponseAsync(ProcessApproveDenyWords);
		}

		private void ProcessApproveDenyWords(XmlDocument approveDenyWordsXmlDoc)
		{
			List<string> naughtyWords = new List<string>();
			List<string> niceWords = new List<string>();

			XmlNodeList naughtyWordXmlNodes = approveDenyWordsXmlDoc.SelectNodes("ApproveDenyWords/NaughtyWords/No");
			XmlNodeList niceWordXmlNodes = approveDenyWordsXmlDoc.SelectNodes("ApproveDenyWords/NiceWords/Yes");

			//parse naughty list
			foreach(XmlNode naughtyWordNode in naughtyWordXmlNodes)
			{
				naughtyWords.Add(CondenseString(naughtyWordNode.InnerText));
			}

			foreach (XmlNode niceWordNode in niceWordXmlNodes)
			{
				niceWords.Add(CondenseString(niceWordNode.InnerText));
			}

			mBadWordFilter = new TrieFilter(naughtyWords);

			mExclusionFilter = new TrieFilter(niceWords);

			//// XXX Hard coding, get this from real source!
			//List<string> exclusions = new List<string>();
			//exclusions.Add("assistant");
			//exclusions.Add("corkscrew");
			//exclusions.Add("bluster");
			//exclusions.Add("japanese");
			//exclusions.Add("hello");
			//exclusions.Add("fun");
			//mExclusionFilter = new TrieFilter(exclusions);
		}
	}
}
