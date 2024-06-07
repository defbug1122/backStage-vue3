using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    public class UserModel
    {
        public int f_id { get; set; }
        public string f_userName { get; set; }
        public string f_password { get; set; }
        public DateTime? f_createTime { get; set; }
    }
}