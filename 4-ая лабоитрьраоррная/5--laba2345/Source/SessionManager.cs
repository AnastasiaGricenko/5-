using System.ComponentModel;
using DndAssistant.Model;

namespace DndAssistant.Services
{
    /// <summary>
    /// Паттерн Singleton (Одиночка) — гарантирует существование ровно одного
    /// экземпляра менеджера сессии и предоставляет глобальную точку доступа к нему.
    ///
    /// Зачем это нужно:
    /// Текущая GameSession и путь к файлу должны быть едины для всего приложения.
    /// GMViewModel и другие компоненты обращаются к одному и тому же объекту,
    /// не создавая его заново каждый раз.
    ///
    /// Как это работает:
    /// 1) Конструктор приватный — создать через new нельзя.
    /// 2) Единственный экземпляр хранится в приватном статическом поле _instance.
    /// 3) GetInstance() возвращает его, создавая при первом обращении (lazy init).
    /// 4) INotifyPropertyChanged позволяет UI-слою подписаться на изменение LastFilePath.
    /// </summary>
    public class SessionManager : INotifyPropertyChanged
    {
        private static SessionManager? _instance;

        /// <summary>Глобальная точка доступа к единственному экземпляру.</summary>
        public static SessionManager GetInstance()
        {
            return _instance ??= new SessionManager();
        }

        private GameSession _currentSession = new();
        private string _lastFilePath = string.Empty;

        /// <summary>Текущая игровая сессия.</summary>
        public GameSession CurrentSession
        {
            get => _currentSession;
            set
            {
                if (_currentSession != value)
                {
                    _currentSession = value;
                    OnPropertyChanged(nameof(CurrentSession));
                }
            }
        }

        /// <summary>Путь к последнему открытому/сохранённому файлу.</summary>
        public string LastFilePath
        {
            get => _lastFilePath;
            set
            {
                if (_lastFilePath != value)
                {
                    _lastFilePath = value;
                    OnPropertyChanged(nameof(LastFilePath));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
