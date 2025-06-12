using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JTMS.Models
{
    public class ProjectNotesModels : ObservableObject
    {
        [Key]
        public Guid Id { get; set; }
        private string _name = string.Empty;
        public string Name
        {
            get => _name; set => SetProperty(ref _name, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description; set => SetProperty(ref _description, value);
        }

        private string _note = string.Empty;
        public string Note
        {
            get => _note; set => SetProperty(ref _note, value);
        }
        public Guid ProjectId { get; set; }
        public virtual ProjectModel Project { get; set; }
    }
}
