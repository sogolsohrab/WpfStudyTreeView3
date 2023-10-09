using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStudyTreeView3.Library
{
    public class TreeNodeViewModel
    {
        public TreeNodeModel SelectedItem { get; set; }

        public ObservableCollection<TreeNodeModel> Items { get; set; }

        public TreeNodeViewModel()
        {
            Items = new ObservableCollection<TreeNodeModel>();
        }
    }
}
