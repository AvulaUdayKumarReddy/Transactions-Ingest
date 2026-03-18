using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Transactions_Ingest.Models;

namespace Transactions_Ingest.Services
{
    public class MockApi
    {
       private readonly string _filepath;

        public MockApi(string filepath) {  _filepath = filepath; }

        public async Task<List<TransactionDTO>> GetTransactionsAsync()
        {
            var json = await File.ReadAllTextAsync(_filepath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<TransactionDTO>>(json,options) ?? new List<TransactionDTO>();

        }

    }
}
