using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace socisaWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders[typeof(DateTime)] = new DateTimeModelBinder("dd.MM.yyyy HH:mm:ss");
            ModelBinders.Binders[typeof(DateTime?)] = new DateTimeModelBinder("dd.MM.yyyy HH:mm:ss");
            //ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder("dd.MM.yyyy HH:mm:ss"));
            //ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder("dd.MM.yyyy HH:mm:ss"));

            //ModelBinders.Binders.Remove(typeof(byte[]));
            //ModelBinders.Binders.Add(typeof(byte[]), new CustomByteArrayModelBinder());
            //ModelBinders.Binders[typeof(ImportDosarView)] = new CustomImportDosareModelBinder();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
                SOCISA.Models.Utilizator u = (SOCISA.Models.Utilizator)Session["CURENT_USER"];
                u.IS_ONLINE = false;
                u.Update();
                System.Web.Security.FormsAuthentication.SignOut();
            }
            catch (Exception exp) { SOCISA.LogWriter.Log(exp); }
        }
    }

    public class DateTimeModelBinder : IModelBinder
    {
        private readonly string _customFormat;
        public DateTimeModelBinder(string CustomFormat)
        {
            this._customFormat = CustomFormat;
        }
        object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || String.IsNullOrEmpty(value.AttemptedValue)) return null;
            try
            {
                string formatedDate = value.AttemptedValue;
                if (value.AttemptedValue.Length == 10)
                    formatedDate = String.Format("{0} 00:00:00", value.AttemptedValue);
                //return DateTime.ParseExact(value.AttemptedValue, this._customFormat, System.Globalization.CultureInfo.InvariantCulture);
                return DateTime.ParseExact(formatedDate, this._customFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(Exception exp) { return null; }
        }
    }

    public class CustomByteArrayModelBinder : IModelBinder
    {
        object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || String.IsNullOrEmpty(value.AttemptedValue)) return null;
            try
            {
                return Convert.FromBase64String(value.AttemptedValue);
            }
            catch (Exception exp) { return null; }
        }
    }

    public class CustomImportDosareModelBinder : IModelBinder
    {
        object IModelBinder.BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            ValueProviderResult value = ((DictionaryValueProvider<object>)(((ValueProviderCollection)bindingContext.ValueProvider)[2])).GetValue("ImportDosarView[0][0].Status");
            if (value == null || String.IsNullOrEmpty(value.AttemptedValue)) return null;
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ImportDosarView>(value.AttemptedValue);
            }
            catch (Exception exp) { return null; }
        }
    }
}
