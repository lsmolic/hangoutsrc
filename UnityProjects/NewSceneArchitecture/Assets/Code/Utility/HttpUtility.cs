/* 
 * HttpUtility
 * 
 * Helper functions shamelessly copied from Future Mono's System.Web.HttpUtility
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hangout.Client
{
    public static class HttpUtility
    {
        static char[] hexChars = "0123456789abcdef".ToCharArray();

        public static string UrlPathEncode(string s)
        {
            if (s == null || s.Length == 0)
            {
                return s;
            }
            MemoryStream result = new MemoryStream();
            int length = s.Length;
            for (int i = 0; i < length; ++i)
            {
                UrlPathEncodeChar(s[i], result);
            }
            return Encoding.ASCII.GetString(result.ToArray());
        }

        private static void UrlPathEncodeChar(char c, Stream result)
        {
            if (c < 33 || c > 126)
            {
                byte[] bIn = Encoding.UTF8.GetBytes(c.ToString());
                for (int i = 0; i < bIn.Length; ++i)
                {
                    result.WriteByte((byte)'%');
                    int idx = ((int)bIn[i]) >> 4;
                    result.WriteByte((byte)hexChars[idx]);
                    idx = ((int)bIn[i]) & 0x0F;
                    result.WriteByte((byte)hexChars[idx]);
                }
            }
            else if (c == ' ')
            {
                result.WriteByte((byte)'%');
                result.WriteByte((byte)'2');
                result.WriteByte((byte)'0');
            }
            else
            {
                result.WriteByte((byte) c);
            }
        }

        public static string LocalPathToFileUrlPrefix(string localPath)
        {
            string result = "file:///" + UrlPathEncode(localPath.Replace("\\", "/"));

            if (result[result.Length - 1] != '/')
            {
                return result + "/";
            }
            else
            {
                return result;
            }
        }
    }
}
