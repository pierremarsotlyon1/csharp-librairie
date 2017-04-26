using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace R
{
    public static class RTempData
    {
        public static bool Add<T>(this TempDataDictionary tempData, T value, string key = null) where T : class
        {
            try
            {
                if (value.IsNull() || key.IsNullOrEmpty()) return false;

                if (ContainsKey(tempData, key))
                {
                    return tempData.Update(value, key);
                }

                tempData.Add(key, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }    
        }

        private static bool Update<T>(this TempDataDictionary tempData, T value, string key = null) where T : class
        {
            try
            {
                if (value.IsNull() || key.IsNullOrEmpty()) return false;

                if (ContainsKey(tempData, key))
                {
                    if (tempData.Remove(key).IsFalse()) return false;
                }

                tempData.Add(key, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Remove(this TempDataDictionary tempData, string key = null)
        {
            try
            {
                return !key.IsNullOrEmpty() && tempData.Remove(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ContainsKey(TempDataDictionary tempData, string key = null)
        {
            try
            {
                if (key.IsNullOrEmpty()) return false;

                if (tempData.Keys.IsEmpty()) return false;

                foreach (var k in tempData.Keys)
                {
                    if (k.ToLower().Equals(key.ToLower())) return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Get(this TempDataDictionary tempData, string key = null)
        {
            try
            {
                if (key.IsNullOrEmpty()) return null;

                if (ContainsKey(tempData, key).IsFalse()) return null;

                var r = (from t in tempData.Keys where t.ToLower().Equals(key.ToLower()) select tempData[t].ToString()).FirstOrDefault();
                return r;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
