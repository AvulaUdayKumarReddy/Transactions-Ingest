using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions_Ingest.Data
{
    public class TransactionDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Audit> Audits { get; set; }

        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }
    }
}
