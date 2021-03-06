﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOCISA;
using SOCISA.Models;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Globalization;

namespace socisaWeb.Controllers
{
    [Authorize]
    public class DosareController : Controller
    {
        [AuthorizeUser(ActionName = "Import", Recursive = false)]
        public ActionResult Import()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            return PartialView("DosareImport", new ImportDosarView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr));
        }

        [AuthorizeUser(ActionName = "Import", Recursive = false)]
        [HttpPost]
        public JsonResult PostExcelFile()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            HttpPostedFileBase f = Request.Files[0];
            string initFName = f.FileName;
            string extension = f.FileName.Substring(f.FileName.LastIndexOf('.'));
            string newFName = Guid.NewGuid() + extension;
            Request.Files[0].SaveAs(System.IO.Path.Combine(CommonFunctions.GetImportsFolder(), newFName));
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            response r = dr.GetDosareFromExcel("Sheet1", newFName);
            bool societateDiferita = false;
            foreach(object[] o in (object[])r.Result)
            {
                if( ((DosarExtended)o[1]).SocietateCasco.DENUMIRE_SCURTA != ((SocietateAsigurare)Session["SOCIETATE_ASIGURARE"]).DENUMIRE_SCURTA) // se incearca incarcarea pt. alta societate decat cea a utilizatorului curent
                {
                    societateDiferita = true;
                    break;
                }
            }
            if (societateDiferita)
            {
                response toReturn = new response(false, String.Format("Nu puteti incarca dosare pentru alta societate decat cea curenta ({0})!", ((SocietateAsigurare)Session["SOCIETATE_ASIGURARE"]).DENUMIRE), null, null, null);
                return Json(toReturn, JsonRequestBehavior.AllowGet);
            }
            r = dr.ImportDosareDirect("Sheet1", newFName, 0); // 0 = import manual
            JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        [AuthorizeUser(ActionName = "Import", Recursive = false)]
        [HttpPost]
        public JsonResult GetDosareFromLog(DateTime ImportDate)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            response r = dr.GetDosareFromLog(ImportDate);
            JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        [AuthorizeUser(ActionName = "Import", Recursive = false)]
        [HttpPost]
        public JsonResult Import(ImportDosarView ImportDosarView)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public ActionResult Index(int? id)
        {
            return PartialView();
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpGet]
        public ActionResult Search()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            return PartialView("_DosareNavigator", new DosarView(Convert.ToInt32(Session["CURENT_USER_ID"]), Convert.ToInt32(Session["ID_SOCIETATE"]), conStr));
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpGet]
        public ActionResult Show(int id)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Dosar d = new Dosar(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr, id);
            // verificam daca are drept pe dosar 
            bool? hasWright = (bool?)d.UserHasWright(Convert.ToInt32(Session["CURENT_USER_ID"])).Result;
            if (hasWright == null || !(bool)hasWright)
            {
                HttpContext.Response.Redirect("~");
                return null;
            }

            return PartialView("_DosareNavigator", new DosarView(Convert.ToInt32(Session["CURENT_USER_ID"]), Convert.ToInt32(Session["ID_SOCIETATE"]), d, conStr));
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult Search(DosarView DosarView)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            //string jsonFilter = JsonConvert.SerializeObject(DosarView);
            JObject f = (JObject)JsonConvert.DeserializeObject(Request.Form[0]);
            JToken jDosar = f["Dosar"];
            JToken jdosarJson = f["dosarJson"];
            if (jdosarJson != null)
            {
                if (jDosar == null) jDosar = JToken.FromObject(new Dosar());

                if (jdosarJson["DataEvenimentStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataEvenimentStart"].ToString()) && jdosarJson["DataEvenimentEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataEvenimentEnd"].ToString()))
                {
                    jDosar["DATA_EVENIMENT"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataEvenimentStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataEvenimentEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataScaStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataScaStart"].ToString()) && jdosarJson["DataScaEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataScaEnd"].ToString()))
                {
                    jDosar["DATA_SCA"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataScaStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataScaEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataAvizareStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataAvizareStart"].ToString()) && jdosarJson["DataAvizareEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataAvizareEnd"].ToString()))
                {
                    jDosar["DATA_AVIZARE"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataAvizareStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataAvizareEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataNotificareStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataNotificareStart"].ToString()) && jdosarJson["DataNotificareEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataNotificareEnd"].ToString()))
                {
                    jDosar["DATA_NOTIFICARE"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataNotificareStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataNotificareEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataUltimeiModificariStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataUltimeiModificariStart"].ToString()) && jdosarJson["DataUltimeiModificariEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataUltimeiModificariEnd"].ToString()))
                {
                    jDosar["DATA_ULTIMEI_MODIFICARI"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataUltimeiModificariStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataUltimeiModificariEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataCreareStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataCreareStart"].ToString()) && jdosarJson["DataCreareEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataCreareEnd"].ToString()))
                {
                    jDosar["DATA_CREARE"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataCreareStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataCreareEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataIesireCascoStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataIesireCascoStart"].ToString()) && jdosarJson["DataIesireCascoEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataIesireCascoEnd"].ToString()))
                {
                    jDosar["DATA_IESIRE_CASCO"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIesireCascoStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIesireCascoEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataIntrareRcaStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataIntrareRcaStart"].ToString()) && jdosarJson["DataIntrareRcaEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataIntrareRcaEnd"].ToString()))
                {
                    jDosar["DATA_INTRARE_RCA"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIntrareRcaStart"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIntrareRcaEnd"].ToString(), CommonFunctions.DATE_FORMAT, CultureInfo.InvariantCulture));
                }
            }
            //response r = dr.GetFiltered(null, null, String.Format("{'jDosar':{0},'jdosarJson':{1}}", JsonConvert.SerializeObject(jDosar), JsonConvert.SerializeObject(jdosarJson)), null);
            string jsonParam = "{\"jDosar\":" + JsonConvert.SerializeObject(jDosar) + ",\"jdosarJson\":" + JsonConvert.SerializeObject(jdosarJson) + "}";
            response r = dr.GetFiltered(null, null, jsonParam, null);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public JsonResult Details(int id)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(id).Result;
            Asigurat aCasco = (Asigurat)d.GetAsiguratCasco().Result;
            Asigurat aRca = (Asigurat)d.GetAsiguratRca().Result;
            Auto autoCasco = (Auto)d.GetAutoCasco().Result;
            Auto autoRca = (Auto)d.GetAutoRca().Result;
            Intervenient i = (Intervenient)d.GetIntervenient().Result;
            Nomenclator tipDosar = (Nomenclator)d.GetTipDosar().Result;
            bool validForAvizare = d.ValidareAvizare().Status;
            string toReturn = "{\"aCasco\":" + JsonConvert.SerializeObject(aCasco) + ",\"aRca\":" + JsonConvert.SerializeObject(aRca) + ",\"autoCasco\":" + JsonConvert.SerializeObject(autoCasco) + ",\"autoRca\":" + JsonConvert.SerializeObject(autoRca) + ",\"intervenient\":" + JsonConvert.SerializeObject(i) + ",\"tipDosar\":" + JsonConvert.SerializeObject(tipDosar) + ",\"validForAvizare\":" + validForAvizare.ToString().ToLower() + "}";
            object j = JsonConvert.DeserializeObject(toReturn);
            return Json(toReturn, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult MovePendinToOk(DosarExtended dosar)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);

            Asigurat x = new Asigurat(_CURENT_USER_ID, conStr);
            PropertyInfo[] pis = dosar.AsiguratCasco.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(x, pi.GetValue(dosar.AsiguratCasco));
            }
            if (x.ID == null)
            {
                response r = x.Insert();
                x.ID = r.InsertedId;
            }else
            {
                x.Update();
            }
            dosar.Dosar.ID_ASIGURAT_CASCO = x.ID;


            x = new Asigurat(_CURENT_USER_ID, conStr);
            pis = dosar.AsiguratRca.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(x, pi.GetValue(dosar.AsiguratRca));
            }
            if (x.ID == null)
            {
                response r = x.Insert();
                x.ID = r.InsertedId;
            }
            else
            {
                x.Update();
            }
            dosar.Dosar.ID_ASIGURAT_RCA = x.ID;


            Auto y = new Auto(_CURENT_USER_ID, conStr);
            pis = dosar.AutoCasco.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(y, pi.GetValue(dosar.AutoCasco));
            }
            if (y.ID == null)
            {
                response r = y.Insert();
                y.ID = r.InsertedId;
            }
            else
            {
                y.Update();
            }
            dosar.Dosar.ID_AUTO_CASCO = y.ID;


            y = new Auto(_CURENT_USER_ID, conStr);
            pis = dosar.AutoRca.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(y, pi.GetValue(dosar.AutoRca));
            }
            if (y.ID == null)
            {
                response r = y.Insert();
                y.ID = r.InsertedId;
            }
            else
            {
                y.Update();
            }
            dosar.Dosar.ID_AUTO_RCA = y.ID;



            SocietateAsigurare z = new SocietateAsigurare(_CURENT_USER_ID, conStr);
            pis = dosar.SocietateCasco.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(z, pi.GetValue(dosar.SocietateCasco));
            }
            if (z.ID == null)
            {
                response r = y.Insert();
                z.ID = r.InsertedId;
            }
            else
            {
                z.Update();
            }
            dosar.Dosar.ID_SOCIETATE_CASCO = z.ID;


            z = new SocietateAsigurare(_CURENT_USER_ID, conStr);
            pis = dosar.SocietateRca.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(z, pi.GetValue(dosar.SocietateRca));
            }
            if (z.ID == null)
            {
                response r = y.Insert();
                z.ID = r.InsertedId;
            }
            else
            {
                z.Update();
            }
            dosar.Dosar.ID_SOCIETATE_RCA = z.ID;


            Dosar d = new Dosar(_CURENT_USER_ID, conStr);
            pis = dosar.Dosar.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(d, pi.GetValue(dosar.Dosar));
            }

            response toReturn = d.UpdateWithErrors();
            if (!toReturn.Status)
                return Json(toReturn, JsonRequestBehavior.AllowGet);
            toReturn = d.Validare();
            if (!toReturn.Status)
                return Json(toReturn, JsonRequestBehavior.AllowGet);
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            toReturn = dr.MovePendinToOk(Convert.ToInt32(d.ID));
            return Json(toReturn, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public bool ValidareAvizare(int id)
        {
            return Helpers.Helpers.ValidareAvizare(id);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult Edit(DosarView DosarView)
        {
            response r = new response();
            response toReturn = new response(true, "", null, null, new List<Error>());

            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            if (DosarView.Dosar.ID == null) // insert
            {
                /*
                var prop = DosarView.Dosar.GetType().GetField("authenticatedUserId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                prop.SetValue(DosarView.Dosar, _CURENT_USER_ID);
                prop = DosarView.Dosar.GetType().GetField("connectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                prop.SetValue(DosarView.Dosar, conStr);
                */
                Dosar d = new Dosar(_CURENT_USER_ID, conStr);
                PropertyInfo[] pis = DosarView.Dosar.GetType().GetProperties();
                foreach(PropertyInfo pi in pis)
                {
                    pi.SetValue(d, pi.GetValue(DosarView.Dosar));
                }

                if (DosarView.dosarJson != null)
                {
                    if (!String.IsNullOrEmpty(DosarView.dosarJson.NumeAsiguratCasco)){
                        AsiguratiRepository ar = new AsiguratiRepository(_CURENT_USER_ID, conStr);
                        Asigurat aCasco = (Asigurat)ar.Find(DosarView.dosarJson.NumeAsiguratCasco).Result;
                        if (aCasco != null && aCasco.ID != null)
                        {
                            d.ID_ASIGURAT_CASCO = aCasco.ID;
                        }
                        else
                        {
                            aCasco = new Asigurat(_CURENT_USER_ID, conStr);
                            aCasco.DENUMIRE = DosarView.dosarJson.NumeAsiguratCasco;
                            r = aCasco.Insert();
                            if (r.Status) d.ID_ASIGURAT_CASCO = Convert.ToInt32(aCasco.ID);
                            else toReturn.AddResponse(r);
                        }
                    }

                    if (!String.IsNullOrEmpty(DosarView.dosarJson.NumeAsiguratRca))
                    {
                        AsiguratiRepository ar = new AsiguratiRepository(_CURENT_USER_ID, conStr);
                        Asigurat aRca = (Asigurat)ar.Find(DosarView.dosarJson.NumeAsiguratRca).Result;
                        if (aRca != null && aRca.ID != null)
                        {
                            d.ID_ASIGURAT_RCA = aRca.ID;
                        }
                        else
                        {
                            aRca = new Asigurat(_CURENT_USER_ID, conStr);
                            aRca.DENUMIRE = DosarView.dosarJson.NumeAsiguratRca;
                            r = aRca.Insert();
                            if (r.Status) d.ID_ASIGURAT_RCA = Convert.ToInt32(aRca.ID);
                            /* -- nu e obligatoriu
                            else toReturn.AddResponse(r);
                            */
                        }
                    }

                    if (!String.IsNullOrEmpty(DosarView.dosarJson.NumarAutoCasco))
                    {
                        AutoRepository ar = new AutoRepository(_CURENT_USER_ID, conStr);
                        Auto autoCasco = (Auto)ar.Find(DosarView.dosarJson.NumarAutoCasco).Result;
                        if (autoCasco != null && autoCasco.ID != null)
                        {
                            d.ID_AUTO_CASCO = autoCasco.ID;
                        }
                        else
                        {
                            autoCasco = new Auto(_CURENT_USER_ID, conStr);
                            autoCasco.NR_AUTO = DosarView.dosarJson.NumarAutoCasco;
                            r = autoCasco.Insert();
                            if (r.Status) d.ID_AUTO_CASCO = Convert.ToInt32(autoCasco.ID);
                            else toReturn.AddResponse(r);
                        }
                    }

                    if (!String.IsNullOrEmpty(DosarView.dosarJson.NumarAutoRca))
                    {
                        AutoRepository ar = new AutoRepository(_CURENT_USER_ID, conStr);
                        Auto autoRca = (Auto)ar.Find(DosarView.dosarJson.NumarAutoRca).Result;
                        if (autoRca != null && autoRca.ID != null)
                        {
                            d.ID_AUTO_RCA = autoRca.ID;
                        }
                        else
                        {
                            autoRca = new Auto(_CURENT_USER_ID, conStr);
                            autoRca.NR_AUTO = DosarView.dosarJson.NumarAutoRca;
                            r = autoRca.Insert();
                            if (r.Status) d.ID_AUTO_RCA = Convert.ToInt32(autoRca.ID);
                            else toReturn.AddResponse(r);
                        }
                    }

                    if (!String.IsNullOrEmpty(DosarView.dosarJson.NumeIntervenient))
                    {
                        IntervenientiRepository ar = new IntervenientiRepository(_CURENT_USER_ID, conStr);
                        Intervenient i = (Intervenient)ar.Find(DosarView.dosarJson.NumeIntervenient).Result;
                        if (i != null && i.ID != null)
                        {
                            d.ID_INTERVENIENT = i.ID;
                        }
                        else
                        {
                            i = new Intervenient(_CURENT_USER_ID, conStr);
                            i.DENUMIRE = DosarView.dosarJson.NumeIntervenient;
                            r = i.Insert();
                            if (r.Status) d.ID_INTERVENIENT = Convert.ToInt32(i.ID);
                            /* -- nu e obligatoriu
                            else toReturn.AddResponse(r);
                            */
                        }
                    }
                    /*
                    if (!String.IsNullOrEmpty(DosarView.dosarJson.TipDosar))
                    {
                        NomenclatoareRepository ar = new NomenclatoareRepository(_CURENT_USER_ID, conStr);
                        Nomenclator tipDosar = (Nomenclator)ar.Find("tip_dosare", DosarView.dosarJson.TipDosar).Result;
                        if (tipDosar != null && tipDosar.ID != null)
                        {
                            d.ID_TIP_DOSAR = tipDosar.ID;
                        }
                        else
                        {
                            tipDosar = new Nomenclator(_CURENT_USER_ID, conStr, "tip_dosare");
                            tipDosar.DENUMIRE = DosarView.dosarJson.TipDosar;
                            r = tipDosar.Insert();
                            if (r.Status) d.ID_TIP_DOSAR = Convert.ToInt32(tipDosar.ID);
                            // -- nu e obligatoriu
                            //else toReturn.AddResponse(r);
                        }
                    }
                    */
                }
                if (!toReturn.Status)
                    return Json(toReturn, JsonRequestBehavior.AllowGet);

                r = d.Insert();
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            else { // update
                Dosar d = (Dosar)dr.Find(Convert.ToInt32(DosarView.Dosar.ID)).Result;
                Asigurat aCasco = (Asigurat)d.GetAsiguratCasco().Result;
                Asigurat aRca = (Asigurat)d.GetAsiguratRca().Result;
                Auto autoCasco = (Auto)d.GetAutoCasco().Result;
                Auto autoRca = (Auto)d.GetAutoRca().Result;
                Intervenient i = (Intervenient)d.GetIntervenient().Result;
                Nomenclator tipDosar = (Nomenclator)d.GetTipDosar().Result;

                if (DosarView.dosarJson.NumeAsiguratCasco != aCasco.DENUMIRE)
                {
                    aCasco.DENUMIRE = DosarView.dosarJson.NumeAsiguratCasco;
                    if (aCasco.ID == null)
                    {
                        r = aCasco.Insert();
                        if (r.Status) DosarView.Dosar.ID_ASIGURAT_CASCO = r.InsertedId;
                    }
                    else
                        r = aCasco.Update();
                    if (!r.Status)
                        toReturn.AddResponse(r);
                }
                if (DosarView.dosarJson.NumeAsiguratRca != aRca.DENUMIRE)
                {
                    aRca.DENUMIRE = DosarView.dosarJson.NumeAsiguratRca;
                    if (aRca.ID == null)
                    {
                        r = aRca.Insert();
                        if (r.Status) DosarView.Dosar.ID_ASIGURAT_RCA = r.InsertedId;
                    }
                    else
                        r = aRca.Update();
                    if (!r.Status)
                        toReturn.AddResponse(r);
                }
                if (DosarView.dosarJson.NumarAutoCasco != autoCasco.NR_AUTO)
                {
                    autoCasco.NR_AUTO = DosarView.dosarJson.NumarAutoCasco;
                    if (autoCasco.ID == null)
                    {
                        r = autoCasco.Insert();
                        if (r.Status) DosarView.Dosar.ID_AUTO_CASCO = r.InsertedId;
                    }
                    else
                        r = autoCasco.Update();
                    if (!r.Status)
                        toReturn.AddResponse(r);
                }
                if (DosarView.dosarJson.NumarAutoRca != autoRca.NR_AUTO)
                {
                    autoRca.NR_AUTO = DosarView.dosarJson.NumarAutoRca;
                    if (autoRca.ID == null)
                    {
                        r = autoRca.Insert();
                        if (r.Status) DosarView.Dosar.ID_AUTO_RCA = r.InsertedId;
                    }
                    else
                        r = autoRca.Update();
                    if (!r.Status)
                        toReturn.AddResponse(r);
                }
                if (DosarView.dosarJson.NumeIntervenient != i.DENUMIRE)
                {
                    i.DENUMIRE = DosarView.dosarJson.NumeIntervenient;
                    if (i.ID == null)
                    {
                        r = i.Insert();
                        if (r.Status) DosarView.Dosar.ID_INTERVENIENT = r.InsertedId;
                    }
                    else
                        r = i.Update();
                    if (!r.Status)
                        toReturn.AddResponse(r);
                }
                /*
                if (DosarView.dosarJson.TipDosar != tipDosar.DENUMIRE)
                {
                    tipDosar.DENUMIRE = DosarView.dosarJson.TipDosar;
                    if (tipDosar.ID == null)
                    {
                        tipDosar.Insert();
                        if (r.Status) DosarView.Dosar.ID_TIP_DOSAR = r.InsertedId;
                    }
                    else
                        r = tipDosar.Update();
                    if (!r.Status)
                        toReturn.AddResponse(r);
                }
                */
                if (!toReturn.Status)
                    return Json(toReturn, JsonRequestBehavior.AllowGet);

                string s = JsonConvert.SerializeObject(DosarView.Dosar, Formatting.None, new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = CommonFunctions.DATE_FORMAT });
                r = d.Update(s);
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpGet]
        public JsonResult Print(int id)
        {
            response r = new response();
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            Dosar d = new Dosar(_CURENT_USER_ID, conStr, id);
            // verificam daca are drept pe dosar 
            bool? hasWright = (bool?)d.UserHasWright(_CURENT_USER_ID).Result;
            if (hasWright == null || !(bool)hasWright)
            {
                HttpContext.Response.Redirect("~");
                return null;
            }
            else
            {
                DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
                r = dr.ExportDosarCompletToPdf(id);
                if (r.Status)
                {
                    r.Result = r.Message = r.Message.Substring(r.Message.LastIndexOf("\\") + 1);
                }
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult Avizare(int id, bool avizat)
        {
            response r = new response();
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            //DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            Dosar d = new Dosar(_CURENT_USER_ID, conStr, id);
            // verificam daca are drept pe dosar 
            bool? hasWright = (bool?)d.UserHasWright(_CURENT_USER_ID).Result;
            if (hasWright == null || !(bool)hasWright)
            {
                HttpContext.Response.Redirect("~");
                return null;
            }

            //r = dr.Avizare(id, avizat);
            r = d.Avizare(avizat);
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}
