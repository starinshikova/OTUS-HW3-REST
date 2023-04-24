using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Models;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ApplicationDbContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // получение данных всех пользователей
        //GET: api/customer
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _context.Customers.ToArray();
        }

        // получение данных пользователя по id
        //GET: api/customer/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var customer = _context.Customers.SingleOrDefault(p => p.Id == id);

            if (customer != null)
            {
                return Ok(customer);
            }

            return NotFound();
        }

        // создание нового пользователя
        // POST api/customer
        [HttpPost]
        public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
        {
            var currentCustomer = await _context.Customers.FindAsync(customer.Id);

            if (currentCustomer != null)
            {
                return Conflict();
            }

            this._context.Customers.Add(customer);

            await this._context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = customer.Id }, customer);
        }

        //изменение имеющего пользователя
        // PUT: api/customer/5
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromBody] Customer customer)
        {
            var currentCustomer = await _context.Customers.FindAsync(customer.Id);
            if (currentCustomer == null)
            {
                return NotFound();
            }
            currentCustomer.Firstname = customer.Firstname;
            currentCustomer.Lastname = customer.Lastname;

            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        // удаление пользователя по id
        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var currentCustomer = await _context.Customers.FindAsync(id);

            if (currentCustomer == null)
            {
                return NotFound();
            }

            _context.Remove(currentCustomer);

            await _context.SaveChangesAsync();

            return Ok();

        }
    }
}