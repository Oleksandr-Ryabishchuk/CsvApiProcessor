using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelWriteApi.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }        
        public string Status { get; set; }        
        public string Type { get; set; }
        public string ClientName { get; set; }
        public decimal Amount { get; set; }
    }
}
