using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MMOngo.ViewModels
{
    public class GuildFormViewModel
    {
        public string OriginalGuildName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Guild Name")]
        public string GuildName { get; set; } = string.Empty;

        [Required]
        public string Creator { get; set; } = string.Empty;

        [Display(Name = "Creation Date")]
        public string CreationDate { get; set; } = string.Empty;

        [Display(Name = "Benefits")]
        public string BenefitsText { get; set; } = string.Empty;

        [Display(Name = "Members")]
        public List<string> SelectedMembers { get; set; } = new List<string>();

        public List<SelectListItem> MemberOptions { get; set; } = new List<SelectListItem>();
    }
}
