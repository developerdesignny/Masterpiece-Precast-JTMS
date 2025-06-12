using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JTMS.Models
{
    public class ProjectModel : ObservableObject
    {
        [Key]
        public Guid Id { get; set; }
        public string JobName { get; set; } = "";
        public DateTime JobDate { get; set; } = DateTime.Now;
        public string JobColor { get; set; } = "";
        public string? Address { get; set; } = "";
        public DateTime ProjectUpdateDate { get; set; } = DateTime.Now;
        public double PercentageCleared { get; set; }

        public Guid CompanyId { get; set; }
        public virtual CompanyModel Company { get; set; }

        private int _selectedIndex = -1;
        [NotMapped]
        public int selectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }
    }
}
