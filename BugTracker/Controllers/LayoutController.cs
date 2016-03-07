using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class LayoutController : BaseController
    {
        [ChildActionOnly]
        public ActionResult UserFullName()
        {
            //string Name = String.Concat(GetUserInfo().FirstName, " ", GetUserInfo().LastName);
            
            return new ContentResult { Content = String.Concat(GetUserInfo().FirstName, " ", GetUserInfo().LastName) };
        }
        
        public ActionResult UserRole()
        {
            return new ContentResult { Content = GetRole().ToString() };
        }

    }
}