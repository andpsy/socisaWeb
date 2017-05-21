﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SOCISA;

namespace socisaWeb
{
    public class DashboardJson
    {
        [Display(Name = "Total dosare in baza de date")]
        public int DOSARE_TOTAL { get; set; }

        [Display(Name = "Total dosare CASCO in baza de date")]
        public int DOSARE_CASCO_TOTAL { get; set; }

        [Display(Name = "Total dosare RCA in baza de date")]
        public int DOSARE_RCA_TOTAL { get; set; }

        [Display(Name = "Dosare noi de la ultimul login")]
        public int DOSARE_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare CASCO noi de la ultimul login")]
        public int DOSARE_CASCO_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare RCA noi de la ultimul login")]
        public int DOSARE_RCA_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Mesaje noi")]
        public int MESAJE_NOI { get; set; }

        [Display(Name = "Mesaje noi (DOSAR NOU)")]
        public int MESAJE_NOI_DOSAR_NOU { get; set; }

        [Display(Name = "Mesaje noi (DOCUMENT NOU)")]
        public int MESAJE_NOI_DOCUMENT_NOU { get; set; }


        public DashboardJson() { }

        public DashboardJson(int ID_UTILIZATOR, int ID_SOCIETATE, string conStr)
        {
            DataAccess da = new DataAccess(ID_UTILIZATOR, conStr, System.Data.CommandType.StoredProcedure, "DASHBOARDsp_select", new object[] { new MySql.Data.MySqlClient.MySqlParameter("_ID_SOCIETATE", ID_SOCIETATE) });
            MySql.Data.MySqlClient.MySqlDataReader r = da.ExecuteSelectQuery();
            while (r.Read())
            {
                System.Data.IDataRecord dj = (System.Data.IDataRecord)r;
                try { this.DOSARE_TOTAL = Convert.ToInt32(dj["DOSARE_TOTAL"]); }
                catch { }
                try { this.DOSARE_CASCO_TOTAL = Convert.ToInt32(dj["DOSARE_CASCO_TOTAL"]); }
                catch { }
                try { this.DOSARE_RCA_TOTAL = Convert.ToInt32(dj["DOSARE_RCA_TOTAL"]); }
                catch { }
                try { this.DOSARE_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.DOSARE_CASCO_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_CASCO_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.DOSARE_RCA_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_RCA_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.MESAJE_NOI = Convert.ToInt32(dj["MESAJE_NOI"]); }
                catch { }
                try { this.MESAJE_NOI_DOSAR_NOU = Convert.ToInt32(dj["MESAJE_NOI_DOSAR_NOU"]); }
                catch { }
                try { this.MESAJE_NOI_DOCUMENT_NOU = Convert.ToInt32(dj["MESAJE_NOI_DOCUMENT_NOU"]); }
                catch { }

                break;
            }
            r.Close(); r.Dispose();
        }
    }
}