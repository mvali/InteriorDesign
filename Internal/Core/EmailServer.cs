using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

public class MailType
{
    public string EmailAddress { get; set; }
    public string DisplayName { get; set; }
    public MailType(string emailAdress, string displayName = "")
    {
        EmailAddress = emailAdress;
        DisplayName = displayName;
    }
}
public class MailTo
{
    public string EmailAddress { get; set; }
}
public class EmailParameter
{
    public string Name { get; set; }// // added by: new EmailParameter(){Name = "PARAM2", Value = "variable2"},
    public string Value { get; set; }
    public EmailParameter(string name, string value)
    {
        Name = name;
        Value = value;
    }// aded by: new EmailParameter("PARAM1","variable1"),

}
public class EmailParameterBlock
{
    private bool _show = true;

    public string Name { get; set; }
    public bool Show { get { return _show; } set { _show = value; } }
    public List<EmailParameter> EmailParameters { get; set; }
    public EmailParameterBlock(string name, bool show, List<EmailParameter> emailParameters = null)
    {
        Name = name;
        Show = show;
        EmailParameters = emailParameters;
    }
}

public class EmailServer
{
    public bool testMode = Utils.Str2Bool(System.Configuration.ConfigurationManager.AppSettings["EmailTestMode"].ToString());
    public string testEmail = Resources.WebsiteVariables.EmailTestAddress;
    public string emailAdministrator = Resources.WebsiteVariables.EmailAdministrator;
    public string emailPickupDirectory = System.Configuration.ConfigurationManager.AppSettings["EmailPickupDirectory"].ToString();

    SmtpClient Smtp()
    {
        SmtpClient sm = new SmtpClient(Utils.AppSetting("EmailServerHost"), 25);// not required if it is defined in web.config; or: new SmtpClient("mail.server.com",25); ("127.0.0.1")

        //sm.Credentials = new System.Net.NetworkCredential("email@server.com", "password");
        //sm.DeliveryMethod = SmtpDeliveryMethod.Network;// to send to live server
        if (Utils.AppSetting("EmailDeliveryMethod") == "localDirectory")
        {
            sm.UseDefaultCredentials = true;
            sm.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            sm.PickupDirectoryLocation = Utils.MapPath(emailPickupDirectory);
            sm.EnableSsl = false;
        }
        else
        {
            //sm.UseDefaultCredentials = true;
            sm.DeliveryMethod = SmtpDeliveryMethod.Network;
            //sm.Credentials = new System.Net.NetworkCredential(Utils.AppSetting("EmailServerUsername"), Utils.Base64Decode2(Utils.AppSetting("EmailServerPassword")), Utils.AppSetting("sParentUrl"));
            sm.Credentials = new System.Net.NetworkCredential(Utils.AppSetting("EmailServerUsername"), Utils.Base64Decode(Utils.AppSetting("EmailServerPassword")));
            sm.Host = Utils.AppSetting("EmailServerHost");
            sm.Port = 25;
            sm.EnableSsl = false;
        }
        return sm;
    }
    int EmailSendGo(MailType from, MailType to, string body, string subject, List<string> AttachmentFilePath = null)
    {
        try
        {
            MailMessage mail = new MailMessage();

            string fromAddress = from.EmailAddress;
            fromAddress = emailAdministrator;// server does not allow spoofing from another email

            MailAddress mf = null;
            if (String.IsNullOrWhiteSpace(from.DisplayName))
                mf = new MailAddress(fromAddress);
            else
                mf = new MailAddress(fromAddress, from.DisplayName);
            mail.From = mf;

            MailAddress mt = null;
            if (String.IsNullOrWhiteSpace(to.DisplayName))
                mt = new MailAddress(to.EmailAddress);
            else
                mt = new MailAddress(to.EmailAddress, to.DisplayName);

            if (testMode)
                mail.To.Add(new MailAddress(testEmail));
            else
                mail.To.Add(mt);

            if (AttachmentFilePath != null)
            {
                if (AttachmentFilePath.Count > 0)
                {
                    foreach (string item in AttachmentFilePath)
                    {
                        mail.Attachments.Add(new Attachment(item));
                    }
                }
            }
            mail.Body = body;
            mail.Subject = subject;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;

            Smtp().Send(mail);

            Log.AddLogEntryEmailsSent(mail.To.ToString(), mail.Subject, mail.Body, mail.From.ToString());
            return 0;
        }
        catch (Exception ex)
        {
            Log.LogException(ex);
            return -1;
        }
    }

