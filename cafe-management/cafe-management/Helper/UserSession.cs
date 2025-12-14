using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.Helper
{
    public static class UserSession
    {
        public static Account? CurrentUser { get; set; }
    }

}
