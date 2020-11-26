using ExcelWriteApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelWriteApi.Interfaces
{
    public interface IDataProcessor
    {
        List<Transaction> FilterTransactions(string status, string type);
        void MergeTables(List<Transaction> datainput);
    }
}
