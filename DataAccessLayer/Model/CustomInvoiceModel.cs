using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class CustomInvoiceViewModel
    {
        public string Id { get; set; }
        public string? Billing_StreetAddress { get; set; }  
        public string? Billing_City { get; set; }  
        public string? Billing_State { get; set; }  
        public string? Billing_PostalCode { get; set; }  
        public string? Billing_Country { get; set; }  
        public string? Shipping_StreetAddress { get; set; }
        public string? Shipping_City { get; set; }
        public string? Shipping_State { get; set; }
        public string? Shipping_PostalCode { get; set; }
        public string? Shipping_Country { get; set; }
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double LineTotal { get; set; }
        public string Number { get; set; }
        public string CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public double Discount { get; set; }
        public double DiscountAmount { get; set; }
        public double ShippingCharge { get; set; }
        public double TotalAmount { get; set; }
    }
}
