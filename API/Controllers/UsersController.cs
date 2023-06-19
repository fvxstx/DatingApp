using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    // Using the Token to authorize your access
    [Authorize]
    public class UsersController : BaseApiController
    {       
        private readonly DataContext _context;

        public UsersController(DataContext context){
            _context = context;
        }

        // Allow that especific function to be accessed by anonymous
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            var users = await _context.Users!.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id){
            var user = await _context.Users!.FindAsync(id)!; 
            return user!;
        }
    }
}