using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internal
{
    public class Contact
    {
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        [StringLength(200, MinimumLength = 2, ErrorMessageResourceName = "NameMax", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        [StringLength(200, MinimumLength = 2, ErrorMessageResourceName = "EmailMax", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{1,64}@(?:[a-zA-Z0-9-]{1,63}\.){1,125}[a-zA-Z]{2,63}$", ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        public string Email { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessageResourceName = "PhoneMax", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        [RegularExpression(@"^[a-zA-Z0-9.+-]{4,26}", ErrorMessageResourceName = "PhoneInvalid", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceName = "MessageRequired", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        [StringLength(2000, MinimumLength = 2, ErrorMessageResourceName = "MessageMax", ErrorMessageResourceType = typeof(Resources.Contact.ContactAjax))]
        public string Message { get; set; }

        public Contact() { }

        public bool SendEmail()
        {
            bool bRet = false;
            var fromName = Resources.WebsiteVariables.EmailFromName;
            var fromEmail = Resources.WebsiteVariables.EmailFromEmail;

            string strIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            string strRef = System.Web.HttpContext.Current.Request.UrlReferrer.Host;

            List<EmailParameter> ep = new List<EmailParameter>{
                new EmailParameter("NAME", Name),
                new EmailParameter("EMAIL", Email),
                new EmailParameter("PHONE", Phone),
                new EmailParameter("MESSAGE", Message),
                new EmailParameter("IP", strIp),
                new EmailParameter("REFERRER",strRef)
                };

            int r1 = new EmailServer().EmailSend("EmailContact", new MailType(fromEmail), ep, null);
            bRet = r1 >= 0 ? true : false;

            return bRet;
        }
    }
}
