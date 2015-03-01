using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ninject;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.Utilities;
using System.Web.Security;

namespace HoppsWebPlatform_Revamp.Controllers.API
{
    public class APIKeyController : ApiController
    {
        private NLog.Logger _logger;
        private IApiRepository _apiRepository;
        private IAltRepository _altRepository;

        public APIKeyController()
        {
            _apiRepository = new DataAccess.APIRepository();
            _altRepository = new DataAccess.AltRepository();
        }

        /// <summary>
        /// Creates a new API key
        /// </summary>
        /// <param name="newAPI">New API to add</param>
        /// <returns>Response message based on the insert</returns>
        [Attributes.Authorize]
        public HttpResponseMessage Post([FromBody]ApiKey newAPI)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    newAPI.PilotName = User.Identity.Name;
                    bool validEntry = true;
                    HttpError errorMessage = new HttpError();

                    //Check if the API is valid, and usable
                    if (!APIHelper.EVE_IsAPIValid(newAPI.KeyID, newAPI.VCode))
                    {
                        validEntry = false;
                        ModelState.AddModelError("", "API Key is not valid");
                    }
                    else
                    {
                        //If we got here, the API is valid, so we can check the privaleges
                        if (!APIHelper.EVE_IsAPIAccountWide(newAPI.KeyID, newAPI.VCode))
                        {
                            validEntry = false;
                            ModelState.AddModelError("", "API key not set to account wide.");
                        }
                        if (!APIHelper.EVE_IsAPIKeyFullAccountPermission(newAPI.KeyID, newAPI.VCode))
                        {
                            validEntry = false;
                            ModelState.AddModelError("", "API Key not set to full access. Access Mask: 268435455");
                        }
                        if (!APIHelper.EVE_IsAPISetToNotExpire(newAPI.KeyID, newAPI.VCode))
                        {
                            validEntry = false;
                            ModelState.AddModelError("", "API Key not set to never expire.");
                        }
                    }
                    //If this is true, the API did not get flagged as being invalid
                    if (validEntry)
                    {
                        int addResult = 1;
                        _apiRepository.AddAPIKey(newAPI, out addResult);
                        //Foreach of the characters on the API, add them to the alt list and add an API key for them so that we have a complete record of user characters
                        foreach (string character in APIHelper.EVE_GetCharactersOnAPI(newAPI.KeyID, newAPI.VCode).Where(x => x.Key != User.Identity.Name).Select(x => x.Key))
                        {
                            _altRepository.AddAlt(new Alt() { MainName = newAPI.PilotName, AltName = character, AltPurpose = "Unknown" });
                            _apiRepository.AddAPIKey(new ApiKey() { PilotName = character, KeyID = newAPI.KeyID, VCode = newAPI.VCode, Valid = true }, out addResult);
                            //Check if any of the characters on the API are in the corporation.
                            if (APIHelper.EVE_GetPilotsCorporationID(character) == 98038363)
                            {
                                //If the character is in in the corp and the character has a registered account .. IE, he has an alt or this is the initial API post for a new account, add him to the corp member role
                                if (Membership.GetUser(character) != null)
                                {
                                    if (!Roles.IsUserInRole(character, "CorporationMember"))
                                        Roles.AddUserToRole(character, "CorporationMember");
                                }
                            }
                        }
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ModelState);
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to add a new API", exn.Message));
                ModelState.Clear();
                ModelState.AddModelError("", "An error occured. Please try again to contact IT");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            //Message says it all...
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "This should not get here. Natalie derped");
        }

        /// <summary>
        /// Updates an existing API key aslong as the key belongs to the currently logged in user
        /// </summary>
        /// <param name="ID">KeyID of API Key</param>
        /// <param name="newAPI">API to update</param>
        /// <returns>Response message based on procedure</returns>
        [Attributes.Authorize]
        public HttpResponseMessage Put(int ID, [FromBody]ApiKey newAPI)
        {
            IEnumerable<ApiKey> existingKeys = _apiRepository.GetAllApisForPilot(User.Identity.Name);
            foreach (ApiKey key in existingKeys)
            {
                //If this is true, the user owns the api key, so we can proceed with the update
                if (key.KeyID == ID)
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            newAPI.PilotName = User.Identity.Name;
                            bool validEntry = true;
                            HttpError errorMessage = new HttpError();

                            //Check if the API is a valid API, IE its usable
                            if (!APIHelper.EVE_IsAPIValid(newAPI.KeyID, newAPI.VCode))
                            {
                                validEntry = false;
                                ModelState.AddModelError("", "API Key is not valid");
                            }
                            else
                            {
                                //if we get here, the API is valid, now check it has all of the privaleges
                                if (!APIHelper.EVE_IsAPIAccountWide(newAPI.KeyID, newAPI.VCode))
                                {
                                    validEntry = false;
                                    ModelState.AddModelError("", "API key not set to account wide.");
                                }
                                if (!APIHelper.EVE_IsAPIKeyFullAccountPermission(newAPI.KeyID, newAPI.VCode))
                                {
                                    validEntry = false;
                                    ModelState.AddModelError("", "API Key not set to full access. Access Mask: 268435455");
                                }
                                if (!APIHelper.EVE_IsAPISetToNotExpire(newAPI.KeyID, newAPI.VCode))
                                {
                                    validEntry = false;
                                    ModelState.AddModelError("", "API Key not set to never expire.");
                                }
                            }
                            //If this is true, none of the checks flagged the API as invalid
                            if (validEntry)
                            {
                                _apiRepository.UpdateAPIKey(_apiRepository.GetAPIByID(key.Id), newAPI);
                                return new HttpResponseMessage(HttpStatusCode.OK);
                            }
                            else
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                        }
                        return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ModelState);
                    }
                    catch (Exception exn)
                    {
                        _logger.Error(string.Format("An error occured whilst attempting to add a new API", exn.Message));
                        ModelState.Clear();
                        ModelState.AddModelError("", "An error occured. Please try again to contact IT");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "You are not authorised to edit this API");
        }
    }
}