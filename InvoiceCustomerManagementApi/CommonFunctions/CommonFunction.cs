using DataAccessLayer.Model;
using System.Net;
using System.Reflection;
using System.Text;

namespace InvoiceCustomerManagementApi.CommonFunctions
{
    public static class CommonFunction
    {
        //public static void WriteToCsv<T>(IEnumerable<T> data, string filePath)
        //{
        //    if (data == null || !data.Any())
        //    {
        //        throw new ArgumentException("Data cannot be null or empty.");
        //    }

        //    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    using (var writer = new StreamWriter(filePath))
        //    {
        //        // Write header row

        //        string headerName = "Id,Number,CustomerId,InvoiceDate,DueDate,BillingStreetAddress,BillingCity,BillingState,BillingPincode,BillingCountry,ShippingStreetAddress,ShippingCity,ShippingState,ShippingPincode,ShippingCountry,InvoiceLines,Discount,DiscountAmount,ShippingCharge,TotalAmount";

        //        writer.WriteLine(headerName);
        //        //writer.WriteLine(string.Join(",", properties.Select(p => p.Name)));
        //        //writer.WriteLine(string.Join(",", properties.Select(p => FormatHeader(p.Name))));

        //        // Write data rows
        //        foreach (var item in data)
        //        {
        //            var values = properties.Select(p => FormatValue(p.GetValue(item)));
        //            writer.WriteLine(string.Join(",", values));
        //        }
        //    }
        //}

        //private static string FormatValue(object value)
        //{
        //    if (value == null)
        //    {
        //        return "";
        //    }
        //    else if (value is string)
        //    {
        //        return "\"" + value.ToString() + "\"";
        //    }
        //    else if (value is IEnumerable<string>)
        //    {
        //        return "\"" + string.Join(",", (IEnumerable<string>)value) + "\"";
        //    }
        //    else if (value is IEnumerable<object>)
        //    {
        //        return "\"" + string.Join(",", (IEnumerable<object>)value) + "\"";
        //    }
        //    else if (value.GetType().IsClass && value.GetType() != typeof(string))
        //    {
        //        // Recursively format complex objects
        //        var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //        var propertyValues = properties.Select(p => FormatValue(p.GetValue(value)));

        //        return "\"" + string.Join(",", propertyValues) + "\"";
        //    }
        //    else
        //    {
        //        return value.ToString();
        //    }
        //}

        public static string ToCsv<T>(this List<T> items, string delimiter = ",")
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name);

            //if (props.Select(p=>p.Name).Equals("ShippingAddress"))
            //{
               
            //}

            var csv = new StringBuilder();

            // Write Headers
            csv.AppendLine(string.Join(delimiter, props.Select(p => p.Name)));

            // Write Rows
            foreach (var item in items)
            {
                // Write Fields
                csv.AppendLine(string.Join(delimiter, props.Select(p => GetCsvFieldBasedOnValue(p, item))));
            }

            return csv.ToString();
        }
        // Provide generic and specific handling of fields
        private static object GetCsvFieldBasedOnValue<T>(PropertyInfo p, T item)
        {
            string value = "";
            try
            {
                value = p.GetValue(item, null)?.ToString();
                if (value == null) return "NULL";  // Deal with nulls
                if (value.Trim().Length == 0) return ""; // Deal with spaces and blanks
                // Guard strings with "s, they may contain the delimiter!
                if (p.PropertyType == typeof(string))
                {
                    value = string.Format("\"{0}\"", value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }
    }
}



