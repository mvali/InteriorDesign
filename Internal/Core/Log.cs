using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public static class Log
{
    static string GetFileFullName(string filename = "")
    {
        System.Web.UI.Page myPage = new System.Web.UI.Page();
        string fpath = "";
        try
        {
            fpath = System.Configuration.ConfigurationManager.AppSettings["LogPath"].ToString();
            fpath = myPage.Server.MapPath(fpath);
        }
        catch
        {
            fpath = System.Configuration.ConfigurationManager.AppSettings["LogPath"].ToString();
        }

        try
        {
            System.DateTime dt = System.DateTime.Now;

            int month = dt.Month, day = dt.Day;

            fpath += "\\" + filename +
                ((month < 10) ? ("0" + month.ToString()) : month.ToString()) +
                ((day < 10) ? "0" + day.ToString() : day.ToString()) +
                dt.Year +
                ".txt";


            if (!System.IO.File.Exists(fpath))
            {
                System.IO.FileStream fs = System.IO.File.Create(fpath);
                fs.Close();
            }
        }
        catch (Exception ex)
        {
            Log.LogException(ex);
        }
        return fpath;
    }

    /// <summary>
    /// Add Entry In Log file specified on "LogPath" web.config record.
    /// </summary>
    /// <param name="text"></param>
    static void AddLogEntryRoot(string text, string filename = "")
    {
        try
        {
            System.Web.UI.Page myPage = new System.Web.UI.Page();

            string filepath = GetFileFullName(filename);

            //FileInfo fi = new FileInfo(myPage.Server.MapPath("/Logs/ErrorLog.txt"));
            FileInfo fi = new FileInfo(filepath);

            if (fi.Length > 2000000)
            {
                fi.Delete();
            }

            FileStream fs = fi.Open(FileMode.Append, FileAccess.Write, FileShare.None);

            string strError = "\r\n--------------------------------------------------------------------------------\r\n";
            strError += System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ff") + "   -    ";
            strError += text + "\r\n";

            if (System.Web.HttpContext.Current != null)
                strError += "Uri: " + System.Web.HttpContext.Current.Request.Url.AbsoluteUri + "\r\n";

            StreamWriter swFromFile = new StreamWriter(fs);
            swFromFile.Write(strError);
            swFromFile.Flush();
            swFromFile.Close();

            fs.Close();
        }
        catch (Exception ex)
        {
            System.Web.HttpContext.Current.Response.Write("Clog_ex1:" + ex.ToString());
        }
    }
    public static void AddLogEntry(string text)
    {
        AddLogEntryRoot(text);
    }
    public static void AddLogEntryEmailsSent(string to, string subject, string body, string from)
    {
        string text = "To:" + to + "_From:" + from;
        text += "\r\n";
        text += "Subject:" + subject;
        text += "\r\n";
        text += "Body:" + body;

        AddLogEntryRoot(text, "emails");
    }

    public static void LogException(Exception ex, string extraText = "")
    {
        try
        {
            string strError = extraText + " ex: " + ex.ToString() + "\r\n";
            AddLogEntry(strError);
            Utils.rw(ex.ToString() + "<br/>");
        }
        catch (Exception ex1)
        {
            System.Web.HttpContext.Current.Response.Write("Clog_ex2:" + ex1.ToString());
        }
    }
}
