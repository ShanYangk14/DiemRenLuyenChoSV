using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PMQLSV.Models
{
    public class Grades
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Cau1 { get; set; }
        [Required]
        public int Cau2 { get; set; }
        [Required]
        public int Cau3 { get; set; }
        [Required]
        public int Cau4 { get; set; }
        [Required]
        public int Cau5 { get; set; }
        [Required]
        public int Cau6 { get; set; }
        [Required]
        public int Cau7 { get; set; }
        [Required]
        public int Cau8 { get; set; }
        [Required]
        public int Cau9 { get; set; }
        [Required]
        public int Cau10 { get; set; }
        [Required]
        public int Cau11 { get; set; }
        [Required]
        public int Cau12 { get; set; }
        [Required]
        public int Cau13 { get; set; }
        [Required]
        public int Cau14 { get; set; }
        [Required]
        public int Cau15 { get; set; }
        [Required]
        public int Cau16 { get; set; }
        [Required]
        public int Cau17 { get; set; }
        [Required]
        public int Cau18 { get; set; }
        [Required]
        public int Cau19 { get; set; }
        [Required]
        public int Cau20 { get; set; }
        [Required]
        public int Cau21 { get; set; }
        [Required]
        public int Cau22 { get; set; }
        [Required]
        public int Cau23 { get; set; }
        [Required]
        public int Cau24 { get; set; }
        public string Grade { get; set; }

        public static readonly List<string> GradeList = new List<string>
    {
        "Loai Gioi",
        "Loai Kha",
        "Loai Trung binh",
        "Loai Yeu"
    };

        [Required]
        public int Score { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}