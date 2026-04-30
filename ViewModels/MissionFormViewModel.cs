using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MMOngo.ViewModels
{
    public class MissionFormViewModel
    {
        public string OriginalMissionName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mission Name")]
        public string MissionName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mission Type")]
        public string MissionType { get; set; } = string.Empty;

        [Display(Name = "Quest Giver")]
        public string QuestGiver { get; set; } = string.Empty;

        public int Reward { get; set; }

        public List<SelectListItem> QuestGiverOptions { get; set; } = new List<SelectListItem>();
    }
}
