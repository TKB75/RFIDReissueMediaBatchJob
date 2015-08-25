using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFID.Order.DataModel
{
    [Table("PersonProfile")]
    public class PersonProfile
    {
        [Key, Column(Order = 0)]
        public int IPCode { get; set; }
        [Key, Column(Order = 1)]
        public int ProfileTypeCode { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SequenceNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public DateTime UpdateDate { get; set; }
    }
}
