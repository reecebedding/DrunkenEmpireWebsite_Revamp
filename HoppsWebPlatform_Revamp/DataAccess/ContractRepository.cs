using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class ContractRepository : DbContext, IContractRepository
    {
        private IRowMapper<Contract> _contractMapperMain;
        private IRowMapper<ContractContent> _contractContentMapperMain;
        private IRowMapper<ContractContent> _contractCatalogueContentMapperMain;

        private IMapBuilderContext<Contract> CreateContractMainMapper(){ return MapBuilder<Contract>.MapAllProperties().DoNotMap(x => x.TotalBeforeReduction); }
        private IMapBuilderContext<ContractContent> CreateContractContentMainMapper() { return MapBuilder<ContractContent>.MapAllProperties(); }
        private IMapBuilderContext<ContractContent> CreateContractCatalogueContentMainMapper() 
        {
            return MapBuilder<ContractContent>.MapNoProperties().MapByName(x => x.ItemID).MapByName(x => x.ItemName);
        }

        public ContractRepository()
        {
            _contractMapperMain = CreateContractMainMapper().Build();
            _contractContentMapperMain = CreateContractContentMainMapper().Build();
            _contractCatalogueContentMapperMain = CreateContractCatalogueContentMainMapper().Build();
        }

        /// <summary>
        /// Gets all contracts by status
        /// </summary>
        /// <param name="status">Status of contract to get</param>
        /// <returns>Collection of contracts</returns>
        public IEnumerable<Contract> GetContractsByStatus(string status)
        {
            List<Contract> contracts = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Contract_Get_Contract_By_Status_And_Name", _contractMapperMain, status, null).ToList();
            foreach (Contract cont in contracts)
                cont.Content = GetContentsOfContract(cont.UID);
            
            return contracts;
        }

        /// <summary>
        /// Gets all contracts by creator (user) and status
        /// </summary>
        /// <param name="userName">Username to get contracts for</param>
        /// <param name="status">Status of contract to get</param>
        /// <returns>Collection of contracts</returns>
        public IEnumerable<Contract> GetContractsByStatus(string userName, string status)
        {
            List<Contract> contracts = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Contract_Get_Contract_By_Status_And_Name", _contractMapperMain, status, userName).ToList();            
            foreach (Contract cont in contracts)
                cont.Content = GetContentsOfContract(cont.UID);
            
            return contracts;
        }

        /// <summary>
        /// Creates a new contract
        /// </summary>
        /// <param name="newContract">New contract to add</param>
        /// <returns>New contract ID</returns>
        public int CreateNewContract(Contract newContract)
        {
            object result = HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_Contract_Create_New_Contract", newContract.Creator, newContract.Total);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Adds content for a contract
        /// </summary>
        /// <param name="newContractContent">Contract content to add</param>
        public void CreateNewContractItem(ContractContent newContractContent)
        {
            HoppsWebPlatformRevampDatabase.ExecuteNonQuery("Web_Contract_Create_New_ContractContent", newContractContent.ContractID, newContractContent.ItemID, newContractContent.PricePerUnit, newContractContent.Quantity);            
        }

        /// <summary>
        /// Gets the contents for a contract
        /// </summary>
        /// <param name="contractID">ID of contract to get contents for</param>
        /// <returns>Collection of contract contents</returns>
        public IEnumerable<ContractContent> GetContentsOfContract(int contractID)
        {
            IEnumerable<ContractContent> contractContents = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Contract_Get_Contract_Contents_By_Contract_ID", _contractContentMapperMain, contractID);
            return contractContents;
        }

        /// <summary>
        /// Gets a list of contract items that are available for buying
        /// </summary>
        /// <returns>List of contract items</returns>
        public IEnumerable<ContractContent> GetContentCatalog()
        {
            IEnumerable<ContractContent> contractContents = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Contract_Get_Contract_Contents_Catalogue", _contractCatalogueContentMapperMain);
            return contractContents;
        }
    }
}