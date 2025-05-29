using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterHelper.Models;

namespace ShelterHelper.API.Controllers
{
    [Route("api/owners")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly ShelterContext _context;

        public OwnersController(ShelterContext context)
        {
            _context = context;
        }

        // GET: api/owners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Owner>>> GetAllOwners()
        {
            return await _context.Owner.ToListAsync();
        }

        // GET api/owners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> GetOwnerById(int id)
        {
            var owner = await _context.Owner.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            return owner;
        }

        // POST api/owners
        [HttpPost]
        public async Task<ActionResult<Owner>> Post(Owner owner)
        {
            _context.Owner.Update(owner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOwnerById", new { id = owner.OwnerId }, owner);
        }

        // PUT api/owners/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/owners/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public void Delete(int id)
        {
        }
    }
}