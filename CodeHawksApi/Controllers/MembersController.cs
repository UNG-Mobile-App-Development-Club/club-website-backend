using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeHawksApi.Models;
using CodeHawksApi.DTOs;
using System;
using System.Threading.Tasks;
using Resend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration; // Add this
using Microsoft.IdentityModel.Tokens;     // Add this
using System.IdentityModel.Tokens.Jwt;    // Add this
using System.Security.Claims;             // Add this
using System.Text;                        // Add this
using Microsoft.AspNetCore.Authorization;

namespace CodeHawksApi.Controllers
{

    //sets route to / api/members
    [Route("api/[controller]")]
    [ApiController]


    public class MembersController : ControllerBase
    {
        private readonly ClubDataContext _context;
        private readonly IResend _resend;
        // Dependancy Injection, points to our DB using our config Israel Made
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public MembersController(ClubDataContext context, IResend resend, IWebHostEnvironment env,IConfiguration config)
        {
            _context = context;
            _resend = resend;
            _env = env;
            _config = config;

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

        // POST: api/Members/signup

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromForm] SignupDto request)
        {
            // 1. Email Domain Validation
            if (!request.Email.EndsWith("@ung.edu", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("You must use a valid university email address.");
            }

            string? dbImagePath =null;

            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                // Find the wwwroot folder on the server
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "wwwroot", "uploads", "profiles");
                
                // Ensure the directory exists (creates it if it's the first time)
                Directory.CreateDirectory(uploadsFolder); 

                // Generate a unique file name (e.g., 550e8400-e29b.jpg)
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the VPS disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ProfilePicture.CopyToAsync(stream);
                }

                // Set the path that we will save to PostgreSQL
                dbImagePath = $"/uploads/profiles/{uniqueFileName}";
            }

            // 2. Check if user already exists
            // AnyAsync is faster than loading the whole user; it just returns true or false
            bool userExists = await _context.Members.AnyAsync(m => m.Email == request.Email || m.Username == request.Username);
            if (userExists)
            {
                return Conflict("A user with this email or username already exists.");
            }
            // 3. Generate a random 6-digit temp code
            Random random = new Random();
            string generatedCode = random.Next(100000, 999999).ToString();

            // 4. Hash the password using BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 5. Map the incoming DTO to your actual Database Model
            var newMember = new Member
            {
                Username = request.Username,
                Fullname = request.Fullname,
                Email = request.Email,
                Bio = request.Bio,
                Linkedin = request.Linkedin,
                Github = request.Github,
                Passwordhash = passwordHash,
                is_verified = false,       // Locks down the account initially
                Tempcode = generatedCode,  // Saves the code to compare later
                Profilepicurl = dbImagePath
            };

            //Resend Step

            var message = new EmailMessage
            {
                From = "noreply@codehawks.org", 
                To = { request.Email },
                Subject = "Verify your CodeHawks Account",
                HtmlBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Welcome to the App Dev Club!</h2>
                        <p>Hi {request.Fullname},</p>
                        <p>Thanks for signing up. Please use the 6-digit code below to verify your UNG student email and activate your account.</p>
                        <h1 style='color: #00529b; letter-spacing: 5px;'>{generatedCode}</h1>
                        <p>If you didn't request this, you can safely ignore this email.</p>
                    </div>"
            };

            await _resend.EmailSendAsync(message);

            // 6. Save the new member to PostgreSQL
            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            // 7. Output the result (INCLUDING the code for testing purposes)
            return Ok(new 
            { 
                Message = "Signup successful. Awaiting verification.",
                TestingCode = generatedCode // NOTE: We will delete this line once Resend is plugged in!
            });
        }


        // POST: api/Members/verify
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyDto request)
        {
            // 1. Find the user by their email
            // We use FirstOrDefaultAsync because we actually need to modify the user row this time
            var user = await _context.Members.FirstOrDefaultAsync(m => m.Email == request.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // 2. Check if they are already verified
            if (user.is_verified)
            {
                return BadRequest("This account is already verified.");
            }

            // 3. Compares code
            if(user.Tempcode != request.Code)
            {
                return BadRequest("Invalid Verification Code.");
            }

            // 4. Sucess statements

            user.is_verified = true;
            user.Tempcode = null;

            // makes actual changes to DB

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Account successfully verified! You can now log in." });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _context.Members.FirstOrDefaultAsync(m => m.Email == request.Email);

            if(user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if(!user.is_verified)
            {
                return Unauthorized("Please verify your account with the code sent to your email before loggin in.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            string token = CreateToken(user);

            return Ok(new 
            { 
                Message = "Login successful.",
                Token = token 
            });


        }
        private string CreateToken(Member user)
        {
            // 1. Create Claims (Information embedded directly inside the token)
            // This is useful because your frontend can decode the token to say "Hello, {Username}!" 
            // without making another database call.
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 2. Create the Security Key using the secret from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("Jwt:Token").Value!));

            // 3. Create the Signing Credentials (uses the highly secure HMAC SHA512 algorithm)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Assemble the Token Structure
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), // Token expires in 7 days (requires user to log in again)
                SigningCredentials = creds
            };

            // 5. Generate the actual string token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


    }


}

