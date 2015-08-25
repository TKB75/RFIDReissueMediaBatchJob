using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Runtime.Serialization;
using System.Net.Http;
using System.Threading.Tasks;

using RFID.Order.DataModel;
using System.Data.Entity;

namespace RFID.Order.Service
{
    public class OrderService
    {
        public void Test()
        {
            //int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"].ToString());

            //var person = new Person {FirstName = "Steven", 
            //LastName = "Smith", UpdatedDate = DateTime.Now};
            //using (var context = new RTPContext())
            //{
            //   // context.Persons.Add(person);
            //    //context.SaveChanges();

            //    var persons = context.Persons.SqlQuery("SELECT TOP " + batchSize.ToString() + " * FROM Person").ToList<Person>();
            //}
            //Console.Write("Person saved !");
            //Console.ReadLine();
        }

        public void ProcessOrders()
        {
            int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"].ToString());

            Console.Write(string.Format("Batch Size: {0}", batchSize));
            Console.ReadLine();
            Console.WriteLine(); 

            EConnectCreateOrderResponse response = null;
            List<RFIDPassMediaReissueStage> orders = GetRFIDPassMediaReissueStageByStatus(Enums.OrderStatus.Pending);
            List<RFIDPassMediaReissueDetailStage> orderDetails = null;

            foreach (RFIDPassMediaReissueStage order in orders)
            {
                orderDetails = GetRFIDPassMediaReissueDetailStageByPrimaryIPCode(order.PrimaryIPCode);

                //Process request
                try
                {
                    response = ProcessRequest(orderDetails);
                }
                catch(Exception ex)
                {
                    response = new EConnectCreateOrderResponse();
                    response.ResponseCode = 1;
                    response.ResponseMessage = ex.Message;
                }
                
                //Update DB
                try
                {
                    using (var context = new RTPContext())
                    {
                        order.StatusID = response.ResponseCode == 0 ? Enums.OrderStatus.Processed.GetHashCode() : Enums.OrderStatus.Error.GetHashCode();

                        //Parent record is success but there was an error with at least one of the household users
                        if (order.StatusID == Enums.OrderStatus.Processed.GetHashCode() && response.EConnectFulfillOrderResponse.EConnectFulfillOrderDetailResponses.Where(d => d.ResponseCode != 0).Count() > 0)
                        {
                            order.StatusID = Enums.OrderStatus.PartialError.GetHashCode();
                        }

                        order.UpdateDate = DateTime.Now;
                        order.ErrorMessage = response.ResponseMessage;
                        if (response.SupplierOrderID > 0)
                        {
                            order.OrderID = response.SupplierOrderID;
                        }

                        context.RFIDPassMediaReissueStages.Attach(order);
                        context.Entry(order).State = EntityState.Modified;

                        //If null then the order was never created so don't update RFIDPassMediaReissueDetailStage table
                        if (response.EConnectFulfillOrderResponse != null)
                        {
                            foreach (RFIDPassMediaReissueDetailStage detail in orderDetails)
                            {
                                EConnectFulfillOrderDetailResponse responseDetail = response.EConnectFulfillOrderResponse.EConnectFulfillOrderDetailResponses.Where(d => d.IPCode == detail.IPCode).FirstOrDefault();

                                detail.ErrorMessage = responseDetail.ResponseMessage;
                                detail.StatusID = responseDetail.ResponseCode == 0 ? Enums.OrderStatus.Processed.GetHashCode() : Enums.OrderStatus.Error.GetHashCode();

                                context.RFIDPassMediaReissueDetailStages.Attach(detail);
                                context.Entry(detail).State = EntityState.Modified;
                            }
                        }

                        context.SaveChanges();

                        if (order.StatusID == Enums.OrderStatus.Processed.GetHashCode())
                        {
                            Console.WriteLine(string.Format("SUCCESS for PrimaryIPCode: {0}", order.PrimaryIPCode));
                        }
                        else
                        {
                            Console.WriteLine(string.Format("ERROR for PrimaryIPCode: {0}", order.PrimaryIPCode));
                        }
                        Console.WriteLine();
                    }
                }
                catch(Exception ex)
                {
                    string message = ex.Message + "\r\n" + ex.StackTrace + "\r\n DistributorOrderID: " + (string.IsNullOrEmpty(response.DistributorOrderID) ? "" : response.DistributorOrderID);
                    message += "\r\n PrimaryIPCode:" + order.PrimaryIPCode.ToString();
                        
                  //  System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\RFID.Order.Console_ERROR.txt", true);
                   // file.WriteLine(message);
                    //file.Close();

                    //throw ex;

                    
                    Console.WriteLine("Exception during SQL save: " + message);
                    Console.ReadLine();
                    //Console.WriteLine();
                }
            }
        }



