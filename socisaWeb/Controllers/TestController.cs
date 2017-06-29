using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using socisaWeb.ServiceReference1;
using SOCISA;
using SOCISA.Models;
using System.Configuration;

namespace socisaWeb.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Test()
        {
            response r = new response();
            try
            {
                int uid = Convert.ToInt32(Session["CURENT_USER_ID"]);
                string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
                SubrogationServiceClient s = new SubrogationServiceClient();
                s.ClientCredentials.UserName.UserName = "Drift Data";
                s.ClientCredentials.UserName.Password = "driftdata.1";
                SubrogationInfo[] sis = s.BrowseUnreadSubrogations(new DateTime(2017, 1, 1), new DateTime(2017, 6, 15));
                foreach (SubrogationInfo si in sis)
                {
                    try
                    {
                        Dosar d = new Dosar(uid, conStr);
                        try
                        {
                            d.ID_SOCIETATE_CASCO = Convert.ToInt32(Session["ID_SOCIETATE"]); // TO DO: a lista cu ID-urile societatilor care au servicii expuse
                        }
                        catch { }
                        try
                        {
                            SocietateAsigurare sRca = new SocietateAsigurare(uid, conStr, si.SubrogationInsurerName, false);
                            if(sRca == null || sRca.ID == null)
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
                        try { d.SUMA_IBNR = d.VALOARE_DAUNA = d.VALOARE_REGRES = d.VMD = d.REZERVA_DAUNA = Convert.ToDouble( si.ClaimReserveValueRon); } catch { }

                        r = d.Validare();
                        if (r.Status)
                        {
                            r = d.Insert();

                            ClaimDocumentSummary[] cdss = s.BrowseClaimDocuments(si.ClaimId, new DateTime(2017, 1, 1));
                            foreach(ClaimDocumentSummary cds in cdss)
                            {
                                //TO DO: De matchuit Category de la ei cu Tip document de la noi
                                TipDocument td = new TipDocument(uid, conStr, cds.CategoryName);

                                DocumentScanat ds = new DocumentScanat(uid, conStr);
                                ds.DENUMIRE_FISIER = cds.Name;
                                ds.EXTENSIE_FISIER = cds.Extension;
                                ds.ID_TIP_DOCUMENT =  Convert.ToInt32(td.ID);
                                ds.ID_DOSAR = Convert.ToInt32(d.ID);
                                ds.CALE_FISIER = cds.FileName;
                                
                                BinaryContent bc = s.GetClaimDocumentDetails(cds.Id);
                                ds.FILE_CONTENT = bc.BinaryData;

                                r = ds.Validare();
                                if (r.Status)
                                {
                                    ds.Insert();
                                }
                            }
                        }
                        else
                        {
                            d.InsertWithErrors();
                        }
                    }
                    catch (Exception exp) { LogWriter.Log(exp); }
                    finally
                    {
                        s.MarkAsReadByClaimId(si.ClaimId);
                    }                       
                }
            }catch(Exception exp) {
                LogWriter.Log(exp);
            }
            return View();
        }
    }
}