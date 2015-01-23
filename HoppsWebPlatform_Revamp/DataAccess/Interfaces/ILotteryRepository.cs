using HoppsWebPlatform_Revamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface ILotteryRepository
    {
        Lottery GetActiveLottery();
        IEnumerable<LotteryTicket> GetTicketsForLottery(int lotteryID);
    }
}
