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
    }

    public class MesajJson
    {
        public Mesaj Mesaj { get; set; }
        public Utilizator[] Receivers { get; set; }
        public Utilizator Sender { get; set; }

        public MesajJson()
        {

        }

        public MesajJson(Mesaj m, Utilizator s, Utilizator[] us)
        {
            this.Mesaj = m;
            this.Sender = s;
            this.Receivers = us;
        }
    }

}