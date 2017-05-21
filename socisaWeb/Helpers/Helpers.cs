using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace socisaWeb.Helpers
{
    public static class Helpers
    {
        public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
        {
            return evaluation ? value : MvcHtmlString.Empty;
        }

        public static MvcHtmlString HasRight(this MvcHtmlString value, string right)
        {
            bool hasRight = false;
            SOCISA.Models.Nomenclator n = (SOCISA.Models.Nomenclator)HttpContext.Current.Session["CURENT_USER_TYPE"];
            if (n.DENUMIRE.ToLower() == "administrator")
            {
                hasRight = true;
            }
            else
            {
                SOCISA.Models.Drept[] ds = (SOCISA.Models.Drept[])HttpContext.Current.Session["CURENT_USER_RIGHTS"];
                foreach (SOCISA.Models.Drept d in ds)
                {
                    if (d.DENUMIRE == right || d.DENUMIRE.ToLower() == "administrare")
                    {
                        hasRight = true;
                        break;
                    }
                }
            }
            return hasRight ? value : MvcHtmlString.Empty;
        }

        public static bool HasRight(string right)
        {
            bool hasRight = false;
            SOCISA.Models.Nomenclator n = (SOCISA.Models.Nomenclator)HttpContext.Current.Session["CURENT_USER_TYPE"];
            if (n.DENUMIRE.ToLower() == "administrator")
            {
                hasRight = true;
            }
            else
            {
                SOCISA.Models.Drept[] ds = (SOCISA.Models.Drept[])HttpContext.Current.Session["CURENT_USER_RIGHTS"];
                foreach (SOCISA.Models.Drept d in ds)
                {
                    if (d.DENUMIRE == right || d.DENUMIRE.ToLower() == "administrare")
                    {
                        hasRight = true;
                        break;
                    }
                }
            }
            return hasRight;
        }

        public static bool Avizare(int id)
        {
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            SOCISA.Models.Utilizator u = (SOCISA.Models.Utilizator)HttpContext.Current.Session["CURENT_USER"];
            SOCISA.Models.DosareRepository dr = new SOCISA.Models.DosareRepository(Convert.ToInt32(u.ID), conStr);
            SOCISA.Models.Dosar d = (SOCISA.Models.Dosar)dr.Find(id).Result;
            return d.ValidareAvizare().Status;
        }
    }
}