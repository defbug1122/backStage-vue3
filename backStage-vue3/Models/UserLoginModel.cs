﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    public class UserLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SessionId { get; set; }
    }
}