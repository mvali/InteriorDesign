using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Mvc;

public static class Utils
{
    public static object TempDataU(string keyName, System.Web.Mvc.TempDataDictionary dict, object defaultValue)
    {
        var retV = defaultValue;
        try
        {
            var keyValue = dict[keyName];
            if (keyValue != null)
                retV = keyValue;
        }
        catch { }
        return retV;
    }

    public static string TempData(string keyName, System.Web.Mvc.TempDataDictionary dict, string defaultValue = "")
    {
        return TempDataU(keyName, dict, defaultValue).ToString();
    }


    public static string Base64Encode(string m_enc)
    {
        //return m_enc;
        byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(m_enc);
        string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
        returnValue = Base64Scramble(returnValue);
        return returnValue;
    }
    public static string Base64Decode(string m_enc)
    {
        string returnValue = "";
        try
        {
            m_enc = Base64UnScramble(m_enc);
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(m_enc);
            returnValue = System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
        }
        catch (Exception ex)
        {
            Log.LogException(ex);
        }
        return returnValue;
    }
    static string Base64Scramble(string s)
    {
        if (s == null) return null;
        char[] charArray = s.ToCharArray();
        string char1 = "", char2 = "", char3 = "", char4 = "";
        int j = 0;
        for (int i = 0; i < charArray.Length; i++)
        {
            j++;
            switch (j)
            {
                case 1: char1 += charArray[i]; break;
                case 2: char2 += charArray[i]; break;
                case 3: char3 += charArray[i]; break;
                case 4: char4 += charArray[i]; break;
            }
            if (j == 4)
                j = 0;
        }
        return char4 + char3 + char2 + char1;
    }
    static string Base64UnScramble(string s)
    {
        if (s == null) return null;
        string char1 = "", char2 = "", char3 = "", char4 = "";

        double fraction = (double)s.Length / 4;
        int cntAll = (int)System.Math.Ceiling(fraction);
        int cntFirst = (int)System.Math.Floor(fraction);
        int cntRound = (int)System.Math.Round(fraction, 0);
        int h = cntRound == cntAll ? cntAll : cntFirst;
        char4 = s.Substring(0, cntFirst);
        char3 = s.Substring(cntFirst, h);
        char2 = s.Substring(cntFirst + h);
        char1 = s.Substring(cntFirst + 2 * h);

        s = "";
        for (int i = 0; i < cntAll; i++)
        {
            if (char1.Length > i)
                s += char1.Substring(i, 1);
            if (char2.Length > i)
                s += char2.Substring(i, 1);
            if (char3.Length > i)
                s += char3.Substring(i, 1);
            if (char4.Length > i)
                s += char4.Substring(i, 1);
        }
        return s;
    }
    public static string MapPath(string path)
    {
        try
        {
            return System.Web.Hosting.HostingEnvironment.MapPath(path);
        }
        catch (Exception ex)
        {
            return path;
        }
    }
    public static string SessionGet(string key, string defaultVal = "")
    {
        string retValue = defaultVal;
        try
        {
            if (System.Web.HttpContext.Current.Session[key] != null)
                retValue = System.Web.HttpContext.Current.Session[key].ToString();
        }
        catch { }
        return retValue;
    }
    public static void SessionRemove(string key)
    {
        try
        {
            if (System.Web.HttpContext.Current.Session[key] != null)
            {
                System.Web.HttpContext.Current.Session.Remove(key);
            }
        }
        catch { }
    }
    public static int Str2Int(object source, int defaultVal = -1)
    {
        int retVal = defaultVal;
        if (Int32.TryParse(source.ToString(), out retVal))
            return retVal;
        return defaultVal;
    }
    public static bool Str2Bool(string strValue, bool bDefaultValue = false)
    {
        bool bRet = bDefaultValue;
        try
        {
            bRet = Convert.ToBoolean(strValue);
        }
        catch
        {
            if (strValue == "1" || strValue.ToLower() == "true")
                bRet = true;
            if (strValue == "0" || strValue.ToLower() == "false")
                bRet = false;
        }
        return bRet;
    }
    /// <summary>
    /// System.Configuration.ConfigurationManager.AppSettings[appSettingsName].ToString()
    /// </summary>
    /// <param name="appSettingsName"></param>
    /// <returns>string</returns>
    public static string AppSetting(string appSettingsName)
    {
        string sRet = null;
        try
        {
            sRet = System.Configuration.ConfigurationManager.AppSettings[appSettingsName].ToString();
        }
        catch (Exception ex)
        {
            Log.LogException(ex);
        }
        return sRet;
    }
    /// <summary>
    /// "_"+ Response.Write(strValue)+"|" only if AppSetting("responseWrite") in web.config is set to "on"
    /// </summary>
    /// <param name="strValue">text to be writen on page</param>
    /// <param name="backNewLine">Add new line after text</param>
    /// <param name="frontNewLine">Add new line in front of text</param>
    public static void rw(string strValue, Boolean backNewLine = false, Boolean frontNewLine = false)
    {
        string masterWrite = AppSetting("responseWrite");
        if (masterWrite == "on")
        {
            string strRet = "_" + strValue + "|";
            if (frontNewLine)
            {
                strRet = "</br>" + strRet;
            }
            if (backNewLine)
            {
                strRet = strRet + "</br>";
            }
            System.Web.HttpContext.Current.Response.Write(strRet);
        }
    }
    public static string Rq(string name, string defaultValue = "")
    {
        string retV = defaultValue;
        try
        {
            retV = System.Web.HttpContext.Current.Request.QueryString[name].ToString();
        }
        catch { }
        return retV;
    }
    public static bool ControlerViewIs(string controlerName, string actionName, ViewContext vc)
    {
        bool retV = false;
        if (string.Equals(vc.RouteData.Values["controller"].ToString(), controlerName, StringComparison.CurrentCultureIgnoreCase))
        {
            if (string.Equals(vc.RouteData.Values["action"].ToString(), actionName, StringComparison.CurrentCultureIgnoreCase)) retV = true;
            else
                if (string.IsNullOrWhiteSpace(actionName)) retV = true;
        }
        return retV;
    }

}
