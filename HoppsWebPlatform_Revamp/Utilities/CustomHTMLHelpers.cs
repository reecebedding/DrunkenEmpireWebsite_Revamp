using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Linq;
using Newtonsoft.Json;

namespace HoppsWebPlatform_Revamp.Utilities
{
    /// <summary>
    /// Custom HTML Helper class that extends the @html method in the view.
    /// </summary>
    public static class CustomHTMLHelpers
    {
        /// <summary>
        /// Adds a single Validation Success message
        /// </summary>
        /// <param name="helper">Reference to extend the HtmlHelper class</param>
        /// <param name="message">Message to add to container.</param>
        /// <returns>HTML Markup containing success container with message.</returns>
        public static IHtmlString ValidationSummaryValidMessage(this HtmlHelper helper, string message)
        {
            return new HtmlString(string.Format("<div class='validation-summary-valid-message'><ul><li>{0}</li></ul></div>", message));
        }

        /// <summary>
        /// Creates a success validation summary based on the viewdata containing a key of "valid-message" and of type List<string>
        /// </summary>
        /// <param name="helper">Reference to extend the HtmlHelper</param>
        /// <returns>HTML markup containing success messages in list.</returns>
        public static IHtmlString ValidationSummaryValidMessage(this HtmlHelper helper)
        {
            string htmlMarkup = string.Empty;

            //Check if the list of messages is present in the view data.
            var messages = helper.ViewContext.ViewData.ContainsKey("valid-message") ? helper.ViewContext.ViewData["valid-message"] : null;
            if (messages != null)
            {
                //Get the list of messages from the viewdata
                List<string> listOfMessages = ((List<string>)(messages));
                //Create the Markup for the success container.
                StringBuilder htmlMarkupBuilder = new StringBuilder("<div class='validation-summary-valid-message'><ul>");

                foreach (string message in listOfMessages)
                {
                    htmlMarkupBuilder.Append(string.Format("<li>{0}</li>", message));
                }

                htmlMarkupBuilder.Append("</ul></div>");
                htmlMarkup = htmlMarkupBuilder.ToString();
            }
            //Create HtmlString of markup as it will need to render not be encoded (MVC Behaviour with base string.)
            return new HtmlString(htmlMarkup);
                
        }
        
        /// <summary>
        /// Checks if the user has all the roles specified in a csvString
        /// </summary>
        /// <param name="userName">UserName to check roles against</param>
        /// <param name="csvRoles">CSV string of roles to check</param>
        /// <returns>Switch based on whether the user has all the roles specified.</returns>
        public static bool IsUserInAllRoles(string userName, string csvRoles)
        {
            bool allRolesHeld = true;
            string[] rolesHeld = Roles.GetRolesForUser(userName);
            string[] rolesToCheck = csvRoles.Split(',');
            foreach (string roleToCheck in rolesToCheck)
            {
                if (!rolesHeld.Any(x => x == roleToCheck))
                    allRolesHeld = false;
            }
            return allRolesHeld;
        }

        /// <summary>
        /// Overload for the IsUserInAllRoles method to incorporate the HtmlHelper parameter for use in razor.
        /// </summary>
        /// <param name="helper">HtmlHelper instance</param>
        /// <param name="userName">UserName to check for roles.</param>
        /// <param name="csvRoles">CSV String of roles to check for</param>
        /// <returns>Swtich based on whether the user has all the roles specified</returns>
        public static bool IsUserInAllRoles(this HtmlHelper helper, string userName, string csvRoles)
        {
            return IsUserInAllRoles(userName, csvRoles);
        }

        /// <summary>
        /// Checks if the user has any of the roles specified in a csv string
        /// </summary>
        /// <param name="userName">Username to check roles against</param>
        /// <param name="csvRoles">CSV String of roles to check</param>
        /// <returns>Switch based on whether the user has any of the roles specified.</returns>
        public static bool IsUserInAnyRole(string userName, string csvRoles)
        {
            string[] rolesHeld = Roles.GetRolesForUser(userName);
            string[] rolesToCheck = csvRoles.Split(',');
            foreach (string roleToCheck in rolesToCheck)
            {
                if (rolesHeld.Any(x => x == roleToCheck))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Overload for the IsUserInAnyRole method to incorporation the HtmlHelper parameter for use in razor
        /// </summary>
        /// <param name="helper">Reference to extend the HtmlHelper</param>
        /// <param name="userName">Username to check roles against</param>
        /// <param name="csvRoles">CSV String of roles to check</param>
        /// <returns>Switch based on whether the user has any of the roles specified.</returns>
        public static bool IsUserInAnyRole(this HtmlHelper helper, string userName, string csvRoles)
        {
            return IsUserInAnyRole(userName, csvRoles);
        }

        /// <summary>
        /// Take a model state object and converts any model errors in a serialized JSON object, containing both a key (Field name), and Value (Errors)
        /// </summary>
        /// <param name="state">ModelState of controller action</param>
        /// <returns>JSON </returns>
        public static JsonResult ConvertModelStateErrorsToJson(ModelStateDictionary state)
        {
            var obj = new Newtonsoft.Json.Linq.JObject();

            //Extracts all of the ModelState errors into a dictionary containing the fields as both key and value.
            Dictionary<string, string> errors = state.Keys.ToDictionary(x => x.ToString(), x => x.ToString());
            for (int i = 0; i < errors.Count; i++)
            {
                //Updates the value of each KeyValue pair to contain the correct serialized error messages for that field.
                errors[errors.ElementAt(i).Key] = JsonConvert.SerializeObject(state.Values.ToList()[i]);
            }
            
            List<string> keys = new List<string>();
            //Creates a list of keys that actually have errors recorded so we dont send all the rubbish field data back.
            for (int i = 0; i < state.Keys.Count; i++)
            {
                //If there are any errors for that field, add the field name as a key.
                if (state.Values.ElementAt(i).Errors.Count > 0)
                {
                    keys.Add(state.Keys.ElementAt(i).ToString());
                }
            }
                
            //Add the success criteria, - Is there any errors recorded.
            obj.Add("Result", (keys.Count == 0));

            //Serialize the whole thing into a JSON string but only where the error keyValuePair is logged as an error in the Keys list.
            obj.Add("ModelErrors", JsonConvert.SerializeObject(errors.Where(x => keys.Contains(x.Key)), Formatting.Indented));

            //Return new Json Object with the serialized data.
            return new JsonResult() { Data = obj.ToString() };
        }
    }
}