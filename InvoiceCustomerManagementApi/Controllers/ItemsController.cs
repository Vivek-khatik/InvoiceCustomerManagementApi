using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using DataAccessLayer.Service;
using DataAccessLayer.ViewModel;
using InvoiceCustomerManagementApi.CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvoiceCustomerManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemInterface itemService;
        public ItemsController(IItemInterface _itemService)
        {
            itemService = _itemService;
        }
        // GET: api/<ItemsController>
        [HttpGet]
        [Route("getItem")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> Get()
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                var itemList = await itemService.ListAsync();

                objCommonJson.responseStatus = 1;
                if (itemList != null)
                {
                    objCommonJson.message = "Record found successfully";
                }
                else
                {
                    objCommonJson.responseStatus = 2;
                    objCommonJson.message = "No record found";
                }
                objCommonJson.result = itemList;
                objCommonJson.totalRecord = itemList.Count();
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
        [Route("getItemDropDown")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult> GetItemDropDown()
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                var customerList = itemService.getDropdown();

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

        // POST api/<ItemsController>
        [HttpPost]
        [Route("createItem")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Post(Item item)
        {
            var objCommonJson = new CommonJsonResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (itemService.ItemExist(item))
                    {
                        objCommonJson.responseStatus = 0;
                        objCommonJson.message = "Same item exist!";
                        objCommonJson.result = null;
                        return BadRequest(objCommonJson);
                    }
                    await itemService.CreateAsync(item);
                    objCommonJson.responseStatus = 1;
                    objCommonJson.message = "Item added successfully!";
                    objCommonJson.result = item;
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
    }
}
