using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using WinFormsLibrary.Tools;
using WinFormsLibrary.Controls;
using System.Drawing;

namespace NotepadApplication {
    /// <summary>
    /// Класс контроля над конфигурацией приложения.
    /// 
    /// Все поля/свойства отвечают за взаимодействия с файлом конфигурации.
    /// </summary>
    internal static class ConfigurationSetter {
        private static Configuration s_config;
        private static KeyValueConfigurationCollection s_settings;
        private static DirectoryInfo s_previouslyOpenedDirectory;
        private static DirectoryInfo s_backupDirectory;
        private static char s_separator;
        private static string[] s_previouslyOpenedFileKeys;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        static ConfigurationSetter() {
            s_previouslyOpenedDirectory = Directory.CreateDirectory("..\\..\\..\\PreviouslyOpened");
            s_previouslyOpenedDirectory.Attributes |= FileAttributes.Hidden;
            s_backupDirectory = Directory.CreateDirectory("..\\..\\..\\Backups");
            s_backupDirectory.Attributes |= FileAttributes.Hidden;

            s_config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            s_settings = s_config.AppSettings.Settings;

            s_separator = '|';
            s_previouslyOpenedFileKeys = s_settings.
                AllKeys.
                Where(x => x.StartsWith("PreviouslyOpenedFile")).
                ToArray();
        }

        /// <summary>
        /// Получить значение запрашеваемого параметра.
        /// </summary>
        /// <param name="key">Запрашиваемый параметр.</param>
        /// <returns>Значение параметра.</returns>
        private static string[] GetSettings(string key) {
            return s_settings[key].Value.Split(s_separator);
        }

        
        /// <summary>
        /// Установить значение конкретного параметра.
        /// </summary>
        /// <param name="key">Изменяемый параметр.</param>
        /// <param name="value">Новое значение параметра.</param>
        private static void SetSettings(string key, params string[] value) {
            string jointValue = string.Join(s_separator, value);
            if (!s_config.AppSettings.Settings.AllKeys.Contains(key)) {
                s_settings.Add(key, jointValue);
            }
            else {
                s_settings[key].Value = jointValue;
            }
        }

        /// <summary>
        /// Удалить параметр из конфигурации.
        /// </summary>
        /// <param name="key">Удаляемый параметр.</param>
        private static void RemoveSettings(string key) {
            s_settings.Remove(key);
        }

        /// <summary>
        /// Частота автосохранения конфигурации.
        /// </summary>
        public static object AutoSaveFrequency {
            get => int.Parse(GetSettings("AutoSaveFrequency")[0]);
            set {
                string valueString = ValidateFrequency(value);
                SetSettings("AutoSaveFrequency", valueString);
            }
        }

        /// <summary>
        /// Частота журналирования.
        /// </summary>
        public static object BackupSaveFrequency {
            get => int.Parse(GetSettings("BackupSaveFrequency")[0]);
            set {
                string valueString = ValidateFrequency(value);
                SetSettings("BackupSaveFrequency", valueString);
            }
        }

        /// <summary>
        /// Валидация значений временных частот.
        /// </summary>
        /// <param name="value">Валидируемое значение</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Исключение при валидации null.</exception>
        /// <exception cref="ArgumentException">Исключение при некорректной валидации.</exception>
        private static string ValidateFrequency(object value) {
            if (value == null)
                throw new ArgumentNullException($"Значение не может быть null.");
            int frequencyValue;
            string valueString = value.ToString();
            if (!(int.TryParse(valueString, out frequencyValue) && frequencyValue >= 0 && frequencyValue < 1000))
                throw new ArgumentException($"\"{value}\" не является целым числом от 0 до 1000.");
            return valueString;
        }

        /// <summary>
        /// Проверка на автосохранение.
        /// </summary>
        public static bool AutoSaveEnabled {
            get => bool.Parse(GetSettings("AutoSaveEnabled")[0]);
            set => SetSettings("AutoSaveEnabled", value.ToString());
        }

        /// <summary>
        /// Проверка на журналирование.
        /// </summary>
        public static bool BackupSaveEnabled {
            get => bool.Parse(GetSettings("AutoSaveEnabled")[0]);
            set => SetSettings("AutoSaveEnabled", value.ToString());
        }

        /// <summary>
        /// Индекс открытой вкладки.
        /// </summary>
        public static int SelectedIndex {
            get => int.Parse(GetSettings("SelectedIndex")[0]);
            set => SetSettings("SelectedIndex", value.ToString());
        }

        public static List<int> AllBusyUntitled {
            get {
                try {
                    return GetSettings("AllBusyUntitled").Select(int.Parse).ToList();
                }
                catch {
                    return new List<int>() { 1 };
                }
            }
            set => SetSettings("AllBusyUntitled", value.Select(x => x.ToString()).ToArray());
        }

        /// <summary>
        /// Путь к выбранному компилятору.
        /// </summary>
        public static string CompilerPath {
            get => GetSettings("CompilerPath")[0];
            set => SetSettings("CompilerPath", value);
        }

