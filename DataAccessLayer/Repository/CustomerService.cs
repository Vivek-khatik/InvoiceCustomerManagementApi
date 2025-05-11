using DataAccessLayer.CommonModel;
using DataAccessLayer.Model;
using DataAccessLayer.Service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class CustomerService : ICustomerInterface
    {
        public IMongoCollection<Customer> customerCollection;
        public IMongoCollection<FileWithData> fileWithDataCollection;
        public CustomerService(string connectionString,string databaseName) {
            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);
            customerCollection = mongoDatabase.GetCollection<Customer>("Customers");
            fileWithDataCollection = mongoDatabase.GetCollection<FileWithData>("FileWithData");
        }
        public async Task CreateAsync(Customer customers)
        {
            await customerCollection.InsertOneAsync(customers);
        }
        public Customer GetById(string id)
        {
            var customerData = customerCollection.Find(p => p.Id == id).FirstOrDefault();
            return customerData;
        }

        public List<CommonDropdownModel> getDropdown()
        {
            List<Customer> customerList = customerCollection.Find(_ => true).ToListAsync().Result;

            List<CommonDropdownModel> commonDropdownList = new List<CommonDropdownModel>();
            CommonDropdownModel commonDropdownModel = new CommonDropdownModel();

            foreach (var customer in customerList)
            {
                commonDropdownModel = new CommonDropdownModel
                {
                    OptionValue = customer.Id,
                    OptionText = customer.FullName
                };
                commonDropdownList.Add(commonDropdownModel);
            }
            return commonDropdownList;
        }

        public Task<List<Customer>> ListAsync()
        {
            return customerCollection.Find(_=>true).ToListAsync();
        }
        public async void UpdateCustomer(string id,Customer customer)
        {
            var filter = Builders<Customer>.Filter.Eq(r => r.Id, id);
            var oldCustomer = customerCollection.Find(filter).First();
            var oldId = oldCustomer.Id;
            Customer newCustomer = new()
            {
                Id = oldId,
                FullName = customer.FullName,
                Address = customer.Address,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                FileName = customer.FileName
            };
            await customerCollection.ReplaceOneAsync(filter, newCustomer);
        }

        public async void UploadFileWithData(FileWithData model)
        {
            await fileWithDataCollection.InsertOneAsync(model);
        }
    }
}
