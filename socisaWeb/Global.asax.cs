using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.IO;

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
            System.Net.ServicePointManager.ServerCertificateValidationCa‌​llback += (se, cert, chain, sslerror) => true;

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

        protected void Session_Start(object sender, EventArgs e)
        {
            string key = this.RetrieveKey();

            //string conStr = Server.MapPath("~").ToLower().IndexOf("test") > 0 ? StringCipher.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString_test"].ConnectionString, key) : StringCipher.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString, key); // separam socisa de socisa_test
            string conStr = StringCipher.Decrypt(System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString_test"].ConnectionString, key);
            HttpContext.Current.Session["conStr"] = conStr;
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                string conStr = Session["conStr"].ToString(); //System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
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

        private string RetrieveKey()
        {
            return File.ReadAllText(Path.Combine(Server.MapPath("~"), "todo"));
        }
    }

    public static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
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
                        //SOCISA.Models.Action aParent = new SOCISA.Models.Action(Convert.ToInt32(HttpContext.Current.Session["CURENT_USER_ID"]), System.Configuration.ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString, Convert.ToInt32(a.PARENT_ID));
                        SOCISA.Models.Action aParent = new SOCISA.Models.Action(Convert.ToInt32(HttpContext.Current.Session["CURENT_USER_ID"]), HttpContext.Current.Session["conStr"].ToString(), Convert.ToInt32(a.PARENT_ID));
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
