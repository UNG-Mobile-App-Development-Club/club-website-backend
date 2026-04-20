using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CodeHawksApi.DTOs
{
    public class UpdateProfileDto
    {
        public string? Fullname {get; set;}
        public string? Bio {get; set;}
        public string? Linkedin {get; set;}
        public string? Github {get;set;}
    
        public IFormFile? ProfilePicture {get;set;}
    }
}