using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using HoppsWebPlatform_Revamp.Models;
using Newtonsoft.Json.Linq;

namespace HoppsWebPlatform_Revamp.Utilities
{
    public static class APIHelper
    {
        private static string _baseURL = "https://api.eveonline.com";
        private static readonly string _errorContactingMessage = "An error occurred whilst attempting to contact the API server";

        /// <summary>
        /// Checks if the parsed pilot name is present on the API key.
        /// </summary>
        /// <param name="characterName">Pilot name to check</param>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API VCode</param>
        /// <returns>Switch based on whether the character is present on API</returns>
        public static bool EVE_IsCharacterOnAPI(string characterName, long keyID, string vCode)
        {
            return EVE_GetCharactersOnAPI(keyID, vCode).Any(item => string.Equals(item.Key, characterName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets list of a characters and their pilot ID's present on the API
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API VCode</param>
        /// <returns>List of character names and their associated PilotID</returns>
        public static Dictionary<string, long> EVE_GetCharactersOnAPI(long keyID, string vCode)
        {
            Dictionary<string, long> characters = new Dictionary<string, long>();
            try
            {
                XmlDocument doc = new XmlDocument();
                string requestURL = string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode);
                doc.Load(requestURL);
                foreach (XmlNode nod in doc.GetElementsByTagName("rowset")[0].ChildNodes)
                {
                    characters.Add(nod.Attributes["characterName"].InnerText, Convert.ToInt64(nod.Attributes["characterID"].InnerText));
                }
                return characters;
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data from api.");
            }
        }

        /// <summary>
        /// Checks if the character exists in eve.
        /// </summary>
        /// <param name="characterName">Pilot name</param>
        /// <returns>Switch based on whether the character exists in eve</returns>
        public static bool EVE_DoesCharacterExist(string characterName)
        {
            return (EVE_GetPilotIDByName(characterName) != 0);
        }

        /// <summary>
        /// Gets the pilot name based on their pilot ID
        /// </summary>
        /// <param name="pilotID">Pilot ID</param>
        /// <returns>Pilot name</returns>
        public static string EVE_GetPilotNameByID(long pilotID)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/eve/CharacterName.xml.aspx?ids={1}", _baseURL, pilotID));
                return doc.GetElementsByTagName("row")[0].Attributes["name"].Value;
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data.");
            }
        }

        /// <summary>
        /// Gets the name of a mailing list a user is in based on the mailing list id
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API vCode</param>
        /// <param name="characterID">Character ID</param>
        /// <param name="mailID">Mailing list id</param>
        /// <returns>Name of mailing list or error message</returns>
        public static string EVE_GetMailingListNameByID(long keyID, string vCode, long characterID, long mailID)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/char/mailinglists.xml.aspx?keyID={1}&vCode={2}&characterID={3}", _baseURL, keyID, vCode, characterID));
                XmlNodeList nodes = doc.GetElementsByTagName("row");
                Dictionary<long, string> lists = new Dictionary<long, string>();

                //Get all lists
                foreach (XmlNode node in nodes)
                    lists.Add(Convert.ToInt64(node.Attributes["listID"].Value), node.Attributes["displayName"].Value);

                //Returns the name if there are any lists that have the same ID (Should be true, but might fail if the user leaves the list?)
                if (lists.Any(x => x.Key == mailID))
                    return lists.First(x => x.Key == mailID).Value;

