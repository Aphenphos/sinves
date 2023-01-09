using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sinves.Models;
using sinves.Services;

namespace sinves.Controllers


{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessController : Controller
    {
        private readonly BusinessService _businessService;

        public BusinessController(BusinessService businessService) =>
            _businessService = businessService;

        [AllowAnonymous]
        [HttpGet("getAll")]
        public async Task<List<Business>> Get() =>
            await _businessService.GetAsync();
        [AllowAnonymous]
        [HttpGet("get/{id:length(24)}")]
        public async Task<ActionResult<Business>> Get(string id)
        {
            var business = await _businessService.GetAsync(id);

            if (business is null)
            {
                return NotFound();
            }

            return business;
        }
        //Create a getbysearch(category and name at same time)
        [HttpPost("post/")]
        public async Task<IActionResult> Post(Business newBusiness)
        {
            await _businessService.CreateAsync(newBusiness);

            return CreatedAtAction(nameof(Get), new { id = newBusiness.Id }, newBusiness);
        }

        [HttpPut("update/{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Business updatedBusiness)
        {
            var business = await _businessService.GetAsync(id);

            if (business is null)
            {
                return NotFound();
            }

            updatedBusiness.Id = business.Id;

            await _businessService.UpdateAsync(id, updatedBusiness);

            return NoContent();
        }

        [HttpDelete("delete/{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var business = await _businessService.GetAsync(id);

            if (business is null)
            {
                return NotFound();
            }

            await _businessService.RemoveAsync(id);

            return NoContent();
        }
    }
}
