using DataAccessLayer.CommonModel;
using DataAccessLayer.Model;
using DataAccessLayer.Service;
using DataAccessLayer.ViewModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class ItemService : IItemInterface
    {
        public IMongoCollection<Item> itemCollection;
        public ItemService(string connectionString, string databaseName)
        {
            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);
            itemCollection = mongoDatabase.GetCollection<Item>("Items");
        }
        public async Task CreateAsync(Item items)
        {
            await itemCollection.InsertOneAsync(items);
        }

        public List<CommonDropdownModel> getDropdown()
        {
            List<Item> itemList = itemCollection.Find(_ => true).ToListAsync().Result;

            List<CommonDropdownModel> commonDropdownList = new List<CommonDropdownModel>();
            CommonDropdownModel commonDropdownModel = new CommonDropdownModel();

            foreach (var invoice in itemList)
            {
                commonDropdownModel = new CommonDropdownModel
                {
                    OptionValue = invoice.ItemCode,
                    OptionText = invoice.Description
                };
                commonDropdownList.Add(commonDropdownModel);
            }
            return commonDropdownList;
        }

        public bool ItemExist(Item item)
        {
            var itemCode = itemCollection.Find(p => p.ItemCode == item.ItemCode).FirstOrDefault();
            if (itemCode is null) return false; else return true;
        }

        public Task<List<Item>> ListAsync()
        {
            return itemCollection.Find(_ => true).ToListAsync();
        }
    }
}