    public int EmailSend(string EmailNameFromDB, MailType to, List<EmailParameter> ep, List<EmailParameterBlock> epb, string sitesName = "", List<string> AttachmentFilePath = null
        , string subject = "", string body = "", string fromEmail = "", string fromText = ""
        )
    {
        int r = 0; string fromName = "";
        //string subject = ""; string body = ""; string fromEmail="";
        /*string subject = "Subject {PARAM1} test123";
        string body = "asas {PARAM1} <!--BeginBLOCK1-->text123;{PARAM2} text123{PARAM1}-{PARAM2}TEXT123<!--EndBLOCK1-->";
        string fromName = "Sender's name";
        string fromEmail = "sender@server.com";*/

        if (!string.IsNullOrEmpty(EmailNameFromDB))
        {
            subject = Resources.WebsiteVariables.EmailContactSubject;
            body = Resources.WebsiteVariables.EmailContactBody;
            fromName = Resources.WebsiteVariables.EmailFromName;
            fromEmail = Resources.WebsiteVariables.EmailFromEmail;
        }
        else { fromName = fromText; }

        foreach (EmailParameter item in ep)
        {
            body = ReplaceEmailVariable(body, item.Name, item.Value);
            subject = ReplaceEmailVariable(subject, item.Name, item.Value);
        }

        if (epb != null)
        {
            foreach (EmailParameterBlock item in epb)
            {
                if (item.Show)
                {
                    bool bodyAddedOnce = false;
                    if (item.EmailParameters != null)
                    {
                        string blockContent = GetBlockFromString(body, item.Name, false);
                        foreach (EmailParameter itemc in item.EmailParameters)
                        {
                            blockContent = ReplaceEmailVariable(blockContent, itemc.Name, itemc.Value);
                        }
                        body = AddBlock(body, item.Name, blockContent);
                        bodyAddedOnce = true;
                    }
                    if (!bodyAddedOnce)
                        body = ShowBlock(body, item.Name);
                }
                else
                {
                    body = DeleteBlock(body, item.Name);
                }
            }
            // delete leftovers
            foreach (EmailParameterBlock item in epb)
            {
                body = DeleteBlock(body, item.Name);
            }
        }

        MailType from = new MailType(fromEmail, fromName);

        int r1 = EmailSendGo(from, to, body, subject, AttachmentFilePath);
        return r1;
    }

    public void WebpageSendMail()
    {
        MailType mto = new MailType("receiver@server.com", "Receiver's name");

        // beginsample email list
        List<EmailParameter> empl1 = new List<EmailParameter>();
        empl1.Add(new EmailParameter("PARAM1", "variable1"));

        List<EmailParameter> empl2 = new List<EmailParameter>
        {
            new EmailParameter("PARAM1","block1 variable1"),
            new EmailParameter("PARAM2","block1 variable2")
        };
        // end sample email list


        // start sample1 add block 
        EmailParameterBlock epb1 = new EmailParameterBlock("BLOCK1", true, new List<EmailParameter>{
                new EmailParameter("PARAM1","block1 variable1"),
                new EmailParameter("PARAM2","block1 variable2")
            });

        List<EmailParameterBlock> epbl1 = new List<EmailParameterBlock>();
        epbl1.Add(epb1);
        // end sample1 add block

        // start sample2 add block
        List<EmailParameterBlock> empbl = new List<EmailParameterBlock>{
            new EmailParameterBlock("BLOCK1", true, new List<EmailParameter>{
                new EmailParameter("PARAM1","block1 variable1"),
                new EmailParameter("PARAM2","block1 variable2")
            }),
            epb1
        };
        // end sample2 add block
        List<string> attachments = new List<string> { "c:\temp\filepath.txt" };

        int r = EmailSend("EmailNameFromDB", mto, empl2, empbl, "", attachments);
    }


    string ReplaceEmailVariable(string stringToReplaceIn, string vname, string vvalue)
    {
        string variable = "(%" + vname + "%)";
        return stringToReplaceIn.Replace(variable, vvalue);
    }

    string GetBlockFromString(string contentToSearchIn, string blockName, bool withHeaders)
    {
        string sRet = "";
        string beginBlock = "<!--begin" + blockName.ToLower() + "-->";
        string endBlock = "<!--end" + blockName.ToLower() + "-->";

        int firstString = 0, stringLength = 0;
        if (withHeaders)
        {
            firstString = contentToSearchIn.ToLower().IndexOf(beginBlock);
            stringLength = contentToSearchIn.ToLower().IndexOf(endBlock) - firstString + endBlock.Length;
        }
        else
        {
            firstString = contentToSearchIn.ToLower().IndexOf(beginBlock) + beginBlock.Length;
            stringLength = contentToSearchIn.ToLower().IndexOf(endBlock) - firstString;
        }

        if (stringLength > 0 && firstString > 0)
            sRet = contentToSearchIn.Substring(firstString, stringLength);

        return sRet;
    }
    string DeleteBlock(string contentToDeleteFrom, string blockName)
    {
        string sRet = contentToDeleteFrom;
        string selectedBlock = GetBlockFromString(contentToDeleteFrom, blockName, true);
        if (!String.IsNullOrWhiteSpace(selectedBlock))
            sRet = contentToDeleteFrom.Replace(selectedBlock, "");
        return sRet;
    }
    string AddBlock(string contentToAddIn, string blockName, string blockContent)
    {
        string sRet = contentToAddIn;
        string selectedBlock = GetBlockFromString(contentToAddIn, blockName, true);
        if (!String.IsNullOrWhiteSpace(selectedBlock))
            sRet = contentToAddIn.Replace(selectedBlock, blockContent + selectedBlock);
        return sRet;
    }
    string ShowBlock(string contentToProcess, string blockName)
    {
        string sRet = contentToProcess;
        string beginBlock = "<!--begin" + blockName.ToLower() + "-->";
        string endBlock = "<!--end" + blockName.ToLower() + "-->";
        int firstString = contentToProcess.ToLower().IndexOf(beginBlock);
        if (firstString > 0)
        {
            sRet = contentToProcess.Substring(0, firstString);
            sRet += contentToProcess.Substring(firstString + beginBlock.Length);
        }
        string sfirstString = sRet;

        int lastString = sfirstString.ToLower().IndexOf(endBlock);
        if (lastString > 0)
        {
            sRet = sfirstString.Substring(0, lastString);
            sRet += sfirstString.Substring(lastString + endBlock.Length);
        }

        return sRet;
    }

}

