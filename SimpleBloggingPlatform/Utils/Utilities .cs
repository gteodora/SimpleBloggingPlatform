using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBloggingPlatform.Utils
{
    public static class Utilities
    {
        public static string GetZuluOfNow()
        {
            return DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        //TODO
        public static string Slugify(string title)
        {
            //...
            return title.ToLower();
        }
    }
}
