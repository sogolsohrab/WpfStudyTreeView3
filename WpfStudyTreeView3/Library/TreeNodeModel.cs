using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStudyTreeView3.Properties;

namespace WpfStudyTreeView3.Library
{
    public class TreeNodeModel : PropertyChangedBase
    {
        private string name;
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string DisplayedImagePath { get; set; }

        public string ParentName { get; set; }

        public bool IsParentNode { get; set; }

        public ObservableCollection<TreeNodeModel> Items { get; set; }

        public TreeNodeModel(string name, string imagePath, string parentName)
        {
            Name = name;
            ParentName = parentName;
            DisplayedImagePath = imagePath;
            IsParentNode = (parentName == "");
            Items = new ObservableCollection<TreeNodeModel>();
        }
    }
}
