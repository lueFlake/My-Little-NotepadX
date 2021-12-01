﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using WinFormsLibrary.Tools;
using WinFormsLibrary.Controls;
using System.Collections.Specialized;

namespace NotepadApplication {
    internal static class ConfigurationSetter {
        private static Configuration s_config;
        private static KeyValueConfigurationCollection s_settings;
        private static DirectoryInfo s_previouslyOpenedDirectory;
        private static char s_separator;
        private static string[] s_previouslyOpenedFileKeys;

        static ConfigurationSetter() {
            s_previouslyOpenedDirectory = Directory.CreateDirectory("..\\..\\..\\PreviouslyOpened");
            s_previouslyOpenedDirectory.Attributes |= FileAttributes.Hidden;

            s_config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            s_settings = s_config.AppSettings.Settings;

            s_separator = '|';
            s_previouslyOpenedFileKeys = s_settings.
                AllKeys.
                Where(x => x.StartsWith("PreviouslyOpenedFile")).
                ToArray();
        }

        private static string[] GetSettings(string key) {
            return s_settings[key].Value.Split(s_separator);
        }

        private static void SetSettings(string key, params string[] value) {
            string jointValue = string.Join(s_separator, value);
            if (!s_config.AppSettings.Settings.AllKeys.Contains(key)) {
                s_settings.Add(key, jointValue);
            }
            else {
                s_settings[key].Value = jointValue;
            }
        }

        private static void RemoveSettings(string key) {
            s_settings.Remove(key);
        }

        public static object AutoSaveFrequency {
            get => int.Parse(GetSettings("AutoSaveFrequency")[0]);
            set {
                string valueString = ValidateFrequency(value);
                SetSettings("AutoSaveFrequency", valueString);
            }
        }

        public static object BackupSaveFrequency {
            get => int.Parse(GetSettings("BackupSaveFrequency")[0]);
            set {
                string valueString = ValidateFrequency(value);
                SetSettings("BackupSaveFrequency", valueString);
            }
        }

        private static string ValidateFrequency(object value) {
            if (value == null)
                throw new ArgumentNullException($"Значение не может быть null.");
            int frequencyValue;
            string valueString = value.ToString();
            if (!(int.TryParse(valueString, out frequencyValue) && frequencyValue >= 0 && frequencyValue < 1000))
                throw new ArgumentException($"\"{value}\" не является целым числом от 0 до 1000.");
            return valueString;
        }

        public static bool AutoSaveEnabled {
            get => bool.Parse(GetSettings("AutoSaveEnabled")[0]);
            set => SetSettings("AutoSaveEnabled", value.ToString());
        }
        public static bool BackupSaveEnabled {
            get => bool.Parse(GetSettings("AutoSaveEnabled")[0]);
            set => SetSettings("AutoSaveEnabled", value.ToString());
        }

        public static int SelectedIndex {
            get => int.Parse(GetSettings("SelectedIndex")[0]);
            set => SetSettings("SelectedIndex", value.ToString());
        }
        public static List<int> AllBusyUntitled {
            get => GetSettings("AllBusyUntitled").Select(int.Parse).ToList();
            set => SetSettings("AllBusyUntitled", value.Select(x => x.ToString()).ToArray());
        }

        public static void Save() {
            s_config.Save(ConfigurationSaveMode.Minimal);
        }

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

        public static void ClearPreviouslyOpened() {
            foreach (var settings in s_previouslyOpenedFileKeys) {
                RemoveSettings(settings);
            }
            foreach (var file in s_previouslyOpenedDirectory.GetFiles()) {
                file.Delete();
            }
        }

        public static void SetPage(TextPage textPage) {
            string newPath;
            if (!textPage.IsSaved) {
                newPath = s_previouslyOpenedDirectory.FullName + "\\" + NamingManager.GetNewUnsaved();
            }
            else {
                newPath = textPage.FileFullName;
            }
            textPage.TextBox.SaveFile(newPath);
            SetSettings(NamingManager.GetNewPreviouslyOpened(),
                newPath,
                textPage.FileFullName,
                textPage.FileName,
                textPage.IsSaved.ToString(),
                textPage.Empty.ToString()
            );
        }
    }
}
