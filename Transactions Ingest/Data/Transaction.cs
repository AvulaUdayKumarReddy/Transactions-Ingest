using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions_Ingest.Data
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public string CardNumberLast4 { get; set; }

        public string LocationCode { get; set; }

        public string ProductName { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionTime { get; set; }

        public string Status { get; set; }

    }
}
