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
        public Mesaj Mesaj { get; set; }
        [Required(ErrorMessage = "Campul \"Catre\" este obligatoriu!")]
        public string Receivers { get; set; }
        public Utilizator[] InvolvedParties { get; set; }
        public Mesaj[] Mesaje { get; set; }

        public MesajView() {
            this.Mesaj = new Mesaj();
            this.Receivers = "";
            this.InvolvedParties = new List<Utilizator>().ToArray();
            this.Mesaje = new List<Mesaj>().ToArray();
        }
    }
}