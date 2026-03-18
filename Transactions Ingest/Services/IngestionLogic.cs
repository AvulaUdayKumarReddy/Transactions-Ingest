using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Transactions_Ingest.Data;
using Transactions_Ingest.Models;

namespace Transactions_Ingest.Services
{
    public class IngestionLogic
    {
        private readonly TransactionDbContext _db;
        private readonly MockApi _mockApi;

        public IngestionLogic(TransactionDbContext db, MockApi mockApi)
        {
            _db=db;
            _mockApi=mockApi;

        }
        // will be initiated from program.cs
        public async Task RunAsync()
        {
            // snap shot of data pulled from Json file
            var TransactionData = await _mockApi.GetTransactionsAsync();

            using var dbTransaction = await _db.Database.BeginTransactionAsync();
            // finding unique id's hashset
            var TIDs = TransactionData.Select(x => x.TransactionId).ToHashSet();

            foreach (var dto in TransactionData)
            {
                var existing = await _db.Transactions.FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId);

               // var last4numbers = dto.CardNumber[^4..] ?? null;

                if (existing == null)
                {
                    _db.Transactions.Add(new Transaction
                    {
                        TransactionId = dto.TransactionId,
                        CardNumberLast4 = dto.CardNumber[^4..],
                        LocationCode = dto.LocationCode,
                        ProductName = dto.ProductName,
                        Amount = dto.Amount,
                        TransactionTime = dto.Timestamp,
                        Status="Active"
                    });
                }
                else
                {
                    if(existing.Status == "Finalized")
                        continue;
                    TrackChanges(existing, dto, dto.CardNumber[^4..]);

                }

            }
            //logic for Revocation status

            var Recent = await _db.Transactions.Where(t => t.TransactionTime >= DateTime.UtcNow.AddHours(-24) && t.Status == "Active").ToListAsync();

            foreach (var transaction in Recent)
            {
                if (!TIDs.Contains(transaction.TransactionId))
                {
                    AddAuditLog(transaction.TransactionId,"Status",transaction.Status,"Revoked");
                    transaction.Status = "Revoked";
                }
            }
            // logic for finalization
            var ExpTrans = await _db.Transactions.Where(t => t.TransactionTime < DateTime.UtcNow.AddHours(-24) && t.Status != "Finalized").ToListAsync();

            foreach (var transaction in ExpTrans)
            {
                transaction.Status = "Finalized";
            }
            //saving the changes
            await _db.SaveChangesAsync();
            await dbTransaction.CommitAsync();


        }

        //method to track audit data
        public void AddAuditLog(int TransId, String FieldName, string _oldValue, string _newValue)
        {
            _db.Audits.Add(new Audit
            {
                 TransactionId = TransId,
                 Name = FieldName,
                 OldValue = _oldValue,
                 NewValue = _newValue,
                 ModifiedAt = DateTime.UtcNow
            });
        }

        public void TrackChanges(Transaction t, TransactionDTO dto, string Last4numbers)
        {
            // Tracking Amount change
            if (t.Amount != dto.Amount)
            {
                AddAuditLog(t.TransactionId, "Amount", t.Amount.ToString(), dto.Amount.ToString());
                t.Amount = dto.Amount;
            }

            // Tracking Product Name change
            if (t.ProductName != dto.ProductName)
            {
                AddAuditLog(t.TransactionId, "ProductName", t.ProductName, dto.ProductName);
                t.ProductName = dto.ProductName;
            }

            // Tracking Location Code change
            if (t.LocationCode != dto.LocationCode)
            {
                AddAuditLog(t.TransactionId, "LocationCode", t.LocationCode, dto.LocationCode);
                t.LocationCode = dto.LocationCode;
            }

            //Tracking chnage Last four numbers
            if (t.CardNumberLast4 != Last4numbers)
            {
                AddAuditLog(t.TransactionId, "CardNumberLast4", t.CardNumberLast4,Last4numbers);
                t.CardNumberLast4 = Last4numbers;
            }
        }
    }
}
