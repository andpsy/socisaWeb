using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;

namespace socisaWeb
{
    public static class ExtensionMethods
    {
        // returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
        public static string UnixTicks(this DateTime dt)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return String.Format("/Date({0})/", ts.TotalMilliseconds);
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders[typeof(DateTime)] = new DateTimeModelBinder(SOCISA.CommonFunctions.DATE_TIME_FORMAT);
            ModelBinders.Binders[typeof(DateTime?)] = new DateTimeModelBinder(SOCISA.CommonFunctions.DATE_TIME_FORMAT);
            //ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder("dd.MM.yyyy HH:mm:ss"));
            //ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder("dd.MM.yyyy HH:mm:ss"));

            //ModelBinders.Binders.Remove(typeof(byte[]));
            //ModelBinders.Binders.Add(typeof(byte[]), new CustomByteArrayModelBinder());
            //ModelBinders.Binders[typeof(ImportDosarView)] = new CustomImportDosareModelBinder();
            GlobalFilters.Filters.Add(new GlobalAntiForgeryTokenAttribute(false));
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
        /*
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalAntiForgeryTokenAttribute(false));
        }
        */
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
            catch(Exception exp) { SOCISA.LogWriter.Log(exp); return null; }
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
            catch (Exception exp) { SOCISA.LogWriter.Log(exp); return null; }
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
            catch (Exception exp) { SOCISA.LogWriter.Log(exp); return null; }
        }
    }

    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string ActionName { get; set; }
        public bool Recursive { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            /*
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            */

            if (httpContext.Session["CURENT_USER_ID"] == null)
                return false;

            SOCISA.Models.Utilizator u = (SOCISA.Models.Utilizator)httpContext.Session["CURENT_USER"];
            string ut = ((SOCISA.Models.Nomenclator)u.GetTipUtilizator().Result).DENUMIRE;
            if (ut != "Administrator" || (ut == "Administrator" && httpContext.Request.Url.ToString().IndexOf("SelectSocietate") == -1))
            {
                if (httpContext.Session["ID_SOCIETATE"] == null)
                    return false;
            }

            SOCISA.Models.Action[] userActions = (SOCISA.Models.Action[])u.GetActions().Result;
            /*
            bool userHasAction = false;
            foreach(SOCISA.Models.Action a in userActions)
            {
                if (a.NAME == ActionName)
                {
                    userHasAction = true;
                    break;
                }
            }
            return userHasAction;
            */
           return UserHasAction(ActionName, userActions, Recursive);
        }

        protected bool UserHasAction(string actionName, SOCISA.Models.Action[] actions, bool recursive)
        {
            bool toReturn = false;
            foreach(SOCISA.Models.Action a in actions)
            {
                if(actionName == a.NAME)
                {
                    if (a.PARENT_ID != null && recursive)
                    {
                        SOCISA.Models.Action aParent = new SOCISA.Models.Action(Convert.ToInt32(HttpContext.Current.Session["CURENT_USER_ID"]), System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString, Convert.ToInt32(a.PARENT_ID));
                        toReturn = UserHasAction(aParent.NAME, actions, recursive);
                    }
                    else
                    {
                        toReturn = true;
                    }
                    break;
                }
            }
            return toReturn;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["__RequestVerificationToken"]);
        }
    }

    public class GlobalAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool autoValidateAllPost;

        public GlobalAntiForgeryTokenAttribute(bool autoValidateAllPost)
        {
            this.autoValidateAllPost = autoValidateAllPost;
        }

        private const string RequestVerificationTokenKey = "__RequestVerificationToken";
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            var req = filterContext.HttpContext.Request;
            if (req.HttpMethod.ToUpperInvariant() != "GET")
            {
                if (req.Form[RequestVerificationTokenKey] == null && req.IsAjaxRequest()) // && req.Headers[RequestVerificationTokenKey] != null 
                {
                    var cookie = req.Cookies[AntiForgeryConfig.CookieName];
                    if (req.Headers[RequestVerificationTokenKey] == null || cookie == null)
                        throw new HttpAntiForgeryException();
                    AntiForgery.Validate(cookie != null ? cookie.Value : null, req.Headers[RequestVerificationTokenKey]);
                    
                }
                else
                {
                    //if (autoValidateAllPost)
                        AntiForgery.Validate();
                }
            }
        }
    }
}
