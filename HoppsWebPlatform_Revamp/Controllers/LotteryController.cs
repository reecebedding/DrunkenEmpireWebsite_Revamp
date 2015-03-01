using HoppsWebPlatform_Revamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.Controllers
{
    [Attributes.Authorize(Roles = "Director")]
    public class LotteryController : Controller
    {
        private NLog.Logger _logger;

        private ILotteryRepository _lotteryRepository;        

        public LotteryController(ILotteryRepository lotteryRepository)
        {
            _lotteryRepository = lotteryRepository;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public ActionResult Index()
        {
            Lottery activeLottery = _lotteryRepository.GetActiveLottery();
            if (activeLottery != null)
            {
                activeLottery.Tickets = _lotteryRepository.GetTicketsForLottery(activeLottery.Id);                    
            }
            
            return View(activeLottery);
        }
    }
}
