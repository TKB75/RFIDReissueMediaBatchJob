using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFID.Order.DataModel
{
    [Table("RFIDPassMediaReissueStage")]
    public class RFIDPassMediaReissueStage
    {
        [Key]
        public int RFIDPassMediaReissueID { get; set; }
        public int PrimaryIPCode { get; set; }
        public int StatusID { get; set; }
        public string ErrorMessage { get; set; }
        public int? OrderID { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}

