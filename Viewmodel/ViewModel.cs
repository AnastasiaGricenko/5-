using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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
        private const string FilePath = "characters.json"; 

        public static void Save(IEnumerable<Character> characters)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(characters, options);
            File.WriteAllText(FilePath, json);
        }

        public static IEnumerable<Character> Load()
        {
            if (!File.Exists(FilePath)) return new List<Character>();
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Character>>(json) ?? new List<Character>();
        }
    }
    public class GMViewModel : DependencyObject
    {
        public ObservableCollection<Character> AllCharacters { get; set; } = new ObservableCollection<Character>();
        
        public ICollectionView FilteredCharacters { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand GenerateNameCommand { get; }

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
        }

        private bool FilterCharacter(object obj)
        {
            if (obj is not Character c) return false;

            bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) || 
                                 (c.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            
            bool matchesLocation = SelectedLocation == "Все" || c.Location == SelectedLocation;

            return matchesSearch && matchesLocation;
        }

        public void LoadFromFile()
        {
            var loadedCharacters = FileService.Load();
            AllCharacters.Clear();
            foreach (var character in loadedCharacters)
            {
                AllCharacters.Add(character);
            }
        }

        public void GenerateRandomName()
        {
            string[] firstNames = { "Арагорн", "Гимли", "Леголас", "Фродо", "Гэндальф" };
            string[] lastNames = { "Смелый", "Мудрый", "Железная Стопа", "Странник" };

            string newName = $"{firstNames[Random.Shared.Next(firstNames.Length)]} {lastNames[Random.Shared.Next(lastNames.Length)]}";

            GeneratedName = newName;
        }

        public void SaveToFile()
        {
            FileService.Save(AllCharacters);
        }
    }
}