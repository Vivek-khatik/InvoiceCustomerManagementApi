using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Service
{
    public interface IInvoiceInterface
    {
        public Task CreateAsync(Invoice invoice);
        public bool InvoiceExist(Invoice invoice);
        public bool ItemExist(string itemCode);
        public Invoice InvoiceById(string id);
        public List<Invoice> ListAsync();
    }   
}
