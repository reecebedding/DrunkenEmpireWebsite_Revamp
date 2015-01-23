using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web;

namespace HoppsWebPlatform_Revamp.Utilities
{
    public static class EmailHelper
    {
        public static string RecruitmentEmail = "recruitment@thedrunkenempire.org";

        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="senderEmail">User to send email on behalf of.</param>
        /// <param name="recieverEmail">User to recieve mail.</param>
        /// <param name="subject">Subject Header</param>
        /// <param name="bodyHTML">Body for E-Mail</param>
        /// <param name="useDoNotReplySig">Switch based on whether the "Do No Reply" signature should be attached.</param>
        public static void SendEmail(string senderEmail, string recieverEmail, string subject, string bodyHTML, bool useDoNotReplySig)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            message.From = new System.Net.Mail.MailAddress(senderEmail);
            message.To.Add(new System.Net.Mail.MailAddress(recieverEmail));

            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = subject;
            message.Body = bodyHTML;

            System.Net.Mail.AlternateView view = System.Net.Mail.AlternateView.CreateAlternateViewFromString(message.Body, new System.Net.Mime.ContentType("text/html"));
            message.AlternateViews.Add(view);

            if (useDoNotReplySig)
                message.Body += "<br />This is an automated message for test purposes. <br/><br/> <i>- Do not reply to this email as the account is not monitored.</i>";

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Send(message);            
        }

        /// <summary>
        /// Converts a rendered view to a string.
        /// </summary>
        /// <param name="controller">Current controller that the view is being rendered from</param>
        /// <param name="viewName">Name of view to render</param>
        /// <param name="viewData">Data to parse to the view renderer</param>
        /// <returns>String version of rendered view.</returns>
        public static string ConvertViewToString(Controller controller, string viewName, ViewDataDictionary viewData)
        {
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, viewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
