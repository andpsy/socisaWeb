using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOCISA;
using SOCISA.Models;

namespace socisaWeb
{
    public class ImportDosarView
    {
        public ImportDosarJson[] ImportDosareJson { get; set; }
        public string[] ImportDates { get; set; }
        public SocietateAsigurare[] SocietatiRCA { get; set; }

        public ImportDosarView() {
            ImportDosareJson = new List<ImportDosarJson>().ToArray();
            ImportDates = new List<string>().ToArray();
            SocietatiRCA = new List<SocietateAsigurare>().ToArray();
        }

        public ImportDosarView(int CURENT_USER_ID, string conStr)
        {
            DosareRepository dr = new DosareRepository(CURENT_USER_ID, conStr);
            ImportDates = ((List<string>)dr.GetImportDates().Result).ToArray();
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(CURENT_USER_ID, conStr);
            this.SocietatiRCA = (SocietateAsigurare[])sar.GetAll().Result;
        }
    }

    public class ImportDosarJson
    {
        //public bool selected { get; set; } -- il avem in DosarExtended
        public response response { get; set; }
        public DosarExtended DosarExtended { get; set; }

        public ImportDosarJson() { }
    }
}