using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backStage_vue3.Models
{
    public class UpdateDeliveryMethodModel
    {
        public int OrderId { get; set; }
        public byte DeliveryMethod { get; set; }
        public string DeliveryAddress { get; set; }
    }
}