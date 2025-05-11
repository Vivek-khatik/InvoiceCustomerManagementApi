using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.ViewModel;

namespace DataAccessLayer.Model
{
    public class Invoice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9-]*$", ErrorMessage = "Characters are not allowed.")]
        public string Number { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public DateTime InvoiceDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public Address BillingAddress { get; set; }
        [Required]
        public Address ShippingAddress { get; set; }
        [Required]
        public List<ItemsViewModel> InvoiceLines { get; set; }
        public double Discount { get; set; }
        public double DiscountAmount { get; set; }
        public double ShippingCharge { get; set; }
        public double TotalAmount {  get; set; }
    }
    public class Address
    {
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Country { get; set; }
    }

    
}
