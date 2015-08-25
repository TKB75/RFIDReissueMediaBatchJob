using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID.Order.DataModel
{
    public class Enums
    {
        public enum OrderStatus
        {
            Pending = 1,
            Processed = 2,
            Error = 3,
            PartialError = 4
        }
    }
}
