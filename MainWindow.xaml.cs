
using System.Windows;

namespace DndAssistant 
{
    public partial class MainWindow : Window
    {
        private GMViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new GMViewModel();
            
            this.DataContext = _viewModel;

            _viewModel.LoadFromFile();
        }

        private void GenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            string randomName = _viewModel.GenerateRandomName();

            var newNpc = new Character
            {
                Name = randomName,
                Race = "Неизвестно",
                Location = "Таверна", 
                Description = "Внезапный гость...",
                Inventory = "Медные монеты: 5"
            };

            _viewModel.AllCharacters.Add(newNpc);
            _viewModel.NotifyCharactersChanged();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveToFile();
            
            MessageBox.Show(
                "Все персонажи, локации и их инвентарь успешно сохранены!", 
                "Дневник Мастера", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }
    }
}