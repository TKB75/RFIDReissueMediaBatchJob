using System;
using System.Collections.Generic;


namespace RFID.Order.DataModel
{
    public class EConnectFulfillOrderResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int TransactionId { get; set; }

        public List<EConnectFulfillOrderDetailResponse> EConnectFulfillOrderDetailResponses { get; set; }

        public EConnectFulfillOrderResponse()
        {
            EConnectFulfillOrderDetailResponses = new List<EConnectFulfillOrderDetailResponse>();
        }
    }
}
