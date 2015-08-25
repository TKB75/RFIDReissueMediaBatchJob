using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFID.Order.DataModel
{
     [Table("RFIDPassMediaReissueDetailStage")]
    public class RFIDPassMediaReissueDetailStage
    {
        [Key]
        public int RFIDPassMediaReissueDetailID { get; set; }
        public int PrimaryIPCode { get; set; }
        public int IPCode { get; set; }
        public string PassMediaProductHeaderCode { get; set; }
        public int? StatusID { get; set; }
        public string ErrorMessage { get; set; }
    }
}
