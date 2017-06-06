using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOCISA;
using SOCISA.Models;
using System.ComponentModel.DataAnnotations;

namespace socisaWeb
{
    public class MesajView
    {
        public MesajJson MesajJson { get; set; }
        public Utilizator[] InvolvedParties { get; set; }
        public MesajJson[] MesajeJson { get; set; }
        public Nomenclator[] TipuriMesaj { get; set; }

        public MesajView()
        {
            this.MesajJson = new MesajJson();
            MesajJson[] mesaje = new List<MesajJson>().ToArray();
            this.MesajeJson = mesaje;
            Utilizator[] us = new List<Utilizator>().ToArray();
            this.InvolvedParties = us;
            Nomenclator[] ns = new List<Nomenclator>().ToArray();
            this.TipuriMesaj = ns;
        }

        public MesajView(int uid, string conStr)
        {
            this.MesajJson = new MesajJson();
            Utilizator[] us = new List<Utilizator>().ToArray();
            this.InvolvedParties = us;
            Nomenclator[] ns = (Nomenclator[])(new NomenclatoareRepository(uid, conStr).GetAll("tip_mesaje")).Result;
            this.TipuriMesaj = ns;
            Utilizator u = new Utilizator(uid, conStr, uid);
            Mesaj[] ms = (Mesaj[])u.GetMesaje().Result;
            List<MesajJson> lst = new List<MesajJson>();
            foreach (Mesaj m in ms)
            {
                MesajJson mj = new MesajJson(m);
                mj.DataCitire = (DateTime?)m.GetMessageReadDate(uid).Result;
                lst.Add(mj);
            }
            this.MesajeJson = lst.ToArray();
        }
    }

    public class MesajJson : MesajExtended
    {
        /*
        public Mesaj Mesaj { get; set; }
        public Utilizator[] Receivers { get; set; }
        public Utilizator Sender { get; set; }
        public Nomenclator TipMesaj { get; set; }
        */
        public DateTime? DataCitire { get; set; }

        public MesajJson()
        {

        }

        public MesajJson(Mesaj m)
        {
            MesajExtended me = new MesajExtended(m);
            this.Mesaj = m;
            this.Dosar = me.Dosar;
            this.Sender = me.Sender;
            this.TipMesaj = me.TipMesaj;
            this.Receivers = me.Receivers;
            this.selected = me.selected;
        }

        public MesajJson(Mesaj m, Dosar dos, Utilizator s, Utilizator[] us, Nomenclator n, DateTime? d)
        {
            this.Mesaj = m;
            this.Dosar = dos;
            this.Sender = s;
            this.Receivers = us;
            this.TipMesaj = n;
            this.DataCitire = d;
        }
    }
}