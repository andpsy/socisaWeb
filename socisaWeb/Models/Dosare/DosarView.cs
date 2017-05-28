using SOCISA.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace socisaWeb
{
    public class DosarView
    {
        public Dosar Dosar { get; set; }
        public DosarJson dosarJson { get; set; }
        public Dosar[] DosareResult { get; set; }
        public SocietateAsigurare[] SocietatiCASCO { get; set; }
        public SocietateAsigurare[] SocietatiRCA { get; set; }
        public Nomenclator[] TipuriCaz { get; set; }
        public Nomenclator[] TipuriDosar { get; set; }

        public DosarView() { }

        public DosarView(int _CURENT_USER_ID, int _ID_SOCIETATE, string conStr)
        {
            this.Dosar = new Dosar(_CURENT_USER_ID, conStr);
            this.Dosar.ID_SOCIETATE_CASCO = _ID_SOCIETATE;
            this.dosarJson = new DosarJson(_CURENT_USER_ID, _ID_SOCIETATE, conStr);
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(_CURENT_USER_ID, conStr);
            this.SocietatiCASCO = this.SocietatiRCA = (SocietateAsigurare[])sar.GetAll().Result;
            NomenclatoareRepository nr = new NomenclatoareRepository(_CURENT_USER_ID, conStr);
            this.TipuriCaz = (Nomenclator[])nr.GetAll("tip_caz").Result;
            this.TipuriDosar = (Nomenclator[])nr.GetAll("tip_dosare").Result;
        }

        public DosarView(int _CURENT_USER_ID, int _ID_SOCIETATE, Dosar dosar, string conStr)
        {
            this.Dosar = dosar;
            this.dosarJson = new DosarJson(_CURENT_USER_ID, _ID_SOCIETATE, conStr);
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(_CURENT_USER_ID, conStr);
            this.SocietatiCASCO = this.SocietatiRCA = (SocietateAsigurare[])sar.GetAll().Result;
            NomenclatoareRepository nr = new NomenclatoareRepository(_CURENT_USER_ID, conStr);
            this.TipuriCaz = (Nomenclator[])nr.GetAll("tip_caz").Result;
            this.TipuriDosar = (Nomenclator[])nr.GetAll("tip_dosare").Result;
        }
    }

    public class DosarJson
    {
        public string NumeAsiguratCasco { get; set; }
        public string NumeAsiguratRca { get; set; }
        public string NumarAutoCasco { get; set; }
        public string NumarAutoRca { get; set; }
        public string NumeIntervenient { get; set; }
        //public string TipDosar { get; set; }
        [Display(Name = "Data Eveniment de la:")]
        public DateTime? DataEvenimentStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataEvenimentEnd { get; set; }
        [Display(Name = "Data SCA de la:")]
        public DateTime? DataScaStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataScaEnd { get; set; }
        [Display(Name = "Data Iesire CASCO de la:")]
        public DateTime? DataIesireCascoStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataIesireCascoEnd { get; set; }
        [Display(Name = "Data Intrare RCA de la:")]
        public DateTime? DataIntrareRcaStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataIntrareRcaEnd { get; set; }
        [Display(Name = "Data Avizare de la:")]
        public DateTime? DataAvizareStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataAvizareEnd { get; set; }
        [Display(Name = "Data Notificare de la:")]
        public DateTime? DataNotificareStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataNotificareEnd { get; set; }

        [Display(Name = "Data Creare de la:")]
        public DateTime? DataCreareStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataCreareEnd { get; set; }
    
        [Display(Name = "Data Ultimei Modificari de la:")]
        public DateTime? DataUltimeiModificariStart { get; set; }
        [Display(Name = "pana la:")]
        public DateTime? DataUltimeiModificariEnd { get; set; }

        public DosarJson() { }

        public DosarJson(int _CURENT_USER_ID, int _ID_SOCIETATE, string conStr) {}
    }
}