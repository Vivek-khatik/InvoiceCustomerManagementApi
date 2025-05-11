using DataAccessLayer.CommonModel;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Service
{
    public interface ICustomerInterface
    {
        public Task<List<Customer>> ListAsync();
        public Task CreateAsync(Customer customers);
        public List<CommonDropdownModel> getDropdown();
        public Customer GetById(string id);
        public void UpdateCustomer(string id, Customer customer);
        public void UploadFileWithData(FileWithData model);
    }
}
