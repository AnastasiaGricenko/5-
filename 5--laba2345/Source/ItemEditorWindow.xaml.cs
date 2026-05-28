using System.Windows;
using DndAssistant.Model.Items;

namespace DndAssistant
{
    public partial class ItemEditorWindow : Window
    {
        public ItemEditorWindow(Item item)
        {
            InitializeComponent();
            DataContext = new ItemEditorViewModel(item);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
