using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class ApplicationBackgroundCheckViewModel
    {
        public RecruitmentApplication Application { get; set; }
        public HtmlString CFCBlackListMarkup { get; set; }
        public HtmlString WalletJournalMarkup { get; set; }
    }
}