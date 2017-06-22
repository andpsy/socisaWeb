using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace socisaWeb.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Test()
        {
            try
            {
                ServiceReference1.SubrogationServiceClient s = new ServiceReference1.SubrogationServiceClient();
                s.ClientCredentials.UserName.UserName = "Drift Data";
                s.ClientCredentials.UserName.Password = "driftdata.1";
                ServiceReference1.SubrogationInfo[] sis = s.BrowseUnreadSubrogations(new DateTime(2017, 1, 1), new DateTime(2017, 6, 15));
            }catch(Exception exp) {
                SOCISA.LogWriter.Log(exp);
            }
            return View();
        }
    }
}