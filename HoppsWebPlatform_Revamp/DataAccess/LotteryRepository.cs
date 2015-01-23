using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class LotteryRepository : DbContext, ILotteryRepository
    {
        #region Properties

        private NLog.Logger _logger;

        #endregion

        #region Mappers

        private IRowMapper<Lottery> _lotteryRowMapperMain;
        private IMapBuilderContext<Lottery> CreateMainLotteryRowMapper()
        {
            return MapBuilder<Lottery>.MapAllProperties().DoNotMap(x => x.Tickets);
        }

        private IRowMapper<LotteryTicket> _lotteryTicketRowMapperMain;
        private IMapBuilderContext<LotteryTicket> CreateMainLotteryTicketRowMapper()
        {
            return MapBuilder<LotteryTicket>.MapAllProperties();
        }

        #endregion

        public LotteryRepository()
        {
            _lotteryRowMapperMain = CreateMainLotteryRowMapper().Build();
            _lotteryTicketRowMapperMain = CreateMainLotteryTicketRowMapper().Build();
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public Lottery GetActiveLottery()
        {
            Lottery lottery = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Lottery_Get_Active_Lottery", _lotteryRowMapperMain).FirstOrDefault();
            return lottery;
        }


        public IEnumerable<LotteryTicket> GetTicketsForLottery(int lotteryID)
        {
            IEnumerable<LotteryTicket> tickets = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Lottery_Get_Lottery_Tickets", _lotteryTicketRowMapperMain, lotteryID);
            return tickets;
        }
    }
}