        private List<RFIDPassMediaReissueStage> GetRFIDPassMediaReissueStageByStatus(Enums.OrderStatus status)
        {
            int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"].ToString());

            List<RFIDPassMediaReissueStage> orders = new List<RFIDPassMediaReissueStage>();

            Console.WriteLine(string.Format("BEGIN Get RFIDPassMediaReissueStage data from SQL Server"));
            Console.WriteLine(); 

            using (var context = new RTPContext())
            {
                orders = context.RFIDPassMediaReissueStages.SqlQuery("SELECT TOP " + batchSize.ToString() + " * FROM RFIDPassMediaReissueStage WHERE StatusID = " + status.GetHashCode().ToString()).ToList<RFIDPassMediaReissueStage>();
            }

            Console.WriteLine(string.Format("END Get RFIDPassMediaReissueStage data from SQL Server"));
            Console.WriteLine(); 

            return orders;
        }

        private List<RFIDPassMediaReissueDetailStage> GetRFIDPassMediaReissueDetailStageByPrimaryIPCode(int primaryIPCode)
        {
            List<RFIDPassMediaReissueDetailStage> orderDetails = new List<RFIDPassMediaReissueDetailStage>();

            Console.WriteLine(string.Format("BEGIN Get RFIDPassMediaReissueDetailStage data  (PrimaryIPCode: {0} ) from SQL Server", primaryIPCode));
            Console.WriteLine(); 

            using (var context = new RTPContext())
            {
                orderDetails = context.RFIDPassMediaReissueDetailStages.SqlQuery("SELECT * FROM RFIDPassMediaReissueDetailStage WHERE PrimaryIPCode = " + primaryIPCode.ToString()).ToList<RFIDPassMediaReissueDetailStage>();
            }

            Console.WriteLine(string.Format("END Get RFIDPassMediaReissueDetailStage data  (PrimaryIPCode: {0} ) from SQL Server", primaryIPCode));
            Console.WriteLine(); 

            return orderDetails;
        }

        public PersonProfile GetPersonProfileByIPCode(int ipCode)
        {
            List<PersonProfile> personProfiles = new List<PersonProfile>();

            Console.WriteLine(string.Format("BEGIN Get PersonProfile data from SQL Server"));
            Console.WriteLine(); 

            using (var context = new RTPContext())
            {
                personProfiles = context.PersonProfiles.SqlQuery("SELECT IPCode, ProfileTypeCode, SequenceNumber, FirstName, LastName FROM PersonProfile WHERE StatusCode = 1 AND IPCode = " + ipCode.ToString() + " ORDER BY UpdateDate DESC").ToList<PersonProfile>();
            }

            Console.WriteLine(string.Format("END Get PersonProfile data from SQL Server"));
            Console.WriteLine(); 

            return personProfiles.FirstOrDefault();
        }

        private EConnectCreateOrderResponse ExecuteConnectCreateOrder(string requestXML)
        {
            EConnectCreateOrderResponse response = new EConnectCreateOrderResponse();
            ConnectService.SupplierServiceSoapClient myConnect = new ConnectService.SupplierServiceSoapClient();

            Console.WriteLine(string.Format("BEGIN Invoke Connect Service to create order"));
            Console.WriteLine(); 

            string responseXML = myConnect.ProcessMessage(requestXML);

            Console.WriteLine(string.Format("END Invoke Connect Service to create order"));
            Console.WriteLine(); 

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(responseXML);

            XmlNodeList xnList = xml.SelectNodes("/RTPSA_CreateOrderRS");

            foreach (XmlNode xn in xnList)
            {
                XmlNode nodeCode = xn.SelectSingleNode("ResponseCode");
                XmlNode nodeMessage = xn.SelectSingleNode("ResponseMessage");
                XmlNode nodeOrderID = xn.SelectSingleNode("SupplierOrderId");
                XmlNode nodeOrder = xn.SelectSingleNode("Order"); 

                if (nodeCode != null)
                {
                    response.ResponseCode = int.Parse(nodeCode.InnerText);           
                }
                if (nodeMessage != null)
                {
                    response.ResponseMessage = nodeMessage.InnerText;
                }
                if (nodeOrderID != null)
                {
                    response.SupplierOrderID = int.Parse(nodeOrderID.InnerText);
                }
                if (nodeOrder != null && nodeOrder.Attributes["DistributorOrderId"] != null)
                {
                    response.DistributorOrderID = nodeOrder.Attributes["DistributorOrderId"].Value;
                }
            }

            return response;
        }

