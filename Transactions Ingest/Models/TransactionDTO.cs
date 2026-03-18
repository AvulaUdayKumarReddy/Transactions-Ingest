using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions_Ingest.Models
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }

        public string CardNumber { get; set; }

        public string LocationCode { get; set; }

        public string ProductName { get; set; }

        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set;  }
    }
}