                return "Unable to retrieve mailing list name";
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data.");
            }
        }

        /// <summary>
        /// Gets the pilot ID based on their character name
        /// </summary>
        /// <param name="characterName">Pilot name</param>
        /// <returns>Pilot ID</returns>
        public static long EVE_GetPilotIDByName(string characterName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/eve/CharacterID.xml.aspx?names={1}",_baseURL, characterName));
                return Convert.ToInt32(doc.GetElementsByTagName("row")[0].Attributes["characterID"].Value);
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data.");
            }            
        }

        /// <summary>
        /// Checks whether the API is a valid api key.
        /// </summary>
        /// <param name="keyID">API Key ID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>Switch based on whether the API key is valid.</returns>
        public static bool EVE_IsAPIValid(long keyID, string vCode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode));
                return true;
            }
            catch (WebException exn)
            {
                if (exn.Status == WebExceptionStatus.ProtocolError)
                    return false;
                else
                    throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Checks if the api key is set to account wide
        /// </summary>
        /// <param name="KeyID">API Key ID</param>
        /// <param name="vCode">API VCode</param>
        /// <returns>Switch based on whether the API is account wide.</returns>
        public static bool EVE_IsAPIAccountWide(long keyID, string vCode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode));
                return (doc.GetElementsByTagName("key")[0].Attributes["type"].Value == "Account");
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Checks if the api key is set to character wide
        /// </summary>
        /// <param name="keyID">API Key ID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>Switch based on whether the API is character wide.</returns>
        public static bool EVE_IsAPICharacterWide(long keyID, string vCode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode));
                return (doc.GetElementsByTagName("key")[0].Attributes["type"].Value == "Character");
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Checks if the api key is set to corporation type
        /// </summary>
        /// <param name="keyID">API Key ID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>Switch based on whether the API is set to corporation wide.</returns>
        public static bool EVE_IsAPICorporationWide(long keyID, string vCode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode));
                return (doc.GetElementsByTagName("key")[0].Attributes["type"].Value == "Corporation");
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Gets the API keys access mask
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>APIKey access mask</returns>
        public static long EVE_GetKeyAccessMask(long keyID, string vCode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode));
                return Convert.ToInt64(doc.GetElementsByTagName("key")[0].Attributes["accessMask"].Value);
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Returns a switch based on whether the API key is set to full account type permissions
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>Switch based on whether the api key is set to full account type permission</returns>
        public static bool EVE_IsAPIKeyFullAccountPermission(long keyID, string vCode)
        {
            return (EVE_GetKeyAccessMask(keyID, vCode) == 268435455);
        }

        /// <summary>
        /// Returns a switch based on whether the API key is set to full corporation type permissions
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>Switch based on whether the api key is set to full corporation type permission</returns>
        public static bool EVE_IsAPIKeyFullCorporationPermission(long keyID, string vCode)
        {
            return (EVE_GetKeyAccessMask(keyID, vCode) == 67108863);
        }

        /// <summary>
        /// Returns a switch based on whether the API key is set to no expire.
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API vCode</param>
        /// <returns>Switch based on whether the key is set to not expire.</returns>
        public static bool EVE_IsAPISetToNotExpire(long keyID, string vCode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/account/APIKeyInfo.xml.aspx?keyID={1}&vCode={2}", _baseURL, keyID, vCode));
                return (doc.GetElementsByTagName("key")[0].Attributes["expires"].Value == "");
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Checks if the parsed API key is set to full, non expirey and account wide.
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API VCode</param>
        /// <returns>Switch based on whether API matches settings.</returns>
        public static bool EVE_IsAPIFullConsistantAccount(long keyID, string vCode)
        {
            bool valid = true;
            try
            {
                if (!EVE_IsAPIAccountWide(keyID, vCode))
                    valid = false;
                if (!EVE_IsAPISetToNotExpire(keyID, vCode))
                    valid = false;
                if (!EVE_IsAPIKeyFullAccountPermission(keyID, vCode))
                    valid = false;
                return valid;  
            }
            catch (Exception)
            {
                return false;
            }          
        }

        /// <summary>
        /// Checks if the pilot is in the specified role - NOTE: Role names are based on server lookups
        /// </summary>
        /// <param name="pilotName">Pilot name to check</param>
        /// <param name="roleName">Role name to check for</param>
        /// <param name="keyID">API Key KeyID</param>
        /// <param name="vCode">API Key VCode</param>
        /// <returns>Switch based on whether the pilot is in the specified role</returns>
        public static bool EVE_IsPilotInRole(string pilotName, string roleName, long keyID, string vCode)
        {
            long pilotID = EVE_GetPilotIDByName(pilotName);
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/char/CharacterSheet.xml.aspx?keyID={1}&vCode={2}&characterID={3}", _baseURL, keyID, vCode, pilotID));
                XmlNodeList roles = doc.SelectNodes("//eveapi/result//rowset[@name='corporationRoles']/row");
                bool isInRole = false;
                foreach (XmlNode node in roles)                
                    if (node.Attributes["roleName"].Value == roleName)
                        isInRole = true;
                return isInRole;
                
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
        }

        /// <summary>
        /// Gets the corporationID of a pilot.
        /// </summary>
        /// <param name="pilotName">Pilot name to search the corp id of.</param>
        /// <returns>Corporation ID</returns>
        public static long EVE_GetPilotsCorporationID(string pilotName)
        {
            long corpID;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/eve/CharacterInfo.xml.aspx?characterID={1}", _baseURL, EVE_GetPilotIDByName(pilotName)));
                corpID = Convert.ToInt64(doc.GetElementsByTagName("corporationID")[0].InnerText);                     
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _errorContactingMessage, exn.Message));
            }
            return corpID;
        }

        /// <summary>
        /// Gets the corporation name from ID
        /// </summary>
        /// <param name="corporationID">Corporation id to get name of.</param>
        /// <returns>Corporation name</returns>
        public static string EVE_GetCorporationNameByID(long corporationID)
        {
            string corpName;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format("{0}/eve/CharacterName.xml.aspx?ids={1}", _baseURL, corporationID));
                corpName = doc.GetElementsByTagName("row")[0].Attributes["name"].Value;
            }
            catch (Exception exn)
            {
                throw new Exception(string.Format("{0}: {1}", _baseURL, exn.Message));
            }
            return corpName;
        }

        /// <summary>
        /// Gets list of a characters in the corporation
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API VCode</param>
        /// <returns>List of character names and their associated PilotID</returns>
        public static IEnumerable<CorpMember> EVE_GetCorporationMembers(long keyID, string vCode)
        {
            List<CorpMember> corpMembers = new List<CorpMember>();
            try
            {
                XmlDocument doc = new XmlDocument();
                string requestURL = string.Format("{0}/corp/MemberTracking.xml.aspx?keyID={1}&vCode={2}&extended=true", _baseURL, keyID, vCode);
                doc.Load(requestURL);
                foreach (XmlNode nod in doc.GetElementsByTagName("rowset")[0].ChildNodes)
                {
                    corpMembers.Add(new CorpMember() {PilotID = Convert.ToInt64(nod.Attributes["characterID"].Value), PilotName = nod.Attributes["name"].Value, LastLogon = Convert.ToDateTime(nod.Attributes["logonDateTime"].Value), Location = nod.Attributes["location"].Value, Roles = Convert.ToInt64(nod.Attributes["roles"].Value), Ship = nod.Attributes["shipType"].Value });
                }
                return corpMembers;
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data from api.");
            }
        }

        /// <summary>
        /// Gets any wallet journal entries that are of a specific type and are greater or equal than a minimum
        /// </summary>
        /// <param name="keyID">KeyID of api to checks</param>
        /// <param name="vCode">VCode of api to check</param>
        /// <param name="pilotID">Pilot ID to check on the API</param>
        /// <param name="type">Type to check for. Use EVE API docs for args</param>
        /// <param name="minimumVal">Minimum value to flag against</param>
        /// <returns>IEnumerable of wallet journal entries</returns>
        public static IEnumerable<WalletJournalEntry> EVE_GetWalletTransactionsBasedOnTypeAndMin(long keyID, string vCode, long pilotID, int type, decimal minimumVal)
        {
            List<WalletJournalEntry> walletEntries = new List<WalletJournalEntry>();

            XmlDocument doc = new XmlDocument();
            try
            {
                string requestURL = string.Format("{0}/char/WalletJournal.xml.aspx?keyID={1}&vCode={2}&characterID={3}&rowCount=2560", _baseURL, keyID, vCode, pilotID);
                doc.Load(requestURL);
                XmlNodeList nodes = doc.GetElementsByTagName("row");

                //Iterate around every wallet journal entry (row under rowset)
                foreach (XmlNode nod in nodes)
                {
                    if (Convert.ToInt32(nod.Attributes["refTypeID"].Value) == type && (Convert.ToDecimal(nod.Attributes["amount"].Value) >= minimumVal || Convert.ToDecimal(nod.Attributes["amount"].Value) <= (minimumVal * -1)))
                    {
                        walletEntries.Add(new WalletJournalEntry()
                        {
                            Amount = Convert.ToDecimal(nod.Attributes["amount"].Value),
                            Sender = nod.Attributes["ownerName1"].Value,
                            Reciever = nod.Attributes["ownerName2"].Value,
                            Reason = nod.Attributes["reason"].Value,
                            TimeStamp = Convert.ToDateTime(nod.Attributes["date"].Value),
                            Type = type
                        });
                    }
                }
            }
            catch (Exception exn)
            {
                throw new Exception("Unable to retrieve data from api");
            }


            return walletEntries;
        }

        /// <summary>
        /// Gets an eve mail body based on the mail id and api key / character id params
        /// </summary>
        /// <param name="keyID">key ID of API Key</param>
        /// <param name="vCode">vCode of API Key</param>
        /// <param name="characterID">CharacterID associated with mail</param>
        /// <param name="mailId">Mail ID to retrieve body of.</param>
        /// <returns>Body of eve mail or error message</returns>
        public static string EVE_GetMailMessageBody(long keyID, string vCode, long characterID, long mailId)
        {
            string body = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                string requestURL = string.Format("{0}/char/MailBodies.xml.aspx?characterID={1}&keyID={2}&vCode={3}&ids={4}", _baseURL, characterID, keyID, vCode, mailId);
                doc.Load(requestURL);
                XmlNodeList nodes = doc.GetElementsByTagName("row");
                body = (nodes.Count > 0) ? nodes[0].InnerText : "Message can not be retrieved from API server!";
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data from api");
            }

            return body;
        }

        /// <summary>
        /// Gets all mail messages for all characters on the api
        /// </summary>
        /// <param name="keyID">KeyID of api to check</param>
        /// <param name="vCode">vCode of api to check</param>
        /// <returns>IEnumerable of mail messages</returns>
        public static IEnumerable<MailMessage> EVE_GetMailMessagesForCharacter(long keyID, string vCode, long characterID)
        {
            List<MailMessage> messages = new List<MailMessage>();
		    XmlDocument doc = new XmlDocument();
            try
            {
                string requestURL = string.Format("{0}/char/MailMessages.xml.aspx?characterID={1}&keyID={2}&vCode={3}", _baseURL, characterID, keyID, vCode);
                doc.Load(requestURL);
                XmlNodeList nodes = doc.GetElementsByTagName("row");

                foreach (XmlNode node in nodes)
                {
                    MailMessage newMail = new MailMessage()
                    {
                        MessageID = Convert.ToInt64(node.Attributes["messageID"].Value),
                        SenderID = Convert.ToInt64(node.Attributes["senderID"].Value),
                        SenderName = node.Attributes["senderName"].Value,
                        SentDate = Convert.ToDateTime(node.Attributes["sentDate"].Value),
                        Subject = node.Attributes["title"].Value,
                        ToCorpAllianceIdList = (node.Attributes["toCorpOrAllianceID"].Value.Split(',')[0] != "") ? Array.ConvertAll(node.Attributes["toCorpOrAllianceID"].Value.Split(','), long.Parse).ToList() : new List<long>(),
                        ToPilotIdList = (node.Attributes["toCharacterIDs"].Value.Split(',')[0] != "") ? Array.ConvertAll(node.Attributes["toCharacterIDs"].Value.Split(','), long.Parse).ToList() : new List<long>(),
                        ToMailingIdList = (node.Attributes["toListID"].Value.Split(',')[0] != "") ? Array.ConvertAll(node.Attributes["toListID"].Value.Split(','), long.Parse).ToList() : new List<long>(),
                        Body = EVE_GetMailMessageBody(keyID, vCode, characterID, Convert.ToInt64(node.Attributes["messageID"].Value))
                    };
                    foreach (long corpAlly in newMail.ToCorpAllianceIdList)
                        newMail.ToCorporationNameList.Add(corpAlly, EVE_GetPilotNameByID(corpAlly));
                    foreach (long pilotID in newMail.ToPilotIdList)
                        newMail.ToPilotNameList.Add(pilotID, EVE_GetPilotNameByID(pilotID));
                    foreach (long mailID in newMail.ToMailingIdList)
                        newMail.ToMailingListNameList.Add(mailID, EVE_GetMailingListNameByID(keyID, vCode, characterID, mailID));
                    messages.Add(newMail);
                }
            }
            catch (Exception exn)
            {
                throw new Exception("Unable to retrieve from api");
            }          

            return messages;
        }

        /// <summary>
        /// Gets a list of skills of a character.
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API vCode</param>
        /// <param name="characterID">CharacterID</param>
        /// <returns>KeyValue pairs. key = SkillID, value = SkillLevel</returns>
        public static Dictionary<int, int> EVE_GetCharactersSkills(long keyID, string vCode, long characterID)
        {
            Dictionary<int, int> skills = new Dictionary<int, int>();

            XmlDocument doc = new XmlDocument();
            try
            {
                string requestURL = string.Format("{0}/char/CharacterSheet.xml.aspx?keyID={1}&vCode={2}&characterID={3}", _baseURL, keyID, vCode, characterID);
                doc.Load(requestURL);

                XmlNodeList skillCollection = doc.SelectNodes("//eveapi/result/rowset[@name = 'skills']/row");
                foreach (XmlNode node in skillCollection)
                {
                    skills.Add(Convert.ToInt32(node.Attributes["typeID"].Value), Convert.ToInt32(node.Attributes["level"].Value));
                }
                return skills;
            }
            catch (Exception exn)
            {
                throw new Exception("Unable to retrieve from api");
            }            
        }

        /// <summary>
        /// Gets corp wallet journal entries that are of a specific type 
        /// </summary>
        /// <param name="keyID">KeyID of api to checks</param>
        /// <param name="vCode">VCode of api to check</param>
        /// <param name="type">Type to check for. Use EVE API docs for args</param>
        /// <returns>IEnumerable of wallet journal entries</returns>
        public static IEnumerable<WalletJournalEntry> EVE_GetCorpWalletTransactionsBasedOnType(long keyID, string vCode, int type)
        {
            List<WalletJournalEntry> walletEntries = new List<WalletJournalEntry>();

            XmlDocument doc = new XmlDocument();
            try
            {
                string requestURL = string.Format("{0}/corp/WalletJournal.xml.aspx?keyID={1}&vCode={2}&rowCount=20000", _baseURL, keyID, vCode);
                doc.Load(requestURL);
                XmlNodeList nodes = doc.GetElementsByTagName("row");

                //Iterate around every wallet journal entry (row under rowset)
                foreach (XmlNode nod in nodes)
                {
                    if (Convert.ToInt32(nod.Attributes["refTypeID"].Value) == type)
                    {
                        walletEntries.Add(new WalletJournalEntry()
                        {
                            Amount = Convert.ToDecimal(nod.Attributes["amount"].Value),
                            Sender = nod.Attributes["ownerName1"].Value,
                            Reciever = nod.Attributes["ownerName2"].Value,
                            Reason = nod.Attributes["reason"].Value,
                            TimeStamp = Convert.ToDateTime(nod.Attributes["date"].Value),
                            Type = type
                        });
                    }
                }
            }
            catch (Exception exn)
            {
                throw new Exception("Unable to retrieve data from api");
            }


            return walletEntries;
        }

        /// <summary>
        /// Does a check on whether the pilot is blacklisted on a CFC level (Region Commander)
        /// </summary>
        /// <param name="pilotName">Pilot to check </param>
        /// <returns>Switch based on whether the pilot is blacklisted</returns>
        public static bool RC_IsPilotBlacklisted(string pilotName)
        {
            WebClient wc = new WebClient();
            try
            {
                string data = wc.DownloadString("https://rc-eve.net/rcbot/cbl/BCZRvxhBpr/" + pilotName);
                JArray obj = JArray.Parse(data);
                return (string)obj[0]["output"] != "NOT FOUND";
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve data from api");
            }
        }
    }

    
}
