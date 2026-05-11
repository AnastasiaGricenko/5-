using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DndAssistant.Model;
using DndAssistant.Model.Items;
using DndAssistant.Services;
using Microsoft.Win32;

namespace DndAssistant
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
    }

    public static class FileService
    {
        private const string DefaultFilePath = "gamesessions.json"; 

        public static void Save(GameSession session, string? filePath = null)
        {
            var path = filePath ?? DefaultFilePath;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(session, options);
            File.WriteAllText(path, json);
        }

        public static GameSession Load(string? filePath = null)
        {
            var path = filePath ?? DefaultFilePath;
            if (!File.Exists(path)) return new GameSession();
            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions();
            return JsonSerializer.Deserialize<GameSession>(json, options) ?? new GameSession();
        }
    }
    public class GMViewModel : DependencyObject
    {
        private readonly NameGenerator _nameGenerator = new NameGenerator();
        private GameSession _currentSession = new GameSession();

        public ObservableCollection<Character> AllCharacters { get; set; } = new ObservableCollection<Character>();
        
        public ICollectionView FilteredCharacters { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand GenerateNameCommand { get; }
        public ICommand AddWeaponCommand { get; }
        public ICommand AddArmorCommand { get; }
        public ICommand AddArtifactCommand { get; }
        public ICommand AddTrinketCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand RemoveCharacterCommand { get; }

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

        private static void OnFilterCriteriaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GMViewModel vm)
            {
                vm.FilteredCharacters.Refresh();
            }
        }

        public GMViewModel()
        {
            FilteredCharacters = CollectionViewSource.GetDefaultView(AllCharacters);
            FilteredCharacters.Filter = FilterCharacter;

            SaveCommand = new RelayCommand(_ => SaveToFile());
            LoadCommand = new RelayCommand(_ => LoadFromFile());
            GenerateNameCommand = new RelayCommand(_ => GenerateRandomName());
            AddWeaponCommand = new RelayCommand(param => AddWeapon(param as Character), param => param is Character);
            AddArmorCommand = new RelayCommand(param => AddArmor(param as Character), param => param is Character);
            AddArtifactCommand = new RelayCommand(param => AddArtifact(param as Character), param => param is Character);
            AddTrinketCommand = new RelayCommand(param => AddTrinket(param as Character), param => param is Character);
            EditItemCommand = new RelayCommand(param => EditItem(param as Item), param => param is Item);
            RemoveCharacterCommand = new RelayCommand(param => RemoveCharacter(param as Character), param => param is Character);
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
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл для загрузки",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _currentSession = FileService.Load(openFileDialog.FileName);
                AllCharacters.Clear();
                if (_currentSession.Groups.Count > 0)
                {
                    foreach (var character in _currentSession.Groups[0].Members)
                    {
                        AllCharacters.Add(character);
                    }
                }
            }
        }

        public void GenerateRandomName()
        {
            GeneratedName = _nameGenerator.Generate();
        }

        private void AddWeapon(Character? character)
        {
            if (character == null) return;
            character.Inventory.Items.Add(new Weapon
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

        private void AddArmor(Character? character)
        {
            if (character == null) return;
            character.Inventory.Items.Add(new Armor
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

        private void AddArtifact(Character? character)
        {
            if (character == null) return;
            character.Inventory.Items.Add(new Artifact
            {
                Name = "Осколок звезды",
                Description = "Таинственный артефакт, излучающий легкое свечение.",
                Power = "Светит в темноте и защищает от зла",
                Rarity = 4,
                Weight = 1.0
            });
            RefreshView();
        }

        private void AddTrinket(Character? character)
        {
            if (character == null) return;
            character.Inventory.Items.Add(new Trinket
            {
                Name = "Серебряная подвеска",
                Description = "Крошечный талисман с магическим символом.",
                Effect = "Улучшает удачу при бросках на проверку",
                Rarity = "uncommon",
                Weight = 0.2
            });
            RefreshView();
        }

        private void RefreshView()
        {
            FilteredCharacters.Refresh();
        }

        private void RemoveCharacter(Character? character)
        {
            if (character == null) return;

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить персонажа \"{character.ChName}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                AllCharacters.Remove(character);
                RefreshView();
            }
        }

        private void EditItem(Item? item)
        {
            if (item == null) return;

            var editorWindow = new ItemEditorWindow(item)
            {
                Owner = Application.Current.MainWindow
            };

            if (editorWindow.ShowDialog() == true)
            {
                var viewModel = editorWindow.DataContext as ItemEditorViewModel;
                viewModel?.SaveChanges();
                RefreshView();
            }
        }

        public void SaveToFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Выберите место для сохранения",
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                DefaultExt = ".json",
                FileName = $"DnD_Session_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Ensure we have a default group for characters
                if (_currentSession.Groups.Count == 0)
                {
                    _currentSession.Groups.Add(new PlayerGroup { Name = "Main Party" });
                }

                _currentSession.Groups[0].Members.Clear();
                foreach (var character in AllCharacters)
                {
                    _currentSession.Groups[0].Members.Add(character);
                }

                FileService.Save(_currentSession, saveFileDialog.FileName);

                MessageBox.Show(
                    $"Сессия успешно сохранена в файл:\n{saveFileDialog.FileName}",
                    "Дневник Мастера",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}                                                  
























//нукарочевоттакиепироги
