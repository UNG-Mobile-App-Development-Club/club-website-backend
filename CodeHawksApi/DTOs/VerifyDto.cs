using System.ComponentModel.DataAnnotations;

namespace CodeHawksApi.DTOs
{

public class VerifyDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set;} = null!;

        [Required]
        public string Code {get; set;} = null!;


    }


}