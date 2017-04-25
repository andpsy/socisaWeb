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
        //public TipDocument[] TipuriDocumente { get; set; }
        //public DocumentScanat[] DocumenteScanate { get; set; }
        public TipDocumentJson[] TipuriDocumente { get; set; }
        public DocumentScanat CurDocumentScanat { get; set; }

        public DocumentView() {}

        public DocumentView(int _CURENT_USER_ID, string conStr)
        {
            TipDocumenteRepository tdr = new TipDocumenteRepository(_CURENT_USER_ID, conStr);
            //this.TipuriDocumente = (TipDocument[])tdr.GetAll().Result;
            List<TipDocumentJson> l = new List<TipDocumentJson>();
            TipDocument[] tipuriDocumente = (TipDocument[])tdr.GetAll().Result;
            foreach(TipDocument td in tipuriDocumente)
            {
                l.Add(new TipDocumentJson(td, null));
            }
            this.TipuriDocumente = l.ToArray();
            //this.CurDocumentScanat = new DocumentScanat();
        }

        public DocumentView(int _CURENT_USER_ID, int _ID_DOSAR, string conStr)
        {
            this.ID_DOSAR = _ID_DOSAR;
            TipDocumenteRepository tdr = new TipDocumenteRepository(_CURENT_USER_ID, conStr);
            //this.TipuriDocumente = (TipDocument[])tdr.GetAll().Result;
            List<TipDocumentJson> l = new List<TipDocumentJson>();
            TipDocument[] tipuriDocumente = (TipDocument[])tdr.GetAll().Result;

            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            Dosar d = (Dosar)dr.Find(_ID_DOSAR).Result;
            //this.DocumenteScanate = (DocumentScanat[])d.GetDocumente().Result;
            DocumentScanat[] dss = (DocumentScanat[])d.GetDocumente().Result;
            foreach (TipDocument td in tipuriDocumente)
            {
                List<DocumentScanat> ld = new List<DocumentScanat>();
                foreach(DocumentScanat ds in dss)
                {
                    if(ds.ID_TIP_DOCUMENT == td.ID)
                    {
                        ld.Add(ds);
                    }
                }
                l.Add(new TipDocumentJson(td, ld.ToArray()));
            }
            this.TipuriDocumente = l.ToArray();
            //this.CurDocumentScanat = new DocumentScanat();
        }
    }

    public class TipDocumentJson
    {
        public TipDocument TipDocument { get; set; }
        public DocumentScanat[] DocumenteScanate { get; set; }

        public TipDocumentJson() { }

        public TipDocumentJson(TipDocument td, DocumentScanat[] dss)
        {
            this.TipDocument = td;
            this.DocumenteScanate = dss;
        }
    }
}