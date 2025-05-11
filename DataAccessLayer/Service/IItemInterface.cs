using DataAccessLayer.CommonModel;
using DataAccessLayer.Model;
using DataAccessLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Service
{
    public interface IItemInterface
    {
        public Task<List<Item>> ListAsync();
        public Task CreateAsync(Item items);
        public List<CommonDropdownModel> getDropdown();
        public bool ItemExist(Item item);
    }
}
