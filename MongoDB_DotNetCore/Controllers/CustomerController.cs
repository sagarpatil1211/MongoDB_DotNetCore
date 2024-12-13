using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities;

namespace MongoDB_DotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMongoCollection<Customer> _customers;
        public CustomerController(MongoDbService mongoDbService)
        {
            _customers = mongoDbService.Database.GetCollection<Customer>("customer");
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
          return await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();  
        }

        
        [HttpPost]
        public async Task<ActionResult> create (Customer customer)
        {
            await _customers.InsertOneAsync(customer);
            return Ok();
        }
    }
}
