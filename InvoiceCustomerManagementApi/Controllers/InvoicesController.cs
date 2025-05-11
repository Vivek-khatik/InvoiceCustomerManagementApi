using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using DataAccessLayer.Service;
using DinkToPdf;
using InvoiceCustomerManagementApi.CommonFunctions;
using InvoiceCustomerManagementApi.CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvoiceCustomerManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceInterface invoiceService;
        private readonly ICustomerInterface customerService;
        public InvoicesController(IInvoiceInterface _invoiceService, ICustomerInterface _customerService)
        {
            invoiceService = _invoiceService;
            customerService = _customerService;
        }
        // POST api/<InvoiceController>
        [HttpPost]
        [Route("createInvoice")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> Post(Invoice invoice)
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (invoiceService.InvoiceExist(invoice))
                    {
                        objCommonJson.responseStatus = 0;
                        objCommonJson.message = "Same invoice exist!";
                        objCommonJson.result = null;
                        return BadRequest(objCommonJson);
                    }
                    foreach (var invoiceItem in invoice.InvoiceLines)
                    {
                        if (!invoiceService.ItemExist(invoiceItem.ItemCode))
                        {
                            objCommonJson.responseStatus = 0;
                            objCommonJson.message = "Item not exist!";
                            objCommonJson.result = null;
                            return BadRequest(objCommonJson);
                        }
                    }
                    await invoiceService.CreateAsync(invoice);
                    objCommonJson.responseStatus = 1;
                    objCommonJson.message = "Invoice added successfully!";
                    objCommonJson.result = invoice;
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Any())
                                           .ToDictionary(
                                                kvp => kvp.Key,
                                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                            );
                    objCommonJson.responseStatus = 0;
                    objCommonJson.message = "Validation failed. Please check the errors.";
                    objCommonJson.result = errors;
                }
            }
            catch (Exception ex)
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = ex.Message;
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    objCommonJson.message = ex.InnerException.Message;
                }
            }
            return Ok(objCommonJson);
        }

        [HttpGet]
        [Route("pdfExport/{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> PdfExport(string id)
        {
            var objCommonJson = new CommonJsonResponse();
            //get invoice data
            var invoiceObj = invoiceService.InvoiceById(id);
            if (invoiceObj != null && !string.IsNullOrEmpty(invoiceObj.Id))
            {
                string invoiceHtml = System.IO.File.ReadAllText("C:\\Users\\Khush\\source\\repos\\Trainee2024\\Vivek Khatik\\DOTNET\\InvoiceCustomerManagementApi\\InvoiceCustomerManagementApi\\InvoiceTemplate\\InvoiceDetail.html");

                invoiceHtml = invoiceObj.InvoiceDate.ToString() != null ? invoiceHtml.Replace("{invoiceDate}", invoiceObj.InvoiceDate.ToString("MM/dd/yyyy")) : invoiceHtml.Replace("{invoiceDate}","");
                invoiceHtml = invoiceObj.DueDate.ToString() != null ? invoiceHtml.Replace("{dueDate}", invoiceObj.DueDate.ToString("MM/dd/yyyy")) : invoiceHtml.Replace("{dueDate}", "");
                invoiceHtml = invoiceObj.Discount.ToString() != null ?  invoiceHtml.Replace("{discount}", invoiceObj.Discount.ToString()) : invoiceHtml.Replace("{discount}", "0%");
                invoiceHtml = invoiceObj.DiscountAmount.ToString() != null ?  invoiceHtml.Replace("{discountAmount}", invoiceObj.DiscountAmount.ToString()) : invoiceHtml.Replace("{discountAmount}", "0");
                invoiceHtml = invoiceObj.ShippingAddress.ToString() != null ? invoiceHtml.Replace("{shippingCharge}", invoiceObj.ShippingCharge.ToString()) : invoiceHtml.Replace("{shippingCharge}", "0");
                invoiceHtml = invoiceObj.TotalAmount.ToString() != null ? invoiceHtml.Replace("{totalAmount}", invoiceObj.TotalAmount.ToString()) : invoiceHtml.Replace("{totalAmount}", "0");

                var shippingAddress = new StringBuilder();
                shippingAddress.Append("<p>");
                shippingAddress.Append(invoiceObj.ShippingAddress.StreetAddress);
                shippingAddress.Append(", ");
                shippingAddress.Append(invoiceObj.ShippingAddress.City);
                shippingAddress.Append(", ");
                shippingAddress.Append("</p>");
                shippingAddress.Append("<p>");
                shippingAddress.Append(invoiceObj.ShippingAddress.State);
                shippingAddress.Append(", ");
                shippingAddress.Append(invoiceObj.ShippingAddress.PostalCode);
                shippingAddress.Append(", ");
                shippingAddress.Append("</p>");
                shippingAddress.Append("<p>");
                shippingAddress.Append(invoiceObj.ShippingAddress.Country);
                shippingAddress.Append("</p>");
                invoiceHtml = invoiceHtml.Replace("{shippingAddress}", shippingAddress.ToString());

                var billingAddress = new StringBuilder();
                billingAddress.Append("<p>");
                billingAddress.Append(invoiceObj.BillingAddress.StreetAddress);
                billingAddress.Append(", ");
                billingAddress.Append(invoiceObj.BillingAddress.City);
                billingAddress.Append(", ");
                billingAddress.Append("</p>");
                billingAddress.Append("<p>");
                billingAddress.Append(invoiceObj.BillingAddress.State);
                billingAddress.Append(", ");
                billingAddress.Append(invoiceObj.BillingAddress.PostalCode);
                shippingAddress.Append(", ");
                billingAddress.Append("</p>");
                billingAddress.Append("<p>");
                billingAddress.Append(invoiceObj.BillingAddress.Country);
                billingAddress.Append("</p>");
                invoiceHtml = invoiceHtml.Replace("{billingAddress}", billingAddress.ToString());

                var lineItemsTr = new StringBuilder();
                double subTotal = 0;
                foreach (var item in invoiceObj.InvoiceLines)
                {
                    lineItemsTr.Append("<tr>");
                    lineItemsTr.Append("<td class=\"text-center\" style=\"padding-left:20px\"> " + item.ItemCode + " </td>");
                    lineItemsTr.Append("<td class=\"text-center\" style=\"padding-left:55px\"> " + item.Description + " </td>");
                    lineItemsTr.Append("<td class=\"text-center\" style=\"padding-left:30px\"> " + item.UnitPrice + " </td>");
                    lineItemsTr.Append("<td class=\"text-center\" style=\"padding-left:20px\"> " + item.Quantity + " </td>");
                    lineItemsTr.Append("<td class=\"text-center\" style=\"padding-left:20px\"> " + item.LineTotal + " </td>");
                    lineItemsTr.Append("</tr>");
                    subTotal += item.LineTotal;
                }

                invoiceHtml = invoiceHtml.Replace("{subTotal}", subTotal.ToString());
                invoiceHtml = invoiceHtml.Replace("{lineItemsTr}", lineItemsTr.ToString());

                var converter = new SynchronizedConverter(new PdfTools());
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 },
                    //Out = @"Pdf\test.pdf",
                },
                        Objects = {
                    new ObjectSettings()
                    {
                        PagesCount=true,
                        HtmlContent = invoiceHtml,
                        WebSettings = {DefaultEncoding = "utf-8"},
                        HeaderSettings = {FontSize = 9,Right="Page[page]of[toPage]",Line=true,Spacing=2.812}
                        //Page = "https://om.satvasolutions.com/",
                    },
                }
                };
                byte[] pdfFile = converter.Convert(doc);
                return File(pdfFile, "application/octet-stream", "Invoice.pdf");
            }
            else
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "Invoice not exist!";
                objCommonJson.result = null;
                return BadRequest(objCommonJson);
            }
        }

        [HttpGet]
        [Route("csvExport")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> CsvExport()
        {
            var invoiceList = invoiceService.ListAsync();
            var objnew = invoiceList.SelectMany(objInvoice => objInvoice.InvoiceLines.Select(objLineItem => new CustomInvoiceViewModel
            {
                Id = objInvoice.Id, 
                CustomerId = objInvoice.CustomerId,
                ItemCode = objLineItem.ItemCode,
                Description = objLineItem.Description,
                UnitPrice = objLineItem.UnitPrice,
                Quantity = objLineItem.Quantity,
                LineTotal = objLineItem.LineTotal,
                Number = objInvoice.Number,
                Billing_StreetAddress = objInvoice.BillingAddress.StreetAddress,
                Billing_City = objInvoice.BillingAddress.City,
                Billing_State = objInvoice.BillingAddress.State,
                Billing_PostalCode = objInvoice.BillingAddress.PostalCode,
                Billing_Country = objInvoice.BillingAddress.Country,
                Shipping_StreetAddress = objInvoice.ShippingAddress.StreetAddress,
                Shipping_City = objInvoice.ShippingAddress.City,
                Shipping_State = objInvoice.ShippingAddress.State,
                Shipping_PostalCode = objInvoice.ShippingAddress.PostalCode,
                Shipping_Country = objInvoice.ShippingAddress.Country,
                InvoiceDate = objInvoice.InvoiceDate,
                DueDate = objInvoice.DueDate,
                Discount = objInvoice.Discount,
                DiscountAmount = objInvoice.DiscountAmount,
                ShippingCharge = objInvoice.ShippingCharge,
                TotalAmount = objInvoice.TotalAmount
            })).ToList();
            var csvInvoiceString = CommonFunction.ToCsv(objnew);
            byte[] bytes = Encoding.ASCII.GetBytes(csvInvoiceString);
            return File(bytes, "application/octet-stream", "Invoice2.csv");
        }
    }
}
