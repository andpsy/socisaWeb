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

            ModelBinders.Binders[typeof(DateTime)] = new DateTimeModelBinder("dd.MM.yyyy");
            ModelBinders.Binders[typeof(DateTime?)] = new DateTimeModelBinder("dd.MM.yyyy");
            //ModelBinders.Binders.Remove(typeof(byte[]));
            //ModelBinders.Binders.Add(typeof(byte[]), new CustomByteArrayModelBinder());
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
                return DateTime.ParseExact(value.AttemptedValue, this._customFormat, System.Globalization.CultureInfo.InvariantCulture);
            }catch(Exception exp) { return null; }
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
}
