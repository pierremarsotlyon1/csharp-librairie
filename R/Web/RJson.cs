using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace R.Web
{
    public static class RJson
    {
        public static T DeserializeJson<T>(this string json, T obj)
        {
            try
            {
                return json.IsNullOrEmpty() ? default(T) : JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default (T);
            }
        }
    }
}
