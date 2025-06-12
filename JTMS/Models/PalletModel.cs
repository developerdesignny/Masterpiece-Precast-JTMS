using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JTMS.Models
{
    public class PalletModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MoldCodes { get; set; }
        public string MoldIds { get; set; }
        public bool IsFilled { get; set; } = false;

        [NotMapped]
        public string Status
        {
            get
            {
                if (IsFilled)
                    return "Filled";
                else
                    return "Not Filled";
            }
        }

        public Guid ProjectId { get; set; }
        public virtual ProjectModel Project { get; set; }
    }
}
