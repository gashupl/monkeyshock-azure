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
    public class CountriesController : ODataController
    {
        private readonly Model.AppContext _db;

        private readonly ILogger<CountriesController> _logger;

        public CountriesController(Model.AppContext dbContext, ILogger<CountriesController> logger)
        {
            _logger = logger;
            _db = dbContext;
        }

        //GET: https://localhost:7057/v1/countries
        [EnableQuery(PageSize = 15)]
        public IQueryable<Country> Get()
        {
            return _db.Countries;
        }

        //GET: https://localhost:7057/v1/countries/1
        [EnableQuery]
        public SingleResult<Country> Get([FromODataUri] int key)
        {
            var result = _db.Countries.Where(c => c.Id == key);
            return SingleResult.Create(result);
        }

        //POST: https://localhost:7057/v1/countries
        //Body:
        //{
        //  "Name": "Ukraine"
        //}
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return Created(country);
        }

        //PATH: https://localhost:7057/v1/countries/4
        //Body:
        //{
        //  "Name": "Ukraine"
        //}
        [EnableQuery]
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<Country> country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingCountry = await _db.Countries.FindAsync(key);
            if (existingCountry == null)
            {
                return NotFound();
            }

            country.Patch(existingCountry);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(existingCountry);
        }

        //DELETE: https://localhost:7057/v1/countries/1
        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            Country existingCountry = await _db.Countries.FindAsync(key);
            if (existingCountry == null)
            {
                return NotFound();
            }

            _db.Countries.Remove(existingCountry);
            await _db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        private bool CountryExists(int key)
        {
            return _db.Countries.Any(p => p.Id == key);
        }
    }
}