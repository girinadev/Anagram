using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Anagram.Web.Models
{
    public class AnagramModel
    {
        [Required]
        [MaxLength(50)]
        public string Value { get; set; }

        [Required]
        [EnumDataType(typeof(Language))]
        public Language Language { get; set; }
    }

    public enum Language
    {
        RU = 1,
        EN = 2
    }
}
