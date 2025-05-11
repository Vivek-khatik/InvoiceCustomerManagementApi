using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using DataAccessLayer.Service;
using DataAccessLayer.ViewModel;
using InvoiceCustomerManagementApi.CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvoiceCustomerManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerInterface customerService;
        private readonly IFileUploadInterface fileUploadInterface;
        public CustomersController(ICustomerInterface _customerService, IFileUploadInterface _fileUploadInterface)
        {
            customerService = _customerService;
            fileUploadInterface = _fileUploadInterface;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        [Route("getCustomer")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> Get()
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                var customerList = await customerService.ListAsync();

                objCommonJson.responseStatus = 1;
                if (customerList != null)
                {
                    objCommonJson.message = "Record found successfully";
                }
                else
                {
                    objCommonJson.responseStatus = 2;
                    objCommonJson.message = "No record found";
                }
                objCommonJson.result = customerList;
                objCommonJson.totalRecord = customerList.Count();
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
        [Route("getCustomerDropDown")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> GetCustomerDropDown()
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                var customerList = customerService.getDropdown();

                objCommonJson.responseStatus = 1;
                if (customerList != null)
                {
                    objCommonJson.message = "Record found successfully";
                }
                else
                {
                    objCommonJson.responseStatus = 2;
                    objCommonJson.message = "No record found";
                }
                objCommonJson.result = customerList;
                objCommonJson.totalRecord = customerList.Count();
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

        [HttpPost]
        [Route("createCustomer")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Post(Customer customers)
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    await customerService.CreateAsync(customers);
                    objCommonJson.responseStatus = 1;
                    objCommonJson.message = "Customer added successfully!";
                    objCommonJson.result = customers;
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

        // POST api/Customer/{id}/upload
        [HttpPost]
        [Route("{id}/upload")]
        public async Task<IActionResult> Upload(string id)
        {
            var objCommonJson = new CommonJsonResponse();
            
            //get customer data
            var customerObj = customerService.GetById(id);
            if(customerObj is null)
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "Invalid customer id";
                return BadRequest(objCommonJson);
            }

            var file = Request.Form.Files[0];
            if (file == null || file.Length == 0)
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not selected";
                return BadRequest(objCommonJson);
            }

            //upload file
            var filePath = fileUploadInterface.CreateFile(file);

            if (System.IO.File.Exists(filePath.Result))
            {
                //assign file name
                customerObj.FileName = filePath.Result;
                //update customer
                customerService.UpdateCustomer(id,customerObj);

                objCommonJson.responseStatus = 1;
                objCommonJson.message = "File uploaded successfully";
                return Ok(objCommonJson);
            }
            else
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not uploaded";
                return Ok(objCommonJson);
            }
        }

        // POST api/Customer/{id}/upload
        [HttpPost]
        [Route("{id}/uploadWithData")]
        public async Task<IActionResult> UploadWithData(string id, [FromForm] FileUploadDto model)
        {
            var objCommonJson = new CommonJsonResponse();
            FileWithData fileWithData = new FileWithData();
            //get customer data
            var customerObj = customerService.GetById(id);

            var file = model.File;
            if (file == null || file.Length == 0)
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not selected";
                return BadRequest(objCommonJson);
            }

            //upload file
            var filePath = fileUploadInterface.CreateFile(file);

            if (System.IO.File.Exists(filePath.Result))
            {
                //assign file name
                fileWithData.CustomerId = id;
                fileWithData.FileName = filePath.Result;
                fileWithData.AdditionalData = model.AdditionalData;
                //insert into file with data collection
                customerService.UploadFileWithData(fileWithData);

                objCommonJson.responseStatus = 1;
                objCommonJson.message = "File uploaded successfully";
                return Ok(objCommonJson);
            }
            else
            {
                objCommonJson.responseStatus = 0;
                objCommonJson.message = "File not uploaded";
                return Ok(objCommonJson);
            }
        }
    }
}
