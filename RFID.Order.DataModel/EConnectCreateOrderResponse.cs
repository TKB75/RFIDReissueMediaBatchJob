using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID.Order.DataModel
{
    public class EConnectCreateOrderResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int SupplierOrderID { get; set; }
        public string DistributorOrderID   { get; set; }
        public int TransactionId { get; set; }
        public EConnectFulfillOrderResponse EConnectFulfillOrderResponse { get; set; }
    }
}
