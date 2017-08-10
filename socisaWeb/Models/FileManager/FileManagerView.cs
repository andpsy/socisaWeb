using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SOCISA;
using SOCISA.Models;

namespace socisaWeb
{
    public class OrphanFile
    {
        public string FILE_NAME;
        public bool SELECTED;
    }

    public class OrphanDocument
    {
        public DocumentScanat DOCUMENT_SCANAT;
        public bool SELECTED;
    }
    public class FileManagerView
    {
        public OrphanFile[] orphanFiles;
        public OrphanDocument[] orphanDocuments;
    }
}