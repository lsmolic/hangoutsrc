using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    public static class CheckType
    {
        public static T TryAssignType<T>(object objectToConvert)
        {
            try
            {
                T returnValue = (T)Convert.ChangeType(objectToConvert, typeof(T));
                return returnValue;
            }
            catch (System.Exception)
            {
                throw new Exception("Error casting " + objectToConvert.GetType().ToString() + " to " + typeof(T).ToString() + ". Check that the data is valid.");
            }
        }
    }
}
