using System.ComponentModel.DataAnnotations;

namespace JTMS.Models
{
    public class CompanyModel
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int NumberOfJobs { get; set; }
    }
}
