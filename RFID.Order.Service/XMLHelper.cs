using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using RFID.Order.DataModel;

namespace RFID.Order.Service
{
    public class XMLHelper
    {
        private int _supplierCode = 0;
        private string _supplierName = "";
        private int _distributorCode = 0;
        private string _distributorName = "";
        private OrderService _svcOrder = null;

        public XMLHelper()
        {
            _supplierCode = int.Parse(ConfigurationManager.AppSettings["SupplierCode"].ToString());
            _supplierName = ConfigurationManager.AppSettings["SupplierName"].ToString();
            _distributorCode = int.Parse(ConfigurationManager.AppSettings["DistributorCode"].ToString());
            _distributorName = ConfigurationManager.AppSettings["DistributorName"].ToString();
            _svcOrder = new OrderService();
        }        

        public string AssembleOneUserCreateOrderRequest(RFIDPassMediaReissueDetailStage orderDetail)
        {
            StringBuilder sb = new StringBuilder();
            PersonProfile personProfile = _svcOrder.GetPersonProfileByIPCode(orderDetail.IPCode);

            sb.Append("<RTPSA_Header>");
            sb.Append("<SupplierCode>" + _supplierCode.ToString() + "</SupplierCode>");
            sb.Append("<SupplierName>" + _supplierName + "</SupplierName>");
            sb.Append("<DistributorCode>" + _distributorCode.ToString() + "</DistributorCode>");
            sb.Append("<DistributorName>" + _distributorName + "</DistributorName>");
            sb.Append("<MessageType>Order</MessageType>");
            sb.Append("<MessageAction>CreateOrder</MessageAction>");
            sb.Append("<Target>Test</Target>");
            sb.Append("<RTPSA_MessageBody>");
            sb.Append("<RTPSA_CreateOrderRQ>");
            sb.Append("<DistributorOrderId>" + Guid.NewGuid().ToString().Replace("-", "") + "</DistributorOrderId>");
            sb.Append("<ArrivalDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ArrivalDate>");
            sb.Append("<DepartureDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</DepartureDate>");
            sb.Append("<Customer>");
            sb.Append("<CustomerId>" + orderDetail.IPCode.ToString() + "</CustomerId>");
            sb.Append("<LastName>" + personProfile.LastName + "</LastName>");
            sb.Append("<FirstName>" + personProfile.FirstName + "</FirstName>"); 
            sb.Append("</Customer>"); 
            sb.Append("<LineItems>");
            sb.Append("<LineItem>");
            sb.Append("<DistributorLineNumber>" + orderDetail.IPCode.ToString() + "</DistributorLineNumber>");
            sb.Append("<ProductCode>" + orderDetail.PassMediaProductHeaderCode + "</ProductCode>"); // 16329
            sb.Append("<ProductDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ProductDate>");
            sb.Append("<Quantity>1</Quantity>");
            sb.Append("<Price>0</Price>"); //23.58
            sb.Append("<CurrencyCode>USD</CurrencyCode>");
            sb.Append("</LineItem>");
            sb.Append("</LineItems>");
            sb.Append("</RTPSA_CreateOrderRQ>");
            sb.Append("</RTPSA_MessageBody>");
            sb.Append("</RTPSA_Header>");


            return sb.ToString();
        }

