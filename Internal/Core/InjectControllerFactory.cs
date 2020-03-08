using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Threading;

//http://stackoverflow.com/questions/32764989/asp-net-mvc-5-culture-in-route-and-url

public class InjectControllerFactory : DefaultControllerFactory
{
    protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
    {
        //Get the {language} parameter in the RouteData
        string UILanguage = "", routeLanguage = "";
        string routeLanguage2 = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
        string cultureCookie = GetCultureCookie();

        if (requestContext.RouteData.Values["language"] == null)
        {// no language defined
        }
        else
        {
            routeLanguage = requestContext.RouteData.Values["language"].ToString();
        }
        //req1: first pass: no language in url-> routeLanguage="", routeLanguage2="en-us", UILanguage="ro"(from browser)
        //req1: second pass:        routeLanguage="ro",    routeLanguage2="ro-ro"
        //req2: charge lang to eng: routeLanguage="en",    routeLanguage2="en-us"
        //req2: second pass:        routeLanguage="en-US", routeLanguage2="en-us"
        //reg3: submit contact-en:  routeLanguage="",      routeLanguage2="en-us", UILanguage="ro"(from browser)

        if (string.IsNullOrWhiteSpace(routeLanguage) && !string.IsNullOrWhiteSpace(cultureCookie))
            routeLanguage = cultureCookie;

        // by post use this:
        if (string.IsNullOrWhiteSpace(routeLanguage))
        {
            if (requestContext.HttpContext.Request.UserLanguages != null)
                if (requestContext.HttpContext.Request.UserLanguages.Length > 0)
                    UILanguage = requestContext.HttpContext.Request.UserLanguages[0];

            if (!string.IsNullOrWhiteSpace(UILanguage))
            {
                UILanguage = CultureHelper.GetImplementedCulture(UILanguage);
                routeLanguage = UILanguage;
            }
        }

        //req1: first pass: no language in url-> routeLanguage=UILanguage="ro"(from browser)
        //req1: second pass: routeLanguage="ro"(from RouteData)

        // Validate culture name
        routeLanguage = CultureHelper.GetImplementedCulture(routeLanguage); // This is safe

        //if(routeLanguage != UILanguage.ToLowerInvariant())
        {
            requestContext.RouteData.Values["language"] = routeLanguage;
        }
        SetCultureCookie(routeLanguage);

        //Get the culture info of the language code
        CultureInfo culture = CultureInfo.CreateSpecificCulture(routeLanguage);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        return base.GetControllerInstance(requestContext, controllerType);
    }
    public void SetCultureCookie(string culture)
    {
        // Validate input
        var cultureOk = CultureHelper.GetImplementedCulture(culture);

        // Save culture in a cookie
        HttpCookie cookie = HttpContext.Current.Request.Cookies["_culture"];
        if (cookie != null)
            cookie.Value = culture;
        else
        {
            cookie = new HttpCookie("_culture");
            cookie.Value = culture;
            cookie.Expires = DateTime.Now.AddYears(1);
        }
        HttpContext.Current.Response.Cookies.Add(cookie);
    }
    public string GetCultureCookie()
    {
        string culture = "";

        // get culture from cookie
        HttpCookie cookie = HttpContext.Current.Request.Cookies["_culture"];
        if (cookie != null)
            culture = cookie.Value;

        return culture;
    }
}
