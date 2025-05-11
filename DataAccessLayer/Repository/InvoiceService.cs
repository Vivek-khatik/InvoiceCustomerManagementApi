using DataAccessLayer.Model;
using DataAccessLayer.Service;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class InvoiceService : IInvoiceInterface
    {
        public IMongoCollection<Invoice> invoiceCollection;
        public IMongoCollection<Item> itemCollection;
        public InvoiceService(string connectionString, string databaseName)
        {
            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);
            invoiceCollection = mongoDatabase.GetCollection<Invoice>("Invoices");
            itemCollection = mongoDatabase.GetCollection<Item>("Items");
        }

        private async Task<List<Item>> GetItemListAsync()
        {
            return await itemCollection.Find(_ => true).ToListAsync();
        }
        public bool InvoiceExist(Invoice invoice)
        {
            var invoiceCode = invoiceCollection.Find(p => p.Number == invoice.Number).FirstOrDefault();
            if (invoiceCode is null) return false; else return true;
        }
        public bool ItemExist(string itemCode)
        {
            var getItemCode = itemCollection.Find(p => p.ItemCode == itemCode).FirstOrDefault();
            if (getItemCode is null) return false; else return true;
        }

        public async Task CreateAsync(Invoice invoice)
        {
            List<Item> itemListObj = await GetItemListAsync();

            for(int counter = 0; counter < invoice.InvoiceLines.Count(); counter++)
            {
                var foundItem = itemListObj.FirstOrDefault(i => i.ItemCode == invoice.InvoiceLines[counter].ItemCode);
                
                invoice.InvoiceLines[counter].Id = foundItem.Id; 
                invoice.InvoiceLines[counter].Description = foundItem.Description;
                invoice.InvoiceLines[counter].UnitPrice = foundItem.UnitPrice;
            }

            invoice.InvoiceLines.ForEach(p => p.LineTotal = p.Quantity * p.UnitPrice);
            double totalWithoutDiscount = invoice.InvoiceLines.Sum(p=>p.LineTotal);
            double discountAmount = (totalWithoutDiscount * invoice.Discount) / 100;
            invoice.DiscountAmount = discountAmount;
            invoice.TotalAmount = (totalWithoutDiscount - discountAmount) + invoice.ShippingCharge; 

            await invoiceCollection.InsertOneAsync(invoice);
        }

        public Invoice InvoiceById(string id)
        {
            var invoiceData = invoiceCollection.Find(p => p.Id == id).FirstOrDefault();
            return invoiceData;
        }

        public List<Invoice> ListAsync()
        {
            return invoiceCollection.Find(_ => true).ToList();
        }
    }
}
