using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOCISA;
using SOCISA.Models;

namespace socisaWeb
{
    public class DocumentView
    {
        public Int32 ID_DOSAR { get; set; }
        public TipDocument[] TipuriDocumente { get; set; }
        public DocumentScanat[] DocumenteScanate { get; set; }
        public DocumentScanat CurDocumentScanat { get; set; }

        public DocumentView() {
            //this.TipuriDocumente = new List<TipDocument>().ToArray();
        }

        public DocumentView(int _CURENT_USER_ID, string conStr)
        {
            TipDocumenteRepository tdr = new TipDocumenteRepository(_CURENT_USER_ID, conStr);
            this.TipuriDocumente = (TipDocument[])tdr.GetAll().Result;
        }

        public DocumentView(int _CURENT_USER_ID, int _ID_DOSAR, string conStr)
        {
            this.ID_DOSAR = _ID_DOSAR;
            TipDocumenteRepository tdr = new TipDocumenteRepository(_CURENT_USER_ID, conStr);
            this.TipuriDocumente = (TipDocument[])tdr.GetAll().Result;
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            Dosar d = (Dosar)dr.Find(_ID_DOSAR).Result;
            this.DocumenteScanate = (DocumentScanat[])d.GetDocumente().Result;
        }
    }
}