using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface IContractRepository
    {
        IEnumerable<Contract> GetContractsByStatus(string status);
        IEnumerable<Contract> GetContractsByStatus(string userName, string status);
        IEnumerable<ContractContent> GetContentsOfContract(int contractID);
        IEnumerable<ContractContent> GetContentCatalog();
        int CreateNewContract(Contract newContract);
        void CreateNewContractItem(ContractContent newContractContent);
    }
}
