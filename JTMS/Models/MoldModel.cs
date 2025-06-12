using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JTMS.Models
{
    public class MoldModel : ObservableObject
    {
        [Key]
        public Guid Id { get; set; }
        public int MoldAmount { get; set; } = 1;
        public string? MoldSize { get; set; } = "";
        public string? MoldCode { get; set; } = "";
        public string? MoldCut { get; set; } = "";
        public string? MoldPhoto { get; set; } = "";
        public string? MoldInfo { get; set; } = "";
        public string? Cut { get; set; } = "=";
        public string? Paint { get; set; } = "=";
        public int? ProcessCompleteCount { get; set; }
        public int Process1Count { get; set; }
        public int Process2Count { get; set; }
        public int Process3Count { get; set; }
        public int Process4Count { get; set; }


        public bool AllProcessDone
        {
            get
            {
                if (ProcessCompleteCount == MoldAmount)
                    return true;
                else
                    return false;
            }
        }

        public Guid ProjectId { get; set; }
        public virtual ProjectModel Project { get; set; }
    }
}
