﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class RecruitmentViewModel
    {
        public IEnumerable<RecruitmentApplicationQuestion> Questions { get; set; }
        public string KeyWords { get; set; }
    }
}