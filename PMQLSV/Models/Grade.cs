using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PMQLSV.Models
{
    public class Grades
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int MaxGrade { get; set; }
        public int Grade { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}
