using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace socisaWeb
{
    public class DashboardJson
    {
        [Display(Name = "Dosare in baza de date")]
        public int DOSARE_TOTAL { get; set; }
        [Display(Name = "Dosare noi de la ultimul login")]
        public int DOSARE_FROM_LAST_LOGIN { get; set; }
        [Display(Name = "Mesaje noi")]
        public int MESAJE_NOI { get; set; }
    }
}