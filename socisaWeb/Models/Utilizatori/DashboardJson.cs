using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SOCISA;
using SOCISA.Models;

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

        // -- doar pt. Admin si Super --

        [Display(Name = "Dosare neasignate")]
        public int DOSARE_NEASIGNATE { get; set; }

        [Display(Name = "Dosare CASCO neasignate")]
        public int DOSARE_CASCO_NEASIGNATE { get; set; }

        [Display(Name = "Dosare RCA neasignate")]
        public int DOSARE_RCA_NEASIGNATE { get; set; }

        [Display(Name = "Dosare neasignate de la ultimul login")]
        public int DOSARE_NEASIGNATE_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare CASCO neasignate de la ultimul login")]
        public int DOSARE_NEASIGNATE_CASCO_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare RCA neasignate de la ultimul login")]
        public int DOSARE_NEASIGNATE_RCA_FROM_LAST_LOGIN { get; set; }

        // -- pt. All --

        [Display(Name = "Dosare noi de la ultimul login")]
        public int DOSARE_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare CASCO noi de la ultimul login")]
        public int DOSARE_CASCO_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare RCA noi de la ultimul login")]
        public int DOSARE_RCA_FROM_LAST_LOGIN { get; set; }

        [Display(Name = "Dosare neoperate")]
        public int DOSARE_NEOPERATE { get; set; }

        [Display(Name = "Dosare CASCO neoperate")]
        public int DOSARE_CASCO_NEOPERATE { get; set; }

        [Display(Name = "Dosare RCA neoperate")]
        public int DOSARE_RCA_NEOPERATE { get; set; }

        [Display(Name = "Mesaje noi")]
        public int MESAJE_NOI { get; set; }

        [Display(Name = "Mesaje noi (DOSAR NOU)")]
        public int MESAJE_NOI_DOSAR_NOU { get; set; }

        [Display(Name = "Mesaje noi (DOCUMENT NOU)")]
        public int MESAJE_NOI_DOCUMENT_NOU { get; set; }


        public DashboardJson() { }

        public DashboardJson(int ID_UTILIZATOR, int ID_SOCIETATE, string conStr)
        {
            DataAccess da = new DataAccess(ID_UTILIZATOR, conStr, System.Data.CommandType.StoredProcedure, "DASHBOARDsp_select", new object[] { new MySql.Data.MySqlClient.MySqlParameter("_ID_SOCIETATE", ID_SOCIETATE), new MySql.Data.MySqlClient.MySqlParameter("_EXPIRATION_DAYS", 15) }); // TO DO: de adaugat parametru in setari !!!
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

                // -- doar pt. Admin si Super --
                try { this.DOSARE_NEASIGNATE = Convert.ToInt32(dj["DOSARE_NEASIGNATE"]); }
                catch { }
                try { this.DOSARE_CASCO_NEASIGNATE = Convert.ToInt32(dj["DOSARE_CASCO_NEASIGNATE"]); }
                catch { }
                try { this.DOSARE_RCA_NEASIGNATE = Convert.ToInt32(dj["DOSARE_RCA_NEASIGNATE"]); }
                catch { }

                try { this.DOSARE_NEASIGNATE_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_NEASIGNATE_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.DOSARE_NEASIGNATE_CASCO_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_NEASIGNATE_CASCO_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.DOSARE_NEASIGNATE_RCA_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_NEASIGNATE_RCA_FROM_LAST_LOGIN"]); }
                catch { }

                // -- pt. All --
                try { this.DOSARE_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.DOSARE_CASCO_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_CASCO_FROM_LAST_LOGIN"]); }
                catch { }
                try { this.DOSARE_RCA_FROM_LAST_LOGIN = Convert.ToInt32(dj["DOSARE_RCA_FROM_LAST_LOGIN"]); }
                catch { }

                try { this.DOSARE_NEOPERATE = Convert.ToInt32(dj["DOSARE_NEOPERATE"]); }
                catch { this.DOSARE_NEOPERATE = 0; }
                try { this.DOSARE_CASCO_NEOPERATE = Convert.ToInt32(dj["DOSARE_CASCO_NEOPERATE"]); }
                catch { this.DOSARE_CASCO_NEOPERATE = 0; }
                try { this.DOSARE_RCA_NEOPERATE = Convert.ToInt32(dj["DOSARE_RCA_NEOPERATE"]); }
                catch { this.DOSARE_RCA_NEOPERATE = 0; }

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

    public class DashBoardView
    {
        public DosarExtended[] DosareExtended { get; set; }
        public UtilizatorExtended[] UtilizatoriExtended { get; set; }

        public DashBoardView() { }

        public DashBoardView(Utilizator utilizator, string conStr, int ID_SOCIETATE, int _selectType)
        {
            try
            {
                //Dosar[] ds = (Dosar[])utilizator.GetDosareNoi(ID_SOCIETATE).Result;
                Dosar[] ds = null;
                switch (_selectType)
                {
                    case 1:
                        ds = (Dosar[])utilizator.GetDosareNeasignate(ID_SOCIETATE).Result;

                        Utilizator[] us = (Utilizator[])utilizator.GetUtilizatoriSubordonati().Result;
                        List<UtilizatorExtended> ues = new List<UtilizatorExtended>(us.Length);
                        foreach (Utilizator u in us)
                        {
                            UtilizatorExtended ue = new UtilizatorExtended(u);
                            ues.Add(ue);
                        }
                        this.UtilizatoriExtended = ues.ToArray();

                        break;
                    case 2:
                        ds = (Dosar[])utilizator.GetDosareNeoperate(ID_SOCIETATE).Result;
                        break;
                }
                List<DosarExtended> des = new List<DosarExtended>(ds.Length);
                foreach (Dosar d in ds)
                {
                    DosarExtended de = new DosarExtended(d);
                    des.Add(de);
                }
                this.DosareExtended = des.ToArray();
            }
            catch { }
        }
    }
}