using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Model;

namespace DataAccessLayer.ViewModel
{
    public class ItemsViewModel:Item
    {
        [Range(1,Int32.MaxValue)]
        public int Quantity { get; set; }
        public double LineTotal {  get; set; }  
    }
}
