namespace InvoiceCustomerManagementApi.CommonModel
{
    public class CommonJsonResponse
    {
        public int responseStatus { get; set; }
        public string message { get; set; }
        public dynamic result { get; set; }
        public int page { get; set; }
        public int totalRecord { get; set; }
    }
}
