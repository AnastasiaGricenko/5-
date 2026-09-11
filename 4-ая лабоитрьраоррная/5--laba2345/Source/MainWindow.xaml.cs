
using System.Windows;

namespace DndAssistant
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GMViewModel();
        }
    }
}
