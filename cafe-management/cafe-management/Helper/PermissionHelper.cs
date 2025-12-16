using cafe_management.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.Helper
{
    public static class PermissionHelper
    {
        public static bool isAdmin
        {
            get
            {
                return UserSession.CurrentUser != null &&
                       UserSession.CurrentUser.Role == "admin";
            }
        }
    }
}

