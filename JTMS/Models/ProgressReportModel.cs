using System.ComponentModel.DataAnnotations;

namespace JTMS.Models
{
    public class ProgressReportModel
    {
        [Key]
        public Guid Id { get; set; }
        public DateOnly ProcessDate { get; set; }
        public Guid ProjectId { get; set; }
        public virtual ProjectModel Project { get; set; }
        public string? MoldCode { get; set; } = "";
        public string? MoldProcess { get; set; } = "";
    }
}
