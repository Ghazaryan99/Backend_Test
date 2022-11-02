
using System.ComponentModel.DataAnnotations;

namespace Backend_Test.Data.Models
{
    public class Images
    {
        [Key]
        public string id { get; set; }
        public string Image{ get; set; }
        public string Name{ get; set; }
        public string FileName{ get; set; }
        public bool Changed { get; set; }
    }
}
