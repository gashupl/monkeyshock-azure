using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using MonkeyShock.Azure.WebApi.Model;

namespace MonkeyShock.Azure.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitiesController : ODataController
    {
        private readonly Model.AppContext _db;

        private readonly ILogger<CitiesController> _logger;

        public CitiesController(Model.AppContext dbContext, ILogger<CitiesController> logger)
        {
            _logger = logger;
            _db = dbContext;
        }

        //GET: https://localhost:7057/v1/cities
        [EnableQuery(PageSize = 15)]
        public IQueryable<City> Get()
        {
            return _db.Cities;
        }

        //GET: https://localhost:7057/v1/cities/4bdd38e1-84df-4a4e-9eb6-d0ea2cde7c00
        [EnableQuery]
        public SingleResult<City> Get([FromODataUri] Guid key)
        {
            var result = _db.Cities.Where(c => c.Id == key);
            return SingleResult.Create(result);
        }

        //POST: https://localhost:7057/v1/cities
        //Body:
        //{
        //  "Name": "New York"
        //}
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] City city)
        {
            city.Id = Guid.NewGuid(); 
            _db.Cities.Add(city);
            await _db.SaveChangesAsync();
            return Created(city);
        }

        //PATH: https://localhost:7057/v1/cities/4bdd38e1-84df-4a4e-9eb6-d0ea2cde7c00
        //Body:
        //{
        //  "Name": "New York"
        //}
        [EnableQuery]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<City> city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingCity = await _db.Cities.FindAsync(key);
            if (existingCity == null)
            {
                return NotFound();
            }

            city.Patch(existingCity);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(existingCity);
        }

        //DELETE: https://localhost:7057/v1/cities/4bdd38e1-84df-4a4e-9eb6-d0ea2cde7c00
        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            City existingCity= await _db.Cities.FindAsync(key);
            if (existingCity == null)
            {
                return NotFound();
            }

            _db.Cities.Remove(existingCity);
            await _db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        private bool CityExists(Guid key)
        {
            return _db.Cities.Any(p => p.Id == key);
        }
    }
}