        /// <summary>
        /// Цветовая расцветка.
        /// </summary>
        public static ColorStyle ColorTheme {
            get {
                if (int.TryParse(GetSettings("ColorTheme")[0], out _)) {
                    return new ColorStyle(
                        int.Parse(GetSettings("ColorTheme")[0]),
                        int.Parse(GetSettings("ColorTheme")[1])
                   );
                }
                else {
                    return new ColorStyle(GetSettings("ColorTheme")[0], GetSettings("ColorTheme")[1]);
                }
            }
            set => SetSettings("ColorTheme", value.MainBodyBackcolor.ToArgb().ToString(), value.MainBodyForecolor.ToArgb().ToString());
        }


        /// <summary>
        /// Цвета подсветки синтаксиса.
        /// </summary>
        public static Dictionary<string, Color> SyntaxColors {
            get {
                var colors = new Dictionary<string, Color>();
                var colorData = GetSettings("SyntaxColors");
                foreach (var item in colorData) {
                    string[] value = item.Split(":");
                    colors[value[0]] = Color.FromArgb(int.Parse(value[1]));
                }
                return colors;
            }
            set {
                var colorData = new string[value.Count];
                int index = 0;
                foreach (var item in value) {
                    colorData[index++] = $"{item.Key}:{item.Value.ToArgb()}";
                }
                SetSettings("SyntaxColors", colorData);
            }
        }

        /// <summary>
        /// Основной шрифт редактора.
        /// </summary>
        public static Font MainFont {
            get {
                return new Font(
                    GetSettings("MainFont")[0],
                    float.Parse(GetSettings("MainFont")[1]),
                    (FontStyle)int.Parse(GetSettings("MainFont")[2])
                );
            }
            set {
                SetSettings(
                    "MainFont",
                    value.FontFamily.Name,
                    value.Size.ToString(),
                    ((int)value.Style).ToString()
                );
            }
        }

        /// <summary>
        /// Сохранить конфигурацию.
        /// </summary>
        public static void Save() {
            s_config.Save(ConfigurationSaveMode.Minimal);
        }

        /// <summary>
        /// Получить список вкладок из прошлого сеанса.
        /// </summary>
        /// <returns>Список вкладок (TextPage)</returns>
        public static List<TextPage> GetPreviouslyOpenedFiles() {
            var previouslyOpenedFiles = new List<TextPage>();
            for (var i = 0; i < s_previouslyOpenedFileKeys.Length; i++) {
                string key = s_previouslyOpenedFileKeys[i];
                string[] values = GetSettings(key);
                if (File.Exists(values[1]) || values[1] == "") {
                    previouslyOpenedFiles.Add(new TextPage(
                        new FileInfo(values[0]),
                        values[1],
                        values[2],
                        bool.Parse(values[3]),
                        bool.Parse(values[4])
                    ));
                }
            }
            return previouslyOpenedFiles;
        }

        /// <summary>
        /// Очистить хранилище файлов прошлого сеанса.
        /// </summary>
        public static void ClearPreviouslyOpened() {
            foreach (var settings in s_previouslyOpenedFileKeys) {
                RemoveSettings(settings);
            }
            foreach (var file in s_previouslyOpenedDirectory.GetFiles()) {
                file.Delete();
            }
        }

        /// <summary>
        /// Сохранение вкладки в конфигурации и локальное хранилище.
        /// </summary>
        /// <param name="textPage">Сохраняемая вкладка.</param>
        public static void SavePage(TextPage textPage) {
            string newPath;
            if (!textPage.IsSaved) {
                newPath = s_previouslyOpenedDirectory.FullName + "\\" + NamingManager.GetNewUnsavedFile(
                    Path.GetExtension(textPage.FileFullName == "" ? ".txt" : textPage.FileFullName)
                );
            }
            else {
                newPath = textPage.FileFullName;
            }
            SetSettings(
                NamingManager.GetNewPreviouslyOpened(),
                newPath,
                textPage.FileFullName,
                textPage.FileName,
                textPage.IsSaved.ToString(),
                textPage.Empty.ToString()
            );
            textPage.SaveFile(new FileInfo(newPath));
        }

        /// <summary>
        /// Создание и сохранение отката файла вкладки.
        /// </summary>
        /// <param name="textPage">Сохраняемая вкладка.</param>
        public static void CreateBackup(TextPage textPage) {
            if (!textPage.IsUntitled) {
                string backupSubdirectoryName = textPage.FileFullName.GetHashCode().ToString();
                backupSubdirectoryName = $"{s_backupDirectory.FullName}/{backupSubdirectoryName}";
                DirectoryInfo backupSubdirectory = Directory.CreateDirectory(backupSubdirectoryName);
                string backupName = NamingManager.GetNewBackupFile(textPage.FileName);
                string newPath = $"{backupSubdirectoryName}/{backupName}";
                SetSettings(
                    backupName,
                    newPath,
                    textPage.FileFullName,
                    textPage.FileName,
                    "True",
                    "False"
                );
                textPage.SaveFile(new FileInfo(newPath), false);
            }
        }
    }
}
