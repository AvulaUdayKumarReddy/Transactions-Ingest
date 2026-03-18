using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions_Ingest.Data
{
    public class Audit
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        public string Name { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
