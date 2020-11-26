using CsvHelper;
using ExcelWriteApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWriteApi.Services
{
    public static class CsvProcessor
    {
        public static List<Transaction> ReadProcess(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1)
                .Where(r => r.Length > 0)
                .Select(ParseRow).ToList();
        }

        public static void WriteProcess(string path, IEnumerable<Transaction> transactions)
        {
           using(var writer = new StreamWriter(path))
           using(var csv = new CsvWriter(writer, CultureInfo.GetCultureInfo("en-US")))
            {
                csv.WriteHeader<Transaction>();
                csv.NextRecord();
                foreach(var transaction in transactions)
                {
                    var x = new 
                    {
                        transaction.TransactionId,
                        transaction.Status,
                        transaction.Type,
                        transaction.ClientName,
                        V = transaction.Amount.ToString("C", CultureInfo.GetCultureInfo("en-US"))
                    };              
                    csv.WriteRecord(x);
                    csv.NextRecord();
                }
            }
        }

        private static Transaction ParseRow(string row)
        {
            var columns = row.Split(",");
            return  new Transaction()
            {
                TransactionId = Convert.ToInt32(columns[0]),
                Status = columns[1],
                Type = columns[2],
                ClientName = columns[3],
                Amount = Decimal.Parse(columns[4], NumberStyles.Currency)
            };
        }
    }
}
