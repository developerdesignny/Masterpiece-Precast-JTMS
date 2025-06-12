using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JTMS.Models
{
    public enum ScanMode
    {
        Process1, Process2, Process3, Process4, Process5, Process6
    }

    public class MoldDetailsModel : ObservableObject
    {
        [Key]
        public Guid Id { get; set; }
        public int QIndex { get; set; }

        [NotMapped]
        public string MCode { get => $"{Mold.MoldCode}-{QIndex}"; }

        private bool? _process1Complete = false;
        private bool? _process2Complete = false;
        private bool? _process3Complete = false;
        private bool? _process4Complete = false;

        public bool AllProcessDone
        {
            get
            {
                if (Process1Complete == true && Process3Complete == true && Process4Complete == true)
                    return true;
                else
                    return false;
            }
        }

        public bool? Process1Complete
        {
            get => _process1Complete; set
            {
                SetProperty(ref _process1Complete, value);
            }
        }

        public bool? Process2Complete
        {
            get => _process2Complete; set
            {
                SetProperty(ref _process2Complete, value);
            }
        }

        public bool? Process3Complete
        {
            get => _process3Complete; set
            {
                SetProperty(ref _process3Complete, value);
            }
        }

        public bool? Process4Complete
        {
            get => _process4Complete; set
            {
                SetProperty(ref _process4Complete, value);
            }
        }

        public string CurrentProcess1
        {
            get
            {
                var currentPr = string.Empty;
                if (Process1Complete != true)
                    currentPr = "-";
                else
                    currentPr = "Done";
                return currentPr;
            }
        }

        public string CurrentProcess2
        {
            get
            {
                var currentPr = string.Empty;
                if (Process2Complete != true)
                    currentPr = "-";
                else
                    currentPr = "Done";
                return currentPr;
            }
        }

        public string CurrentProcess3
        {
            get
            {
                var currentPr = string.Empty;
                if (Process3Complete != true)
                    currentPr = "-";
                else
                    currentPr = "Done";
                return currentPr;
            }
        }

        public string CurrentProcess4
        {
            get
            {
                var currentPr = string.Empty;
                if (Process4Complete != true)
                    currentPr = "-";
                else
                    currentPr = "Done";
                return currentPr;
            }
        }

        public string CurrentProcess
        {
            get
            {
                var currentPr = string.Empty;
                if (Process4Complete == true)
                    currentPr = "Process 4 (Ready on Pallet)";
                else if (Process3Complete == true)
                    currentPr = "Process 3 (Pouring)";
                else if (Process2Complete == true)
                    currentPr = "Process 2 (Quality check)";
                else if (Process1Complete == true)
                    currentPr = "Process 1 (Mold Ready)";
                return currentPr;
            }
        }

        public string PalletID { get; set; } = "";
        public Guid MoldId { get; set; }
        public virtual MoldModel Mold { get; set; }

    }
}
