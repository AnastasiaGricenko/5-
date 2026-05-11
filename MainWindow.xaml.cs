
using System.Windows;
using System.Linq;

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
        }

        private void GenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerateRandomName();

            // Проверяем, существует ли уже персонаж с таким именем
            bool nameExists = _viewModel.AllCharacters.Any(c => c.ChName.Equals(_viewModel.GeneratedName, StringComparison.OrdinalIgnoreCase));
            
            if (nameExists)
            {
                MessageBox.Show(
                    $"Персонаж с именем \"{_viewModel.GeneratedName}\" уже существует!\nПопробуйте создать другого персонажа.",
                    "Дублирование имени",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var newNpc = new Character
            {
                ChName = _viewModel.GeneratedName,
                ChRace = "Неизвестно",
                ChDescription = "Внебапный гость...",
                Location = { Name = "Таверна", Description = "Тихая таверна в городе" },
                Inventory = { Copper = 10, Silver = 3, Gold = 5 }
            };

            _viewModel.AllCharacters.Add(newNpc);
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadFromFile();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveToFile();
        }
    }
}