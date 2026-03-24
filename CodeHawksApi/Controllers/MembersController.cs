using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeHawksApi.Models;

namespace CodeHawksApi.Controllers
{

//sets route to / api/members
[Route("api/[controller]")]
[ApiController]


public class MembersController : ControllerBase
{
private readonly ClubDataContext _context;

// Dependancy Injection, points to our DB using our config Israel Made
public MembersController(ClubDataContext context)
{
 _context = context;
}



// GET: api/Members
[HttpGet]
public async Task<IActionResult> GetMembers()
{
// members is a C# list, the await stuff gets all the members from the DB
var members = await _context.Members.ToListAsync();

// Returns Status (EX 200 OK STATUS) in JSON
return Ok(members);
}



}


}

