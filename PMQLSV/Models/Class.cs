using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PMQLSV.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int ClassSize { get; set; }
        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
