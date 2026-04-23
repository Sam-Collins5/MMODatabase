using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class Player
    {
        [Required]
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public List<string> Characters { get; set; } = new List<string>();

        [Required]
        public string MemberSince { get; set; } = string.Empty;
    }
}