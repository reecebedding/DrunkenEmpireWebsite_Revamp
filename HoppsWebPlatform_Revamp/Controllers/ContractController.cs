using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using System.Web.Security;

namespace HoppsWebPlatform_Revamp.Controllers
{
    [Authorize(Roles="CorporationMember")]
    public class ContractController : Controller
    {
        private IContractRepository _contractRepository;
        private IEveDBRepository _eveDBRepository;

        public ContractController(IContractRepository contractRepository, IEveDBRepository eveDBRepository)
        {
            _contractRepository = contractRepository;
            _eveDBRepository = eveDBRepository;
        }

        /// <summary>
        /// Gets index view containing personal contracts and total contracts if the user is a director
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ContractViewModel viewmodel = new ContractViewModel();
            //Users personal contracts where they are Outstanding... ideally this isnt a hardcoded value, but the status' of contracts isnt going to change much
            viewmodel.PersonalContracts = _contractRepository.GetContractsByStatus(User.Identity.Name, "OUTSTANDING");
            if (Roles.IsUserInRole("Director"))
                //If the user is a director, then they can see all of the outstanding contracts
                viewmodel.AdminContracts = _contractRepository.GetContractsByStatus("OUTSTANDING");
            
            return View(viewmodel);
        }

        /// <summary>
        /// New contract view
        /// </summary>
        /// <returns>new contract view</returns>
        public ActionResult NewContract()
        {
            return View(); 
        }

        /// <summary>
        /// Creates the initial contract based on the users chosen items
        /// </summary>
        /// <param name="contractItem">List of contract items</param>
        /// <returns>Contract confirmation view</returns>
        [HttpPost]
        public ActionResult NewContract(IEnumerable<ContractContent> contractItem)
        {
            ModelState.Clear();

            Contract newContract = new Contract() { Creator = User.Identity.Name };
            List<ContractContent> correctVals = new List<ContractContent>();

            //If the user never passed any contract items, set the list to blank so it can be checked later to pass back error
            contractItem = contractItem ?? new List<ContractContent>();

            //Do an initial check on parsed items to determin complete collated quantities
            foreach (ContractContent content in contractItem)
            {
                //If the contract item has an id, and has values add it to the list
                if (content.ItemID > 0 && content.Quantity > 0)
                {
                    content.ItemName = _eveDBRepository.GetItemNameByID(content.ItemID);
                    correctVals.Add(content);
                }                
            }

            //Do a model check on the new list of contract values after collating all of the parsed values
            if (TryValidateModel(correctVals))
            {
                if (correctVals.Count == 0)
                {
                    //Sets the viewbag to have the list of the contract catalog items to populate the dropdowns
                    ViewBag.ContractItemTypes = _contractRepository.GetContentCatalog();
                    ModelState.AddModelError("", "You need to add items to the contract");
                    return View(contractItem);
                }

                List<ContractContent> contractContent = new List<ContractContent>();

                //Only iterate around the items that have a quantity and id, so we ignore all the bad items
                foreach (ContractContent cont in contractItem.Where(x => (x.Quantity > 0) && (x.ItemID > 0)))
                {
                    //If the final collection (Contract Content) has the item existing, increase its quantity, otherwise add it to the collection
                    if (contractContent.Any(x => x.ItemID == cont.ItemID))
                        contractContent.Where(x => x.ItemID == cont.ItemID).First().Quantity += cont.Quantity;
                    else
                        //Add the new contract content item to the final collection
                        contractContent.Add(cont);
                }
                //Add the list of contract content to the final model view
                newContract.Content = contractContent;

                foreach (ContractContent content in newContract.Content)
                {
                    //Foreach of the final items, get the price value for each unit and increate the total before the x% tax reduction
                    content.PricePerUnit = Utilities.EVECentralHelper.GetItemPercentilePriceForRegion(Utilities.EVECentralHelper.Regions.Forge, content.ItemID);
                    content.ItemName = _eveDBRepository.GetItemNameByID(content.ItemID);
                    newContract.TotalBeforeReduction += (content.PricePerUnit * content.Quantity);
                }

                //Total value after the x% reduction for corp
                newContract.Total = newContract.TotalBeforeReduction - (newContract.TotalBeforeReduction * 0.05m);

                //Add the contract to the session .. this is VERY important as it prevents the user from tampering with the final contract and it means we dont have to re-calculate the values.
                HttpContext.Session["newContract"] = newContract;

                return View("ConfirmNewContract", newContract);
            }

            //If we got here, the modelstate was invalid.
            ViewBag.ContractItemTypes = _contractRepository.GetContentCatalog();
            ModelState.AddModelError("", "All items must have a quantity");
            return View(correctVals);
        }

        /// <summary>
        /// Finializes the contract creation after the initial contract has been calculated
        /// </summary>
        /// <param name="result">Flag to show whether the new contract should be created or not</param>
        /// <returns>Index view</returns>
        public ActionResult SubmitNewContract(int result)
        {
            Contract newContract = (Contract)HttpContext.Session["newContract"];
            //Checks if the result is to proceed 0 = Cancel, 1 = go ahead and finish
            if (result > 0)
            {
                int newContID = _contractRepository.CreateNewContract(newContract);
                foreach (ContractContent content in newContract.Content)
                {
                    content.ContractID = newContID;
                    _contractRepository.CreateNewContractItem(content);
                }
            }
            else
            {
                HttpContext.Session["newContract"] = null;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets a serialized list of contract items that are accepted
        /// </summary>
        /// <returns>JSon serialized list of contract items</returns>
        public JsonResult GetContractCatalogueItems()
        {
            IEnumerable<ContractContent> items = _contractRepository.GetContentCatalog();
            var obj = new Newtonsoft.Json.Linq.JObject();
            obj.Add("Values", Newtonsoft.Json.JsonConvert.SerializeObject(items));
            JsonResult result = new JsonResult() { Data = obj.ToString() };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

    }
}
