using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace RFID.Order.DataModel
{
    public class RTPContext : DbContext
    {
        public RTPContext()
            : base("Name=RTPContext")
        {

        }
        public DbSet<PersonProfile> PersonProfiles { get; set; }
        public DbSet<RFIDPassMediaReissueStage> RFIDPassMediaReissueStages { get; set; }
        public DbSet<RFIDPassMediaReissueDetailStage> RFIDPassMediaReissueDetailStages { get; set; }
    }
}
