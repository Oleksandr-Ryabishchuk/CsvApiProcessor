using System.Linq;
using ExcelWriteApi.Data;
using ExcelWriteApi.Interfaces;
using ExcelWriteApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExcelWriteApi.Controllers
{
    [Authorize]
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IDataProcessor _dataProcessor;
        
        public TransactionsController(DataContext dataContext,
                                      IDataProcessor dataProcessor)
        {
            _dataContext = dataContext;
            _dataProcessor = dataProcessor;
        }
       
        [HttpGet] // Shows all the transactions of some csv from specific data path  
        [Route("GetAllTransactions")]
        public IActionResult GetAllTransactions(string @inputpath)
        {
            var transactions = CsvProcessor.ReadProcess(@inputpath);

            if(transactions.Count == 0)
            {
                BadRequest("There are no available transactions");
            }

            return Ok(transactions);
        }

        [HttpGet]
        [Route("GetSingleTransactionById")]
        public IActionResult GetSingleTransactionById(int id)
        {
            var transaction = _dataContext.Transactions.Find(id);
            if (transaction != null)
                return Ok(transaction);
            return BadRequest($"Transaction with id {id} doesn't exist");
        }

        [HttpPost] // Method imports data from csv file by path of rewrite all the records in data base
        [Route("WriteOrRewriteToDataBase")]
        public IActionResult WriteOrRewriteToDataBase(string @inputpath)
        {
            var transactionsToPass = CsvProcessor.ReadProcess(@inputpath);
            var transactions = _dataContext.Transactions;
            if(transactions == null)
            {
                transactions.AddRange(transactionsToPass);
                _dataContext.SaveChanges();
                var whatToReturn = _dataContext.Transactions;
                return Ok(whatToReturn);
            }
            transactions.UpdateRange(transactionsToPass);
            var changes = _dataContext.SaveChanges();
            if(changes > 0)
                return Ok(transactions);
            return BadRequest("There are no updates in file");

        }
        
        [HttpPost] // Exports filtered list of records from data base to csv file
        [Route("Export")]        
        public IActionResult Export(string status, string type, string @pathOutput)
        {
            
            var filteredList = _dataProcessor.FilterTransactions(status, type);
           
            if(filteredList != null)
            {
                CsvProcessor.WriteProcess(@pathOutput, filteredList);
                return Ok(filteredList);
            }

            return BadRequest("You can't export from empty datatable");
        }

        [HttpGet] // Returns record or list of records that contain specific name of client
        [Route("GetTransactionByClientName")]
        public IActionResult GetTransactionByName(string name)
        {
            var query = _dataContext.Transactions.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(d => d.ClientName.Contains(name));
            }
            var result = query.ToList();
            return Ok(result);
        }

        [HttpPut] // Updates record in data base by single field
        [Route("UpdateSingleTransaction")]
        public IActionResult UpdateTransaction(int id, string status)
        {
            var transactionToUpdate = _dataContext.Transactions.Find(id);
            if (transactionToUpdate == null)
            {
                return BadRequest($"Transaction with id: {id} doesn't exists");
            }
            if (transactionToUpdate.TransactionId == id)
            {                 
                transactionToUpdate.Status = status;
                
                var changed = _dataContext.SaveChanges();
                if(changed > 0)
                {
                    return Ok($"{changed} transaction has been changed");
                }                
            }
            return BadRequest($"Transaction with id: {id} doesn't exists");
        }

        [HttpPut] // Updates list of records by id and imports record that not exist in data base
        [Route("UpdateListOfTransactions")] // But not replaces or removes them  
        public IActionResult UpdateListOfTransactions(string @inputpath)
        {            
            var datainput = CsvProcessor.ReadProcess(@inputpath);

            _dataProcessor.MergeTables(datainput);
           
            return Ok(_dataContext.Transactions.ToList());            
        }

        [HttpDelete]
        [Route("DeleteTransactionById")]
        public IActionResult DeleteTransaction(int id)
        {
            var transactionToDelete = _dataContext.Transactions.Find(id);

            var result = _dataContext.Transactions.Remove(transactionToDelete);

            _dataContext.SaveChanges();

            if(result != null)
            {
                return Ok($"Transaction with id: {id}, has been successfully deleted");
            }
            return BadRequest("You can't delete this transaction");           
        }
    }
}
