using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.Utilities;
using Newtonsoft.Json;
using System.Xml;
using System.Text;
namespace HoppsWebPlatform_Revamp.Controllers
{
    [Attributes.Authorize(Roles = "Director")]
    public class JewTrackerController : Controller
    {
        private IAppSettingsRepository _appSettingsRepository;
        private IAltRepository _altRepository;
        private NLog.Logger _logger;
       
        public JewTrackerController(IAppSettingsRepository appSettingsRepository, IAltRepository altRepository)
        {
            _appSettingsRepository = appSettingsRepository;
            _altRepository = altRepository;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public ActionResult Index()
        {
            ViewBag.DateFrom = DateTime.Now.AddMonths(-1);
            ViewBag.DateTo = DateTime.Now;
            return View();
        }

        /// <summary>
        /// Main index
        /// </summary>
        /// <param name="dateFrom">date from to search by, can be null for default</param>
        /// <param name="dateTo">date to to search to, can be null for default</param>
        /// <returns>Index page with initial search</returns>
        [HttpPost]
        public ActionResult Index(DateTime dateFrom, DateTime dateTo)
        {
            //If either date is null, populate with the default 1 month period
            if (dateFrom == null)
                dateFrom = DateTime.Now.AddMonths(-1);
            if (dateTo == null)
                dateTo = DateTime.Now;

            //Get director key to parse to retrieve wallet details
            ApiKey directorKey = _appSettingsRepository.GetCorporationAPI();
            //Get all wallet journal entries with type 85 (rat bounty)
            IEnumerable<WalletJournalEntry> corpWalletJournal = Utilities.APIHelper.EVE_GetCorpWalletTransactionsBasedOnType(directorKey.KeyID, directorKey.VCode, 85);
            
            Dictionary<string, decimal> pilotVals = new Dictionary<string, decimal>();
            JewTrackerIndexViewModel viewModel = new JewTrackerIndexViewModel();

            //Calculate unique pilot name and values from entire wallet journal
            foreach (WalletJournalEntry wallet in corpWalletJournal)
            {
                if (wallet.TimeStamp >= dateFrom && wallet.TimeStamp <= dateTo)
                {
                    if (pilotVals.Keys.Contains(wallet.Reciever))
                        pilotVals[wallet.Reciever] = pilotVals[wallet.Reciever] + wallet.Amount;
                    else
                        pilotVals.Add(wallet.Reciever, wallet.Amount);
                }
            }

            //Collate list into mains, and values sum
            pilotVals = ColateListToMains(pilotVals);

            //Sort the list in descending value
            List<KeyValuePair<string, decimal>> sortedList = pilotVals.OrderBy(x => x.Value).Reverse().ToList();
            pilotVals = sortedList.ToDictionary(x => x.Key, x => x.Value);

            //Build the view model
            if (pilotVals.Count() > 0)
            {
                viewModel.pilotValues = pilotVals;
                viewModel.totalValue = pilotVals.Select(x => x.Value).Sum();
                viewModel.topTaxedPilot = pilotVals.Where(x => x.Value == pilotVals.Max(y => y.Value)).First().Key;
                viewModel.topTaxedAmount = pilotVals.Where(x => x.Value == pilotVals.Max(y => y.Value)).First().Value;
            }
            else
                viewModel.pilotValues = new Dictionary<string, decimal>();


            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            return View(viewModel);
        }

        /// <summary>
        /// Takes a dictionary of pilots and values, and returns a list of mains and sum of alt + main values
        /// </summary>
        /// <param name="values">Un-paired list of alts</param>
        /// <returns>List of mains, and collated values of alt values</returns>
        private Dictionary<string, decimal> ColateListToMains(Dictionary<string, decimal> values)
        {
            Dictionary<string, decimal> sortedValues = new Dictionary<string, decimal>();

            foreach (KeyValuePair<string, decimal> pilot in values)
            {
                //Get alt of pilot
                Alt alt = _altRepository.GetAlt(pilot.Key);
                //If alt is not null, the pilot is an alt, and we can establish the main the value should belong to
                if (alt != null)
                {
                    //If the list contains the main already, increment it
                    if (sortedValues.Where(x => x.Key == alt.MainName).Any())
                        sortedValues[alt.MainName] += pilot.Value;
                    //otherwise, add it as a new main
                    else
                        sortedValues.Add(alt.MainName, pilot.Value);
                }
                else
                {
                    //Same as above method, but treats the pilot in the loop as the main, seeing as the pilot is not an alt
                    if (sortedValues.Where(x => x.Key == pilot.Key).Any())
                        sortedValues[pilot.Key] += pilot.Value;
                    else
                        sortedValues.Add(pilot.Key, pilot.Value);
                }
            }
            return sortedValues;
        }
    }
}
