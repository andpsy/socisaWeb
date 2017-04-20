using System;
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
        // GET: Dosare
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            return PartialView("_DosareNavigator", new DosarView(Convert.ToInt32(Session["CURENT_USER_ID"]), Convert.ToInt32(Session["ID_SOCIETATE"]), conStr));
        }

        [HttpPost]
        public JsonResult Search(DosarView DosarView)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            //string jsonFilter = JsonConvert.SerializeObject(DosarView);
            JObject f = (JObject)JsonConvert.DeserializeObject(Request.Form[0]);
            JToken jDosar = f["Dosar"];
            JToken jdosarJson = f["dosarJson"];
            if (jdosarJson != null)
            {
                if (jdosarJson["DataEvenimentStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataEvenimentStart"].ToString()) && jdosarJson["DataEvenimentEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataEvenimentEnd"].ToString()))
                {
                    jDosar["DATA_EVENIMENT"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataEvenimentStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataEvenimentEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataScaStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataScaStart"].ToString()) && jdosarJson["DataScaEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataScaEnd"].ToString()))
                {
                    jDosar["DATA_SCA"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataScaStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataScaEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataAvizareStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataAvizareStart"].ToString()) && jdosarJson["DataAvizareEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataAvizareEnd"].ToString()))
                {
                    jDosar["DATA_AVIZARE"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataAvizareStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataAvizareEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataNotificareStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataNotificareStart"].ToString()) && jdosarJson["DataNotificareEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataNotificareEnd"].ToString()))
                {
                    jDosar["DATA_NOTIFICARE"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataNotificareStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataNotificareEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataUltimeiModificariStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataUltimeiModificariStart"].ToString()) && jdosarJson["DataUltimeiModificariEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataUltimeiModificariEnd"].ToString()))
                {
                    jDosar["DATA_ULTIMEI_MODIFICARI"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataUltimeiModificariStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataUltimeiModificariEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataIesireCascoStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataIesireCascoStart"].ToString()) && jdosarJson["DataIesireCascoEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataIesireCascoEnd"].ToString()))
                {
                    jDosar["DATA_IESIRE_CASCO"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIesireCascoStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIesireCascoEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
                if (jdosarJson["DataIntrareRcaStart"] != null && !String.IsNullOrEmpty(jdosarJson["DataIntrareRcaStart"].ToString()) && jdosarJson["DataIntrareRcaEnd"] != null && !String.IsNullOrEmpty(jdosarJson["DataIntrareRcaEnd"].ToString()))
                {
                    jDosar["DATA_INTRARE_RCA"] = CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIntrareRcaStart"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture)) + "?" + CommonFunctions.ToMySqlFormatDate(DateTime.ParseExact(jdosarJson["DataIntrareRcaEnd"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture));
                }
            }
            //response r = dr.GetFiltered(null, null, String.Format("{'jDosar':{0},'jdosarJson':{1}}", JsonConvert.SerializeObject(jDosar), JsonConvert.SerializeObject(jdosarJson)), null);
            string jsonParam = "{\"jDosar\":" + JsonConvert.SerializeObject(jDosar) + ",\"jdosarJson\":" + JsonConvert.SerializeObject(jdosarJson) + "}";
            response r = dr.GetFiltered(null, null, jsonParam, null);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        // GET: Dosare/Details/5
        public JsonResult Details(int id)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(id).Result;
            Asigurat aCasco = (Asigurat)d.GetAsiguratCasco().Result;
            Asigurat aRca = (Asigurat)d.GetAsiguratRca().Result;
            Auto autoCasco = (Auto)d.GetAutoCasco().Result;
            Auto autoRca = (Auto)d.GetAutoRca().Result;
            Intervenient i = (Intervenient)d.GetIntervenient().Result;
            Nomenclator tipDosar = (Nomenclator)d.GetTipDosar().Result;
            string toReturn = "{\"aCasco\":" + JsonConvert.SerializeObject(aCasco) + ",\"aRca\":" + JsonConvert.SerializeObject(aRca) + ",\"autoCasco\":" + JsonConvert.SerializeObject(autoCasco) + ",\"autoRca\":" + JsonConvert.SerializeObject(autoRca) + ",\"intervenient\":" + JsonConvert.SerializeObject(i) + ",\"tipDosar\":" + JsonConvert.SerializeObject(tipDosar) + "}";
            object j = JsonConvert.DeserializeObject(toReturn);
            return Json(toReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(DosarView DosarView)
        {
            response r = new response();
            response toReturn = new response(true, "", null, null, new List<Error>());

            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
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

                string s = JsonConvert.SerializeObject(DosarView.Dosar, Formatting.None, new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
                r = d.Update(s);
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Print(int id)
        {
            response r = new response();
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            r = dr.ExportDosarCompletToPdf(id);
            if (r.Status)
            {
                r.Result = r.Message = r.Message.Substring(r.Message.LastIndexOf("\\") + 1);
            }
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}
