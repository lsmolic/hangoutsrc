using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hangout.Shared
{
    internal class ConfigSelector : IComparable<ConfigSelector>
    {
        // Ordered by priority
        private enum SelectorType
        {
            FBID = 0,
            GROUP = 1,
            MOD = 2,
            DEFAULT = 3
        }

        private SelectorType mType;
        private long mFBID;
        private int mModulus;
        private int mRemainder;
        private string mGroup;

        public ConfigSelector(string str)
        {
            if (Regex.IsMatch(str, @"^DEFAULT$"))
            {
                mType = SelectorType.DEFAULT;
            }
            else if (Regex.IsMatch(str, @"^FBID:[0-9]+$"))
            {
                mType = SelectorType.FBID;
                mFBID = Int64.Parse(str.Substring(5, str.Length - 5));
            }
            else if (Regex.IsMatch(str, @"^MOD[0-9]+==[0-9]+$"))
            {
                mType = SelectorType.MOD;
                Match match = Regex.Match(str, @"^MOD([0-9]+)==([0-9]+)$");
                mModulus = Int32.Parse(match.Groups[1].Captures[0].Value);
                mRemainder = Int32.Parse(match.Groups[2].Captures[0].Value);
            }
            else if (Regex.IsMatch(str, @"^GROUP:[a-zA-Z0-9_]+$"))
            {
                mType = SelectorType.GROUP;
                mGroup = str.Substring(6, str.Length - 6);
            }
            else
            {
                throw new ArgumentException("Invalid selector text: " + str);
            }
        }

        private static bool CheckSelector(string str)
        {
            return Regex.IsMatch(str, @"^DEFAULT$|^MOD[0-9]+==[0-9]+$|^FBID:[0-9]+$|^GROUP:[a-zA-Z0-9_]+$");
        }

        public int CompareTo(ConfigSelector other)
        {
            if (this.mType != other.mType)
            {
                return this.mType.CompareTo(other.mType);
            }
            else
            {
                switch (mType)
                {
                    case SelectorType.DEFAULT:
                        return 0;
                    case SelectorType.FBID:
                        return this.mFBID.CompareTo(other.mFBID);
                    case SelectorType.GROUP:
                        return this.mGroup.CompareTo(other.mGroup);
                    case SelectorType.MOD:
                        if (this.mModulus != other.mModulus)
                        {
                            return this.mModulus.CompareTo(other.mModulus);
                        }
                        else
                        {
                            return this.mRemainder.CompareTo(other.mRemainder);
                        }
                    default:
                        throw new Exception("Invalid SelectorType, probably need new case statement in ConfigSelector.Matches");
                }  
            }
        }

        public bool Matches(long? fbId, long? nonIdentifyingHash, List<string> groups)
        {
            switch (mType)
            {
                case SelectorType.DEFAULT:
                    return true;
                case SelectorType.FBID:
                    return fbId == mFBID;
                case SelectorType.GROUP:
                    return groups != null && groups.Contains(mGroup);
                case SelectorType.MOD:
                    if (fbId != null)
                    {
                        return fbId % mModulus == mRemainder;
                    }
                    else
                    {
                        return nonIdentifyingHash % mModulus == mRemainder;
                    }
                default:
                    throw new Exception("Invalid SelectorType, probably need new case statement in ConfigSelector.Matches");
            }
        }
    }

}
