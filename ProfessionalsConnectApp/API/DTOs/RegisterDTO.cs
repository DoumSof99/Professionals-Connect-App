using System.ComponentModel.DataAnnotations;

namespace API.DTOs {

    [Serializable]
    public class RegisterDTO {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