        public string AssembleMultiUserCreateOrderRequest(List<RFIDPassMediaReissueDetailStage> orderDetails)
        {
            StringBuilder sb = new StringBuilder();
            PersonProfile personProfile = _svcOrder.GetPersonProfileByIPCode(orderDetails[0].PrimaryIPCode);

            sb.Append("<RTPSA_Header>");
            sb.Append("<SupplierCode>" + _supplierCode.ToString() + "</SupplierCode>");
            sb.Append("<SupplierName>" + _supplierName + "</SupplierName>");
            sb.Append("<DistributorCode>" + _distributorCode.ToString() + "</DistributorCode>");
            sb.Append("<DistributorName>" + _distributorName + "</DistributorName>");
            sb.Append("<MessageType>Order</MessageType>");
            sb.Append("<MessageAction>CreateOrder</MessageAction>");
            sb.Append("<Target>Test</Target>");
            sb.Append("<RTPSA_MessageBody>");
            sb.Append("<RTPSA_CreateOrderRQ>");
            sb.Append("<DistributorOrderId>" + Guid.NewGuid().ToString().Replace("-", "") + "</DistributorOrderId>");
            sb.Append("<ArrivalDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ArrivalDate>");
            sb.Append("<DepartureDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</DepartureDate>");
            sb.Append("<Customer>");
            sb.Append("<CustomerId>" + orderDetails[0].IPCode.ToString() + "</CustomerId>");
            sb.Append("<LastName>" + personProfile.LastName + "</LastName>");
            sb.Append("<FirstName>" + personProfile.FirstName + "</FirstName>"); 
            sb.Append("</Customer>"); 
            sb.Append("<LineItems>");

            foreach(RFIDPassMediaReissueDetailStage orderDetail in orderDetails)
            {
                personProfile = _svcOrder.GetPersonProfileByIPCode(orderDetail.IPCode);

                sb.Append("<LineItem>");
                sb.Append("<DistributorLineNumber>" + orderDetail.IPCode.ToString() + "</DistributorLineNumber>");
                sb.Append("<ProductCode>" + orderDetail.PassMediaProductHeaderCode + "</ProductCode>"); //16329
                sb.Append("<ProductDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ProductDate>");
                sb.Append("<Quantity>1</Quantity>");
                sb.Append("<Price>0</Price>");
                sb.Append("<CurrencyCode>USD</CurrencyCode>");
                sb.Append("<Customer>");
                sb.Append("<CustomerId>" + orderDetail.IPCode.ToString() + "</CustomerId>");
                sb.Append("<LastName>" + personProfile.LastName + "</LastName>");
                sb.Append("<FirstName>" + personProfile.FirstName + "</FirstName>"); 
                sb.Append("</Customer>");
                sb.Append("</LineItem>");
            }

            sb.Append("</LineItems>");
            sb.Append("</RTPSA_CreateOrderRQ>");
            sb.Append("</RTPSA_MessageBody>");
            sb.Append("</RTPSA_Header>");

            return sb.ToString();
        }

        public string AssembleFulfillOrderRequest(List<RFIDPassMediaReissueDetailStage> orderDetails, string distributorOrderID)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<RTPSA_Header>");
            sb.Append("<SupplierCode>" + _supplierCode.ToString() + "</SupplierCode>");
            sb.Append("<SupplierName>" + _supplierName + "</SupplierName>");
            sb.Append("<DistributorCode>" + _distributorCode.ToString() + "</DistributorCode>");
            sb.Append("<DistributorName>" + _distributorName + "</DistributorName>");
            sb.Append("<MessageType>Order</MessageType>");
            sb.Append("<MessageAction>FulfillOrder</MessageAction>");
            sb.Append("<Target>Test</Target>");
            sb.Append("<RTPSA_MessageBody>");

            sb.Append("<RTPSA_FulfillOrderRQ>");
            sb.Append("<DistributorOrderId>" + distributorOrderID + "</DistributorOrderId>");
            sb.Append("<ReturnPrinterSpecificOutput>N</ReturnPrinterSpecificOutput>");
            sb.Append("<EncodePrinterOutputBase64>N</EncodePrinterOutputBase64>");
            sb.Append("<LineItems>");

            foreach (RFIDPassMediaReissueDetailStage orderDetail in orderDetails)
            {
                sb.Append("<LineItem>");
                sb.Append("<DistributorLineNumber>" + orderDetail.IPCode.ToString() + "</DistributorLineNumber>");
                sb.Append("</LineItem>");
            }

            sb.Append("</LineItems>");
            sb.Append("</RTPSA_FulfillOrderRQ>");
            sb.Append("</RTPSA_MessageBody>");
            sb.Append("</RTPSA_Header>");

            return sb.ToString();
        }
    }
}