        private EConnectFulfillOrderResponse ExecuteConnectFulfillOrder(string requestXML)
        {
            EConnectFulfillOrderResponse response = new EConnectFulfillOrderResponse();
            ConnectService.SupplierServiceSoapClient myConnect = new ConnectService.SupplierServiceSoapClient();

            Console.WriteLine(string.Format("BEGIN Invoke Connect Service to fulfill order"));
            Console.WriteLine(); 

            string responseXML = myConnect.ProcessMessage(requestXML);

            Console.WriteLine(string.Format("END Invoke Connect Service to fulfill order"));
            Console.WriteLine(); 

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(responseXML);

            XmlNodeList xnList = xml.SelectNodes("/RTPSA_FulfillOrderRS");
            XmlNodeList xnListLineItems = xml.SelectNodes("/RTPSA_FulfillOrderRS/Order/LineItems/LineItem");

            foreach (XmlNode xn in xnList)
            {
                XmlNode nodeCode = xn.SelectSingleNode("ResponseCode");
                XmlNode nodeMessage = xn.SelectSingleNode("ResponseMessage");
                XmlNode nodeTransaction = xn.SelectSingleNode("TransactionId");
                
                if (nodeCode != null)
                {
                    response.ResponseCode = int.Parse(nodeCode.InnerText);
                }
                if (nodeMessage != null)
                {
                    response.ResponseMessage = nodeMessage.InnerText;
                }
                if (nodeTransaction != null)
                {
                    response.TransactionId = int.Parse(nodeTransaction.InnerText);
                }
            }

            foreach (XmlNode xn in xnListLineItems)
            {
                EConnectFulfillOrderDetailResponse detailResponse = new EConnectFulfillOrderDetailResponse();
                XmlNode nodeCode = xn.SelectSingleNode("ResponseCode");
                XmlNode nodeMessage = xn.SelectSingleNode("ResponseMessage");

                if (nodeCode != null)
                {
                    detailResponse.ResponseCode = int.Parse(nodeCode.InnerText);
                }
                if (nodeMessage != null)
                {
                    detailResponse.ResponseMessage = nodeMessage.InnerText;
                }

                detailResponse.IPCode = int.Parse(xn.Attributes["DistributorLineNumber"].Value);

                response.EConnectFulfillOrderDetailResponses.Add(detailResponse);
            }

            return response;
        }

        private EConnectCreateOrderResponse ProcessRequest(List<RFIDPassMediaReissueDetailStage> orderDetails)
        {
            string xmlRequestCreateOrder = "";
            string xmlRequestFulfillOrder = "";
            XMLHelper xmlHelper = new XMLHelper();
            EConnectCreateOrderResponse createOrderResponse = null;
            EConnectFulfillOrderResponse fulfillOrderResponse = null;

            //Asssemble XML request
            xmlRequestCreateOrder = orderDetails.Count == 1 ? xmlHelper.AssembleOneUserCreateOrderRequest(orderDetails[0]) : xmlHelper.AssembleMultiUserCreateOrderRequest(orderDetails);

            //Create Order
            createOrderResponse = ExecuteConnectCreateOrder(xmlRequestCreateOrder);

            //Fulfill Order
            if (createOrderResponse.ResponseCode == 0)
            {
                xmlRequestFulfillOrder = xmlHelper.AssembleFulfillOrderRequest(orderDetails, createOrderResponse.DistributorOrderID);

                fulfillOrderResponse = ExecuteConnectFulfillOrder(xmlRequestFulfillOrder);

                createOrderResponse.ResponseCode = fulfillOrderResponse.ResponseCode;
                createOrderResponse.ResponseMessage = fulfillOrderResponse.ResponseMessage;
                createOrderResponse.TransactionId = fulfillOrderResponse.TransactionId;
                createOrderResponse.EConnectFulfillOrderResponse = fulfillOrderResponse;
            }

            return createOrderResponse;
        }
    }
}
