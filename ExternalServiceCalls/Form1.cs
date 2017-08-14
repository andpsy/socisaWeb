using System;
using System.Windows.Forms;
using ExternalServiceCalls.ServiceReference1;
using SOCISA;
using SOCISA.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.ServiceModel;
using System.Configuration;

namespace ExternalServiceCalls
{
    public partial class Form1 : Form
    {
        public DataTable SocietatiMappings = new DataTable();
        public DataTable TipuriDocumentMappings = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SocietatiMappings = GetSocietatiMappings("Allianz");
            this.TipuriDocumentMappings = GetTipuriDocumentMappings("Allianz");
            CallService();
        }

        private void CallService()
        { 
            response r = new response();
            try
            {
                int uid = 1;
                List<string> conStrs = new List<string>();
                //conStrs.Add(Properties.Settings.Default["MySQLConnectionString"].ToString());
                //conStrs.Add(Properties.Settings.Default["TestMySQLConnectionString_test"].ToString());
                conStrs.Add(ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ToString());
                conStrs.Add(ConfigurationManager.ConnectionStrings["MySqlConnectionString_test"].ToString());
                /*
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxBufferPoolSize = binding.MaxBufferSize = Int32.MaxValue;
                binding.MaxReceivedMessageSize = long.MaxValue;
                */
                SubrogationServiceClient s = new SubrogationServiceClient();
                /*
                (s.ChannelFactory.Endpoint.Binding as BasicHttpBinding).MaxBufferPoolSize = Int32.MaxValue;
                (s.ChannelFactory.Endpoint.Binding as BasicHttpBinding).MaxBufferSize = Int32.MaxValue;
                (s.ChannelFactory.Endpoint.Binding as BasicHttpBinding).MaxReceivedMessageSize = Int32.MaxValue;
                */

                //s.ClientCredentials.UserName.UserName = Properties.Settings.Default["AllianzWSUser"].ToString();
                //s.ClientCredentials.UserName.Password = Properties.Settings.Default["AllianzWSPassword"].ToString();
                s.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["AllianzWSUser"].ToString();
                s.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["AllianzWSPassword"].ToString();
                s.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                
                SubrogationInfo[] sis = s.BrowseUnreadSubrogations(new DateTime(2017, 8, 1), DateTime.Now);
                int counter = 0;
                foreach (SubrogationInfo si in sis)
                {
                    //if (counter > 2) return;
                    foreach (string conStr in conStrs) // salvam in ambele baze !
                    {
                        try
                        {
                            Dosar d = new Dosar(uid, conStr);
                            try
                            {
                                SocietateAsigurare sCasco = new SocietateAsigurare(uid, conStr, "ALLIANZ", true);
                                d.ID_SOCIETATE_CASCO = sCasco.ID;
                            }
                            catch { }
                            try
                            {
                                //SocietateAsigurare sRca = new SocietateAsigurare(uid, conStr, si.SubrogationInsurerName, false);
                                SocietateAsigurare sRca = null;
                                int? id_soc = GetSocietateMapping(this.SocietatiMappings, si.SubrogationInsurerId);
                                if (id_soc != null && id_soc != 0)
                                {
                                    sRca = new SocietateAsigurare(uid, conStr, Convert.ToInt32(id_soc));
                                }
                                if (sRca == null || sRca.ID == null)
                                {
                                    sRca = new SocietateAsigurare(uid, conStr);
                                    sRca.DENUMIRE = sRca.DENUMIRE_SCURTA = si.SubrogationInsurerName;
                                    sRca.ADRESA = si.SubrogationInsurerCountry;
                                    r = sRca.Insert();
                                }
                                d.ID_SOCIETATE_RCA = sRca.ID;
                            }
                            catch { }

                            try
                            {
                                Asigurat aCasco = new Asigurat(uid, conStr, si.InsuredFullName);
                                if (aCasco == null || aCasco.ID == null)
                                {
                                    aCasco = new Asigurat(uid, conStr);
                                    aCasco.DENUMIRE = si.InsuredFullName;
                                    r = aCasco.Insert();
                                }
                                d.ID_ASIGURAT_CASCO = aCasco.ID;
                            }
                            catch { }

                            try
                            {
                                Asigurat aRca = new Asigurat(uid, conStr, si.SubrogationGuiltyPartner);
                                if (aRca == null || aRca.ID == null)
                                {
                                    aRca = new Asigurat(uid, conStr);
                                    aRca.DENUMIRE = si.SubrogationGuiltyPartner;
                                    r = aRca.Insert();
                                }
                                d.ID_ASIGURAT_RCA = aRca.ID;
                            }
                            catch { }

                            try
                            {
                                Auto autoCasco = new Auto(uid, conStr, si.InsuredCarPlateNo);
                                if (autoCasco == null || autoCasco.ID == null)
                                {
                                    autoCasco = new Auto(uid, conStr);
                                    autoCasco.NR_AUTO = si.InsuredCarPlateNo;
                                    autoCasco.MARCA = si.InsuredCarBrandName;
                                    autoCasco.MODEL = si.InsuredCarModelName;
                                    autoCasco.SERIE_SASIU = si.InsuredCarChassisNo;
                                    r = autoCasco.Insert();
                                }
                                d.ID_AUTO_CASCO = autoCasco.ID;
                            }
                            catch { }

                            try
                            {
                                Auto autoRca = new Auto(uid, conStr, si.SubrogationCarPlateNo);
                                if (autoRca == null || autoRca.ID == null)
                                {
                                    autoRca = new Auto(uid, conStr);
                                    autoRca.NR_AUTO = si.SubrogationCarPlateNo;
                                    //autoRca.MARCA = si.SubrogationCarBrandName;
                                    //autoRca.MODEL = si.SubrogationCarModelName;
                                    autoRca.SERIE_SASIU = si.SubrogationCarChassisNo;
                                    r = autoRca.Insert();
                                }
                                d.ID_AUTO_RCA = autoRca.ID;
                            }
                            catch { }

                            try { d.CAZ = si.AmiableAssessmentNo; } catch { }
                            try { d.DATA_CREARE = d.DATA_ULTIMEI_MODIFICARI = DateTime.Now; } catch { }
                            try { d.DATA_EVENIMENT = si.LossDate; } catch { }
                            try { d.DATA_NOTIFICARE = si.AssessmentDate; } catch { } //?? - la ei este Data Constatarii, nu cred ca e tot una cu Data Notificarii de la noi
                            try { d.NR_DOSAR_CASCO = si.ClaimFileNo; } catch { }
                            try { d.NR_POLITA_CASCO = si.InsuredPolicyNo; } catch { }
                            try { d.NR_POLITA_RCA = si.SubrogationRcoPolicyNo; } catch { }
                            try { d.SUMA_IBNR = d.VALOARE_DAUNA = d.VALOARE_REGRES = d.VMD = d.REZERVA_DAUNA = Convert.ToDouble(si.ClaimReserveValueRon); } catch { }

                            r = d.Validare();
                            if (r.Status)
                            {
                                r = d.Insert();
                                d.Log(r, 1);  // 1 = automatic import
                                if (r.Status && r.InsertedId != null)
                                {
                                    ClaimDocumentSummary[] cdss = s.BrowseClaimDocuments(si.ClaimId, new DateTime(2017, 8, 1));
                                    foreach (ClaimDocumentSummary cds in cdss)
                                    {
                                        try
                                        {
                                            //TipDocument td = new TipDocument(uid, conStr, cds.CategoryName);
                                            TipDocument td = null;
                                            int? id_tip_doc = GetTipDocumentMapping(this.TipuriDocumentMappings, cds.CategoryId);
                                            if (id_tip_doc != null && id_tip_doc != 0)
                                            {
                                                td = new TipDocument(uid, conStr, Convert.ToInt32(id_tip_doc));
                                            }

                                            DocumentScanat ds = new DocumentScanat(uid, conStr);
                                            ds.DENUMIRE_FISIER = String.IsNullOrWhiteSpace(cds.Name) ? cds.FileName : cds.Name;
                                            ds.EXTENSIE_FISIER = !String.IsNullOrWhiteSpace(cds.Extension) ? cds.Extension : !String.IsNullOrWhiteSpace(cds.Name) ? cds.Name.Substring(cds.Name.LastIndexOf('.')) : cds.FileName.Substring(cds.FileName.LastIndexOf('.'));
                                            ds.ID_TIP_DOCUMENT = Convert.ToInt32(td.ID);
                                            ds.ID_DOSAR = Convert.ToInt32(d.ID);

                                            BinaryContent bc = s.GetClaimDocumentDetails(cds.Id);
                                            ds.FILE_CONTENT = bc.BinaryData;

                                            ds.CALE_FISIER = FileManager.SaveBinaryContentToFile(bc.BinaryData, ds.EXTENSIE_FISIER);

                                            response rd = ds.Validare();
                                            if (rd.Status)
                                            {
                                                rd = ds.Insert();
                                                ds.Log(rd, 1);  // 1 = automatic import
                                            }
                                            if (!rd.Status) // marcam dosarul ca citit (preluat) de la Allianz doar daca s-a reusit preluarea tuturor documentelor asociate
                                                r.AddResponse(rd);
                                        }
                                        catch(Exception exp) { LogWriter.Log(exp); }
                                    }

                                    if (r.Status)
                                    {
                                        //s.MarkAsReadByClaimId(si.ClaimId);
                                    }
                                }
                            }
                            else
                            {
                                r = d.InsertWithErrors();
                                d.Log(r, 1); // 1 = automatic import
                                if (r.Status && r.InsertedId != null)
                                {
                                    ClaimDocumentSummary[] cdss = s.BrowseClaimDocuments(si.ClaimId, new DateTime(2017, 8, 1));
                                    foreach (ClaimDocumentSummary cds in cdss)
                                    {
                                        try
                                        {
                                            //TipDocument td = new TipDocument(uid, conStr, cds.CategoryName);
                                            TipDocument td = null;
                                            int? id_tip_doc = GetTipDocumentMapping(this.TipuriDocumentMappings, cds.CategoryId);
                                            if (id_tip_doc != null && id_tip_doc != 0)
                                            {
                                                td = new TipDocument(uid, conStr, Convert.ToInt32(id_tip_doc));
                                            }

                                            DocumentScanat ds = new DocumentScanat(uid, conStr);
                                            ds.DENUMIRE_FISIER = String.IsNullOrWhiteSpace(cds.Name) ? cds.FileName : cds.Name;
                                            ds.EXTENSIE_FISIER = !String.IsNullOrWhiteSpace(cds.Extension) ? cds.Extension : !String.IsNullOrWhiteSpace(cds.Name) ? cds.Name.Substring(cds.Name.LastIndexOf('.')) : cds.FileName.Substring(cds.FileName.LastIndexOf('.'));
                                            ds.ID_TIP_DOCUMENT = Convert.ToInt32(td.ID);
                                            ds.ID_DOSAR = Convert.ToInt32(d.ID);

                                            BinaryContent bc = s.GetClaimDocumentDetails(cds.Id);
                                            ds.FILE_CONTENT = bc.BinaryData;

                                            ds.CALE_FISIER = FileManager.SaveBinaryContentToFile(bc.BinaryData, ds.EXTENSIE_FISIER);

                                            response rd = ds.Validare();
                                            if (rd.Status)
                                            {
                                                rd = ds.InsertWithErrors();
                                                ds.Log(rd, 1); // 1 = automatic import
                                            }
                                            if (!rd.Status) // marcam dosarul ca citit (preluat) de la Allianz doar daca s-a reusit preluarea tuturor documentelor asociate
                                                r.AddResponse(rd);
                                        }
                                        catch(Exception exp) { LogWriter.Log(exp); }
                                    }

                                    if (r.Status)
                                    {
                                        //s.MarkAsReadByClaimId(si.ClaimId);
                                    }
                                }
                            }
                        }
                        catch (Exception exp) { LogWriter.Log(exp); }
                        counter++;
                    }
                }
            }
            catch (Exception exp)
            {
                LogWriter.Log(exp);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CallService();
        }

        private DataTable GetSocietatiMappings(string _DENUMIRE_SOCIETATE)
        {
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            string file_name = String.Format("{0}SocietatiMappings.xml", ti.ToTitleCase(_DENUMIRE_SOCIETATE));
            DataSet soc = new DataSet("SOCIETATI");
            soc.ReadXml(file_name);
            return soc.Tables[0];
        }

        private int? GetSocietateMapping(DataTable _MAPPINGS, int? _ID_SOCIETATE)
        {
            try
            {
                DataRow dr = _MAPPINGS.Select(String.Format("ID_SOCIETATE = '{0}'", _ID_SOCIETATE))[0];
                return Convert.ToInt32(dr["ID_SCA"]);
            }catch { return null; }
        }

        private DataTable GetTipuriDocumentMappings(string _DENUMIRE_SOCIETATE)
        {
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            string file_name = String.Format("{0}TipuriDocumentMappings.xml", ti.ToTitleCase(_DENUMIRE_SOCIETATE));
            DataSet tipuri_document = new DataSet("TIPURI_DOCUMENT");
            tipuri_document.ReadXml(file_name);
            return tipuri_document.Tables[0];
        }

        private int? GetTipDocumentMapping(DataTable _TIPURI_DOCUMENT, int? _ID_TIP_DOCUMENT_SOCIETATE)
        {
            try
            {
                DataRow dr = _TIPURI_DOCUMENT.Select(String.Format("ID_TIP_DOCUMENT_SOCIETATE LIKE '{0},%' OR ID_TIP_DOCUMENT_SOCIETATE LIKE '%,{0}' OR ID_TIP_DOCUMENT_SOCIETATE LIKE '%,{0},%'", _ID_TIP_DOCUMENT_SOCIETATE.ToString()))[0];
                return Convert.ToInt32(dr["ID_TIP_DOCUMENT_SCA"]);
            }
            catch
            {
                try
                {
                    DataRow dr = _TIPURI_DOCUMENT.Select("ID_TIP_DOCUMENT_SOCIETATE LIKE '%,ALL'")[0];
                    return Convert.ToInt32(dr["ID_TIP_DOCUMENT_SCA"]);
                }
                catch
                {
                    return null;
                }
            }
        }

    }
}
