using System.ComponentModel.DataAnnotations;

namespace MMOngo.Models
{
    public class ToolItem
    {
        public int ToolId { get; set; }

        [Required]
        public string ToolName { get; set; } = string.Empty;

        public int Weight { get; set; }
        public int Price { get; set; }

        public List<string> Usage { get; set; } = new List<string>();
    }
}