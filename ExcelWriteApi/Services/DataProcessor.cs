using ExcelWriteApi.Data;
using ExcelWriteApi.Interfaces;
using ExcelWriteApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ExcelWriteApi.Services
{
    public class DataProcessor: IDataProcessor
    {
        // Main point of DataProcessor is a separating excessive logic 
        // and sql commands from Controller. 
        private readonly DataContext _dataContext;
        public DataProcessor(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public List<Transaction> FilterTransactions(string status, string type)
        {
            var transactions = _dataContext.Transactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                !string.IsNullOrWhiteSpace(type))
            {
                transactions = from transaction in transactions
                               where transaction.Status == status && transaction.Type == type
                               select transaction;
            }

            if (string.IsNullOrWhiteSpace(status) &&
                !string.IsNullOrWhiteSpace(type))
            {
                transactions = from transaction in transactions
                               where transaction.Type == type
                               select transaction;
            }

            if (!string.IsNullOrWhiteSpace(status) &&
               string.IsNullOrWhiteSpace(type))
            {
                transactions = from transaction in transactions
                               where transaction.Status == status
                               select transaction;
            }

            if (string.IsNullOrWhiteSpace(status) &&
                string.IsNullOrWhiteSpace(type))
            {
                transactions = from transaction in transactions
                               select transaction;
            }
            return transactions.ToList();
        }
        public void MergeTables(List<Transaction> datainput)
        {  
            foreach (var input in datainput)
            {
                var command = @"MERGE INTO Transactions
                USING 
                (
                   SELECT   @id as TransactionId,
                            @status AS Status,
                            @type AS Type,
                            @clientName AS ClientName,
                            @amount AS Amount
                ) AS entity
                ON  Transactions.TransactionId = entity.TransactionId
                WHEN MATCHED THEN
                    UPDATE 
                    SET     TransactionId = @id,
                            Status = @status,
                            Type = @type,
                            ClientName = @clientName,
                            Amount = @amount
                        WHEN NOT MATCHED THEN
                    INSERT (TransactionId, Status, Type, ClientName, Amount)
                    VALUES (@id, @status, @type, @clientName, @amount);";

                object[] parameters = {
                  new SqlParameter("@id", input.TransactionId),
                  new SqlParameter("@status",input.Status),
                  new SqlParameter("@type", input.Type),
                  new SqlParameter("@clientName", input.ClientName),
                  new SqlParameter("@amount", input.Amount)
                    };
                _dataContext.Database.ExecuteSqlRaw(command, parameters);
            }
        }
    }
}
