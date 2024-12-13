using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB_DotNetCore.data;
using MongoDB_DotNetCore.Entities.MongoDB;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MongoDB_DotNetCore.Controllers.MongoDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class L1_Pool_OpenedController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMongoCollection<L1_Pool_Opened> _L1_Pool_Opened;
        public L1_Pool_OpenedController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _L1_Pool_Opened = mongoDbContext.GetCollection<L1_Pool_Opened>("L1_Pool_Opened");
        }

        [HttpGet("All")]
        public async Task<IEnumerable<L1_Pool_Opened>> GetAllL1_Pool_Opened()
        {
            return await _L1_Pool_Opened.Find(FilterDefinition<L1_Pool_Opened>.Empty).ToListAsync();
        }

        [HttpGet("Filter")]
        public async Task<IEnumerable<L1_Pool_Opened>> GetL1_Pool_OpenedByFilter()
        {
            // Create a filter to find records with the specified L1Name
            var filter = Builders<L1_Pool_Opened>.Filter.Eq(x => x.L1Name, "HMC_OP205");
            // Use Find() with the filter to get records
            var records = await _L1_Pool_Opened
                .Find(filter)  // Filter to match L1Name
                .ToListAsync();  // Convert result to a list

            return records;
            //return await _L1_Pool_Opened.Find(FilterDefinition<L1_Pool_Opened>.Empty).ToListAsync();
        }

        //// GET api/<L1_Pool_OpenedController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<L1_Pool_OpenedController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<L1_Pool_OpenedController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<L1_Pool_OpenedController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
