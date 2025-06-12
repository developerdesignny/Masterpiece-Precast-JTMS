using System.ComponentModel.DataAnnotations;

namespace JTMS.Models
{
    public class ProjectNotifications
    {
        [Key]
        public Guid Id { get; set; }

        public string NotificationT { get; set; } = string.Empty;

        public Guid SubMoldId { get; set; }
        public virtual MoldDetailsModel SubMold { get; set; }
    }
}
