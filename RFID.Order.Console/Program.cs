using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RFID.Order.Service;
using RFID.Order.DataModel;

namespace RFID.Order.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            OrderService svcOrder = new OrderService();

            svcOrder.ProcessOrders();
        }
    }
}
