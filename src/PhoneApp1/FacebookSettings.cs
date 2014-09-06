using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook
{
    public class FacebookSettings
    {
        public static string AppID = "473794092677913";
        public static string AppSecret = "9474ac81133a04ee1b570d7ffc6f6b85";
    }

    public class FacebookAccess
    {
        public string AccessToken { get; set; }
        public string UserId { get; set; }
    }
}
