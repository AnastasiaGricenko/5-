using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DndAssistant.Commands;
using DndAssistant.Interfaces;
using DndAssistant.Model;
using DndAssistant.Model.Items;
using DndAssistant.Services;
using Microsoft.Win32;

namespace DndAssistant
{
    /// <summary>
    /// Паттерн Observer (Наблюдатель): GMViewModel наследует DependencyObject
    /// и объявляет свойства через DependencyProperty.
    ///
    /// WPF автоматически отслеживает изменения этих свойств и уведомляет подписчиков.
    /// Коллбек OnFilterCriteriaChanged — это и есть «наблюдатель»: он срабатывает
    /// каждый раз, когда меняются SearchText или SelectedLocation, и обновляет фильтр.
    ///
    /// ObservableCollection<Character> реализует тот же принцип:
    /// при добавлении/удалении персонажа список в UI обновляется автоматически.
    /// </summary>
    public class GMViewModel : DependencyObject
    {
        private readonly NameGenerator _nameGenerator = new NameGenerator();
        private readonly IFileService _fileService = new FileService();
        private GameSession _currentSession = new GameSession();

        public ObservableCollection<Character> AllCharacters { get; set; } = new ObservableCollection<Character>();
        public dynamic FilteredCharacters { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand GenerateNameCommand { get; }
        public ICommand AddWeaponCommand { get; }
        public ICommand AddArmorCommand { get; }
        public ICommand AddArtifactCommand { get; }
        public ICommand AddTrinketCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand RemoveCharacterCommand { get; }

        // Паттерн Observer: DependencyProperty с коллбеком — подписчик на изменение свойства
        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(GMViewModel),
                new PropertyMetadata("", OnFilterCriteriaChanged));

        public string SelectedLocation
        {
            get => (string)GetValue(SelectedLocationProperty);
            set => SetValue(SelectedLocationProperty, value);
        }
        public static readonly DependencyProperty SelectedLocationProperty =
            DependencyProperty.Register(nameof(SelectedLocation), typeof(string), typeof(GMViewModel),
                new PropertyMetadata("Все", OnFilterCriteriaChanged));

        public string GeneratedName
        {
            get => (string)GetValue(GeneratedNameProperty);
            set => SetValue(GeneratedNameProperty, value);
        }
        public static readonly DependencyProperty GeneratedNameProperty =
            DependencyProperty.Register(nameof(GeneratedName), typeof(string), typeof(GMViewModel),
                new PropertyMetadata(""));

        // Паттерн Observer: этот метод — «наблюдатель», он вызывается автоматически
        // при каждом изменении SearchText или SelectedLocation
        private static void OnFilterCriteriaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GMViewModel vm)
                vm.FilteredCharacters.Refresh();
        }

        public GMViewModel()
        {
            FilteredCharacters = CollectionViewSource.GetDefaultView(AllCharacters);
            FilteredCharacters.Filter = new Predicate<object>(FilterCharacter);

            SaveCommand = new RelayCommand(_ => SaveToFile());
            LoadCommand = new RelayCommand(_ => LoadFromFile());
            GenerateNameCommand = new RelayCommand(_ => GenerateRandomName());

            AddWeaponCommand = new RelayCommand(p => AddWeapon(p as Character), p => p is Character);
            AddArmorCommand = new RelayCommand(p => AddArmor(p as Character), p => p is Character);
            AddArtifactCommand = new RelayCommand(p => AddArtifact(p as Character), p => p is Character);
            AddTrinketCommand = new RelayCommand(p => AddTrinket(p as Character), p => p is Character);

            EditItemCommand = new RelayCommand(p => EditItem(p as Item), p => p is Item);
            RemoveCharacterCommand = new RelayCommand(p => RemoveCharacter(p as Character), p => p is Character);
        }

        private bool FilterCharacter(object obj)
        {
            if (obj is not Character c) return false;

            bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
                                 (c.ChName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);

            bool matchesLocation = SelectedLocation == "Все" || c.Location.Name == SelectedLocation;

            return matchesSearch && matchesLocation;
        }

        public void LoadFromFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите файл для загрузки",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true) return;

            _currentSession = _fileService.Load(dialog.FileName);
            AllCharacters.Clear();

            if (_currentSession.Groups.Count > 0)
            {
                foreach (var ch in _currentSession.Groups[0].Members)
                    AllCharacters.Add(ch);
            }
        }

        public void SaveToFile()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Выберите место для сохранения",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                DefaultExt = ".json",
                FileName = $"DnD_Session_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true) return;

            if (_currentSession.Groups.Count == 0)
                _currentSession.Groups.Add(new PlayerGroup { Name = "Main Party" });

            _currentSession.Groups[0].Members.Clear();
            foreach (var ch in AllCharacters)
                _currentSession.Groups[0].Members.Add(ch);

            _fileService.Save(_currentSession, dialog.FileName);

            MessageBox.Show(
                $"Сессия успешно сохранена в файл:\n{dialog.FileName}",
                "Дневник Мастера",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void GenerateRandomName() => GeneratedName = _nameGenerator.Generate();

        private void AddWeapon(Character? c)
        {
            if (c == null) return;

            c.Inventory.Items.Add(new Weapon
            {
                Name = "Короткий меч",
                Description = "Стандартное оружие ближнего боя.",
                Damage = 6,
                DamageType = "Piercing",
                WeaponType = "Sword",
                Weight = 3.0,
                IsMagical = false,
                MagicalBonus = 0
            });

            RefreshView();
        }

        private void AddArmor(Character? c)
        {
            if (c == null) return;

            c.Inventory.Items.Add(new Armor
            {
                Name = "Кольчуга",
                Description = "Легкая броня для защиты тела.",
                ArmorClass = 13,
                ArmorType = "Medium",
                Weight = 10.0,
                IsMagical = false,
                MagicalBonus = 0
            });

            RefreshView();
        }

        private void AddArtifact(Character? c)
        {
            if (c == null) return;

            c.Inventory.Items.Add(new Artifact
            {
                Name = "Осколок звезды",
                Description = "Таинственный артефакт, излучающий легкое свечение.",
                Power = "Светит в темноте и защищает от зла",
                Rarity = 4,
                Weight = 1.0
            });

            RefreshView();
        }

        private void AddTrinket(Character? c)
        {
            if (c == null) return;

            c.Inventory.Items.Add(new Trinket
            {
                Name = "Серебряная подвеска",
                Description = "Крошечный талисман с магическим символом.",
                Effect = "Улучшает удачу при бросках на проверку",
                Rarity = "uncommon",
                Weight = 0.2
            });

            RefreshView();
        }

        private void RemoveCharacter(Character? c)
        {
            if (c == null) return;

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить персонажа \"{c.ChName}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                AllCharacters.Remove(c);
                RefreshView();
            }
        }

        private void EditItem(Item? item)
        {
            if (item == null) return;

            var editor = new ItemEditorWindow(item)
            {
                Owner = Application.Current.MainWindow
            };

            if (editor.ShowDialog() == true)
            {
                (editor.DataContext as ItemEditorViewModel)?.SaveChanges();
                RefreshView();
            }
        }

        private void RefreshView() => FilteredCharacters.Refresh();
    }
}
