using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncs.WpfApp.Helpers
{
    public static class SessionManager
    {
        public static string Token { get; private set; } = string.Empty;

        public static void SetToken(string token)
        {
            Token = token;
        }

        public static void ClearSession()
        {
            Token = string.Empty;
        }
    }
}
