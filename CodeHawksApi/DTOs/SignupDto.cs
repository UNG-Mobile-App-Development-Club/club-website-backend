using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace CodeHawksApi.DTOs 
{
    public class SignupDto
    {
        [Required]
        public string Username { get; set;} = null!;

        [Required]
        public string Fullname {get; set;} = null!;

        [Required]
        [EmailAddress]
        public string Email {get; set;} = null!;

        [Required]
        public string Password {get; set;} = null!;

        public string? Bio {get; set;} 

        public string? Github {get; set;}

        public string? Linkedin {get; set;}

        public IFormFile? ProfilePicture{get; set;}

    }
}