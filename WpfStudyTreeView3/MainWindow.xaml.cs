using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfStudyTreeView3.Library;
using static WpfStudyTreeView3.MainWindow;

namespace WpfStudyTreeView3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TreeNodeViewModel ViewModel;

        public List<TreeNodeModel> TreeParentsList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
            LoadTree(GetSampleTreeNode());
            FillTree();
        }

        #region Private Methods

        private void LoadTree(List<TreeNodeModel> sampleData)
        {
            ViewModel = new TreeNodeViewModel();
            TreeParentsList = new List<TreeNodeModel>();

            foreach (TreeNodeModel node in sampleData)
            {
                if (node.ParentName == "")
                {
                    ViewModel.Items.Add(node);
                }
                else
                {
                    TreeNodeModel? parent = sampleData.Find(x => x.Name == node.ParentName);
                    if (parent != null)
                    {
                        parent.Items.Add(node);
                        TreeParentsList.Add(parent);
                    }
                }
            }

            SortTree();
        }

        private static List<TreeNodeModel> GetSampleTreeNode()
        {
            List<string> wellMembers = new() { "Wells", "R1_W7", "R1_W12345678912", "R1_W123456789111", "R2_W1", "R2_W11", "R2_W03", "R2_W123456789123456789", "R1_W1", "R1_W012" };
            List<string> polygonMembers = new() { "Polygons" };
            List<string> rockMembers = new() { "Rocks", "R4", "R2", "R1", "R3", "R7", "R6", "R5" };
            List<string> wellStrategyMembers = new() { "Well Strategies", "WS6", "WS2", "WS4", "WS1", "WS3", "WS5" };
            List<string> r1Members = new() { "R1", "R1_11", "R1_02", "R1_13" };
            List<string> r2Members = new() { "R2", "R2_21", "R2_4", "R2_1", "R2_15" };
            List<string> r3Members = new() { "R2_1", "R2_1_2", "R2_1_1", "R2_1_4", "R2_1_3" };

            List<TreeNodeModel> result = new();

            CreateMembers(result, wellMembers, Constants.ImagePath.WellImagePath, Constants.ImagePath.WellChildImagePath);
            CreateMembers(result, polygonMembers, Constants.ImagePath.PolygonImagePath, Constants.ImagePath.PolygonImagePath);
            CreateMembers(result, rockMembers, Constants.ImagePath.RockImagePath, Constants.ImagePath.RockChildImagePath);
            CreateMembers(result, wellStrategyMembers, Constants.ImagePath.WellStrategyImagePath, Constants.ImagePath.WellStrategyChildImagePath);
            CreateMembers(result, r1Members, Constants.ImagePath.RockChildImagePath, Constants.ImagePath.RockChild2ImagePath);
            CreateMembers(result, r2Members, Constants.ImagePath.RockChildImagePath, Constants.ImagePath.RockChild2ImagePath);
            CreateMembers(result, r3Members, Constants.ImagePath.RockChild2ImagePath, Constants.ImagePath.RockChild3ImagePath);

            return result;
        }

        private static void CreateMembers(List<TreeNodeModel> treeNodesList, List<string> membersNameArray, string parentImagePath, string childImagePath)
        {
            // Add Parents
            if (!treeNodesList.Any(x => x.Name == membersNameArray[0]))
            {
                treeNodesList.Add(new TreeNodeModel(membersNameArray[0], parentImagePath, ""));
            }

            // Add Children
            if (membersNameArray.Count > 1)
            {
                foreach (string childName in membersNameArray.GetRange(1, membersNameArray.Count - 1))
                {
                    treeNodesList.Add(new TreeNodeModel(childName, childImagePath, membersNameArray[0]));
                }
            }
        }

        private void SortTree()
        {
            // Sort Parents
            CustomOrder.OrderModel(ViewModel.Items);

            // Sort Children
            foreach (TreeNodeModel item in TreeParentsList)
            {
                CustomOrder.OrderModel(item.Items);
            }
        }

        private void FillTree()
        {
            MyTree.ItemTemplate = CreateDataTemplate();
            MyTree.ItemContainerStyle = CreateStyle();
            MyTree.ItemsSource = ViewModel.Items;
            MyTree.SelectedItemChanged += OnSelectedItemChanged;
        }

        private static HierarchicalDataTemplate CreateDataTemplate()
        {
            HierarchicalDataTemplate dataTemplate = new()
            {
                ItemsSource = new Binding() { Path = new PropertyPath("Items") }
            };

            FrameworkElementFactory stackPanel = new(typeof(StackPanel))
            {
                Name = "parentStackPanel"
            };
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory image = new(typeof(Image));
            image.SetValue(Image.MarginProperty, new Thickness(2));
            image.SetValue(Image.WidthProperty, 16.0);
            image.SetValue(Image.HeightProperty, 16.0);
            image.SetBinding(Image.SourceProperty, new Binding() { Path = new PropertyPath("DisplayedImagePath") });
            stackPanel.AppendChild(image);

            FrameworkElementFactory textBlock = new(typeof(TextBlock));
            textBlock.SetValue(TextBlock.MarginProperty, new Thickness(5));
            textBlock.SetValue(TextBlock.FontSizeProperty, 12.0);
            textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Name") });
            stackPanel.AppendChild(textBlock);

            dataTemplate.VisualTree = stackPanel;

            return dataTemplate;
        }

        private Style CreateStyle()
        {
            var style = new Style { TargetType = typeof(TreeViewItem) };
            var eventSetter = new EventSetter(PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseRightButtonDown));
            style.Setters.Add(eventSetter);

            return style;
        }


        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedItem == null)
            {
                return;
            }

            ContextMenu contextMenu = new();
            MenuItem renameMenuItem = CreateMenuItem("Rename", Constants.ImagePath.RenameIconImagePath, RenameMenuItem_Click);
            MenuItem deleteMenuItem = CreateMenuItem("Delete", Constants.ImagePath.DeleteIconImagePath, DeleteMenuItem_Click);

            if (ViewModel.SelectedItem.IsParentNode)
            {
                contextMenu.Items.Add(renameMenuItem);
            }
            else
            {
                contextMenu.Items.Add(renameMenuItem);
                contextMenu.Items.Add(deleteMenuItem);
            }
            ((TreeViewItem)sender).ContextMenu = contextMenu;
        }

        private static MenuItem CreateMenuItem(String header, String imagePath, RoutedEventHandler routedEventHandler)
        {
            MenuItem menuItem = new()
            {
                Header = header,
                Icon = new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Relative))
                }
            };
            menuItem.Click += routedEventHandler;

            return menuItem;
        }

        private void UpdatePanelsVisibility(Border selectedBorder)
        {
            List<Border> bordersList = new() { HomePanel, RenamePanel, DeletePanel };

            foreach (var border in bordersList)
            {
                border.Visibility = Visibility.Collapsed;
            }

            selectedBorder.Visibility = Visibility.Visible;
        }
        #endregion Private Methods


        #region Events

        #region SelectedItemChanged
        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedItem = (TreeNodeModel)e.NewValue;
        }
        #endregion SelectedItemChanged


        #region Delete Events
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility(DeletePanel);
        }

        private void DeleteAbortButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility(HomePanel);
        }

        private void DeleteProceedButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeNodeModel item in TreeParentsList)
            {
                item.Items.Remove(ViewModel.SelectedItem);
            }

            UpdatePanelsVisibility(HomePanel);
        }
        #endregion Delete Events


        #region Rename Events
        private void RenameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdatePanelsVisibility(RenamePanel);
            renameTextBox.Text = ViewModel.SelectedItem.Name ?? "";
        }

        private void RenameAbortButton_Click(object sender, RoutedEventArgs e)
        {
            errorLabel.Content = string.Empty;
            UpdatePanelsVisibility(HomePanel);
        }

        private void RenameProceedButton_Click(object sender, RoutedEventArgs e)
        {

            bool IsEditingPossible = CheckEditingPossibility();
            if (IsEditingPossible)
            {
                ViewModel.SelectedItem.Name = renameTextBox.Text;
                UpdatePanelsVisibility(HomePanel);
                SortTree();
                errorLabel.Content = string.Empty;
            }
            else
            {
                UpdatePanelsVisibility(RenamePanel);
            }
            renameTextBox.Clear();
        }

        private bool CheckEditingPossibility()
        {
            // Check Duplicate Nodes
            bool IsDuplicate = false;

            foreach (TreeNodeModel item in TreeParentsList)
            {
                if ((item.Items.Any(x => x.Name == renameTextBox.Text)) || (item.Name == renameTextBox.Text))
                {
                    IsDuplicate = true;
                }
            }

            // Check Validity
            bool IsEditingPossible = false;

            if (renameTextBox.Text.Length < 2)
            {
                errorLabel.Content = "** Please choose a name with more than 2 characters!";
            }
            else if (IsDuplicate)
            {
                errorLabel.Content = "** Duplicated name. Please choose a new one!";
            }
            else
            {
                IsEditingPossible = true;
            }

            return IsEditingPossible;
        }
        #endregion Rename Events

        #endregion Events


        #region Window style Events
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }
        #endregion Window style Events

    }
}
