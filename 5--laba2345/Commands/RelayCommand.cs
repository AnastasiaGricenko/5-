using System;
using System.Windows.Input;

namespace DndAssistant.Commands
{
    /// <summary>
    /// Паттерн Command (Команда): универсальная реализация ICommand.
    ///
    /// Инкапсулирует действие (_execute) и условие его доступности (_canExecute)
    /// в отдельном объекте, отделяя логику выполнения от UI-кнопки.
    /// </summary>
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
}
