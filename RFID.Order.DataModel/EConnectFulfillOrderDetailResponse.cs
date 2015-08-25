using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID.Order.DataModel
{
    public class EConnectFulfillOrderDetailResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int IPCode { get; set; }
    }
}
