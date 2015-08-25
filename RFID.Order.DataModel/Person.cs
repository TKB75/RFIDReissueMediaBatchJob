using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFID.Order.DataModel
{
     [Table("Person")]
    public class Person
    {
         public int Id { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public DateTime UpdatedDate { get; set; }
    }
}
