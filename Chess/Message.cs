using System;
using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Message
    {
        private static Dictionary<Level, Func<string, string, MessageBoxResult>> decider
            = new Dictionary<Level, Func<string, string, MessageBoxResult>>() {
            { Level.Question, ShowQuestion },
            { Level.Warning, ShowWarning },
            { Level.Error, ShowError },
            { Level.Info, ShowInfo }
        };

        public static MessageBoxResult StartBox(Level level, string message, string header = null)
        {
            return decider[level](message, header);
        }

        private static MessageBoxResult ShowWarning(string message, string header)
        {
            return MessageBox.Show(message, header ?? "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static MessageBoxResult ShowError(string message, string header)
        {
            return MessageBox.Show(message, header ?? "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static MessageBoxResult ShowQuestion(string message, string header)
        {
            return MessageBox.Show(message, header ?? "What do you want to do?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private static MessageBoxResult ShowInfo(string message, string header)
        {
            return MessageBox.Show(message, header ?? "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public enum Level
    {
        Info,
        Question,
        Warning,
        Error,
    }
}
