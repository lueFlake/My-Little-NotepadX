using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WinFormsLibrary.Tools {
    /// <summary>
    /// Класс для работы с названифми различных объектов.
    /// </summary>
    public static class NamingManager {
        /// <summary>
        /// Минимальный допустимый номер для называния пустой вкладки.
        /// </summary>
        private static int s_mexForUntitled = 1;
        /// <summary>
        /// Минимальный допустимый номер для называния несохраненного файла для записи в конфигурацию.
        /// </summary>
        private static int s_lastUnsafed = 1;
        /// <summary>
        /// Минимальный допустимый номер для ключа несохраненного файла для записи в конфигурацию.
        /// </summary>
        private static int s_lastPreviouslyOpened = 1;
        /// <summary>
        /// Локальная коллекция занятых имен полностью несохраненных файлов.
        /// </summary>
        private static SortedSet<int> s_setOfUntitled = new SortedSet<int>();
        

        /// <summary>
        /// Получить новое название пустой вкладки.
        /// </summary>
        /// <returns>Название пустой вкладки.</returns>
        public static string GetNewUntitled() {
            int result = s_mexForUntitled;
            s_setOfUntitled.Add(s_mexForUntitled);
            while (s_setOfUntitled.Contains(s_mexForUntitled)) {
                s_mexForUntitled++;
            }
            return $"untitled_{result}";
        }

        /// <summary>
        /// Убрать название пустой вкладки из локальной коллекции.
        /// </summary>
        /// <param name="name">Удаляемое название.</param>
        /// <returns>Результат удаления: true, если файл успешно удален.</returns>
        public static bool RemoveUntitled(string name) {
            if (Regex.Match(name, "untitled_[0-9]+").Value == name && int.TryParse(name.Split('_')[1], out int index)) {
                s_mexForUntitled = Math.Min(s_mexForUntitled, index);
                return s_setOfUntitled.Remove(index);
            }
            return false;
        }

        /// <summary>
        /// Свойство для работы с коллекцией занятых имен полностью несохраненных файлов.
        /// </summary>
        /// <returns>Коллекция занятых имен.</returns>
        public static List<int> AllBusyUntitled {
            get => s_setOfUntitled.ToList();
            set {
                if (value == null) {
                    s_setOfUntitled = new SortedSet<int>();
                    s_setOfUntitled.Add(1);
                    return;
                }
                s_setOfUntitled = new SortedSet<int>(value); 
                while (s_setOfUntitled.Contains(s_mexForUntitled)) {
                    s_mexForUntitled++;
                }
            }
        }

        /// <summary>
        /// Получить название для сохранения несохраненного файла в конфигурацию.
        /// </summary>
        /// <returns>Новое название.</returns>
        public static string GetNewUnsavedFile(string extension) {
            return $"unsaved{s_lastUnsafed++}{extension}";
        }

        /// <summary>
        /// Получить название для нового ключа для сохранения в конфигурацию.
        /// </summary>
        /// <returns>Новое название.</returns>
        public static string GetNewPreviouslyOpened() {
            return $"PreviouslyOpenedFile{s_lastPreviouslyOpened++}";
        }

        /// <summary>
        /// Получить название для нового откатываемого файла.
        /// </summary>
        /// <returns>Новое название.</returns>
        public static string GetNewBackupFile(string fileName) {
            return $"{Path.GetFileNameWithoutExtension(fileName)}__" +
                $"{DateTime.Now.ToString("dd-MM-yyyy__hh-mm", DateTimeFormatInfo.InvariantInfo)}" +
                $"{Path.GetExtension(fileName)}";
        }
    }
}
