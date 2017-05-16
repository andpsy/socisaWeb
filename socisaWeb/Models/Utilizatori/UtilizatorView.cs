using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOCISA;
using SOCISA.Models;
using System.Reflection;

namespace socisaWeb
{
    public class UtilizatorView
    {
        public UtilizatorJson UtilizatorJson { get; set; }
        public SocietateAsigurareExtended[] SocietatiAsigurare { get; set; }
        public DreptExtended[] Drepturi { get; set; }
        public ActionExtended[] Actions { get; set; }
        public Nomenclator[] TipuriUtilizator { get; set; }
        public SocietateAsigurareExtended[] SocietatiAsigurareAdministrate { get; set; }

        public UtilizatorView() { }

        public UtilizatorView(int CURENT_USER_ID, string conStr)
        {
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(CURENT_USER_ID, conStr);
            SocietatiAsigurare = SocietatiAsigurareAdministrate = GetFromBase((SocietateAsigurare[])sar.GetAll().Result);

            DrepturiRepository dr = new DrepturiRepository(CURENT_USER_ID, conStr);
            Drepturi = GetFromBase((Drept[])dr.GetAll().Result);

            ActionsRepository ar = new ActionsRepository(CURENT_USER_ID, conStr);
            Actions = GetFromBase((SOCISA.Models.Action[])ar.GetAll().Result);

            NomenclatoareRepository nr = new NomenclatoareRepository(CURENT_USER_ID, conStr);
            TipuriUtilizator = (Nomenclator[])nr.GetAll("tip_utilizatori").Result;

            UtilizatorJson = new UtilizatorJson(CURENT_USER_ID, conStr, CURENT_USER_ID);
        }

        SocietateAsigurareExtended[] GetFromBase(SocietateAsigurare[] baze)
        {
            List<SocietateAsigurareExtended> toReturn = new List<SocietateAsigurareExtended>();
            foreach(SocietateAsigurare baza in baze)
            {
                toReturn.Add(new SocietateAsigurareExtended(baza));
            }
            return toReturn.ToArray();
        }

        DreptExtended[] GetFromBase(Drept[] baze)
        {
            List<DreptExtended> toReturn = new List<DreptExtended>();
            foreach (Drept baza in baze)
            {
                toReturn.Add(new DreptExtended(baza));
            }
            return toReturn.ToArray();
        }

        ActionExtended[] GetFromBase(SOCISA.Models.Action[] baze)
        {
            List<ActionExtended> toReturn = new List<ActionExtended>();
            foreach (SOCISA.Models.Action baza in baze)
            {
                toReturn.Add(new ActionExtended(baza));
            }
            return toReturn.ToArray();
        }
    }

    public class SocietateAsigurareExtended : SocietateAsigurare
    {
        public bool selected;

        public SocietateAsigurareExtended() { }

        public SocietateAsigurareExtended(SocietateAsigurare baza)
        {
            PropertyInfo[] pis = baza.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(this, pi.GetValue(baza));
            }
        }
    }

    public class DreptExtended : Drept
    {
        public bool selected;

        public DreptExtended() { }

        public DreptExtended(Drept baza)
        {
            PropertyInfo[] pis = baza.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(this, pi.GetValue(baza));
            }
        }
    }

    public class ActionExtended : SOCISA.Models.Action
    {
        public bool selected;

        public ActionExtended() { }

        public ActionExtended(SOCISA.Models.Action baza)
        {
            PropertyInfo[] pis = baza.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(this, pi.GetValue(baza));
            }
        }
    }

    public class UtilizatorJson
    {
        public Utilizator Utilizator { get; set; }
        public UtilizatorJson[] UtilizatoriSubordonati { get; set; }
        public SocietateAsigurare SocietateAsigurare { get; set; }
        public Drept[] Drepturi { get; set; }
        public SOCISA.Models.Action[] Actions { get; set; }
        public Nomenclator TipUtilizator { get; set; }
        public SocietateAsigurare[] SocietatiAsigurareAdministrate { get; set; }

        public UtilizatorJson() { }

        public UtilizatorJson (int CURENT_USER_ID, string conStr, int ID_UTILIZATOR)
        {
            UtilizatoriRepository ur = new UtilizatoriRepository(CURENT_USER_ID, conStr);
            Utilizator u = (Utilizator)ur.Find(ID_UTILIZATOR).Result;

            Utilizator = u;
            SocietateAsigurare = (SocietateAsigurare)u.GetSocietatiAsigurare().Result;
            SocietatiAsigurareAdministrate = (SocietateAsigurare[])u.GetSocietatiAdministrate().Result;
            Drepturi = (Drept[])u.GetDrepturi().Result;
            Actions = (SOCISA.Models.Action[])u.GetActions().Result;
            TipUtilizator = (Nomenclator)u.GetTipUtilizator().Result;

            Utilizator[] us = (Utilizator[])u.GetUtilizatoriSubordonati().Result;
            List<UtilizatorJson> l = new List<UtilizatorJson>();
            foreach(Utilizator uu in us)
            {
                if(uu.ID != ID_UTILIZATOR && uu.ID != CURENT_USER_ID)
                    l.Add(new UtilizatorJson(CURENT_USER_ID, conStr, Convert.ToInt32(uu.ID)));
            }

            UtilizatoriSubordonati = l.ToArray();
        }
    }
}