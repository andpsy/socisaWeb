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

        public MesajView() {
            /*
            this.Mesaj = new Mesaj();
            this.Receivers = new List<Utilizator>().ToArray();
            this.InvolvedParties = new List<Utilizator>().ToArray();
            this.Mesaje = new List<Mesaj>().ToArray();
            this.TipuriMesaj = new List<Nomenclator>().ToArray();
            */
        }
    }

    public class MesajJson
    {
        public Mesaj Mesaj { get; set; }
        public Utilizator[] Receivers { get; set; }

        public MesajJson()
        {

        }

        public MesajJson(Mesaj m, Utilizator[] us)
        {
            this.Mesaj = m;
            this.Receivers = us;
        }
    }

}