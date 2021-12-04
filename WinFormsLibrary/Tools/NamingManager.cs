using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

namespace WinFormsLibrary.Tools {
    public static class NamingManager {
        private static int s_mexForUntitled = 1;
        private static int s_lastUnsafed = 1;
        private static SortedSet<int> s_setOfUntitled = new SortedSet<int>();
        private static int s_previouslyOpened = 1;
        

        public static string GetNewUntitled() {
            int result = s_mexForUntitled;
            s_setOfUntitled.Add(s_mexForUntitled);
            while (s_setOfUntitled.Contains(s_mexForUntitled)) {
                s_mexForUntitled++;
            }
            return $"untitled_{result}";
        }

        public static bool RemoveUntitled(string name) {
            if (Regex.Match(name, "untitled_[0-9]+").Value == name && int.TryParse(name.Split('_')[1], out int index)) {
                s_mexForUntitled = Math.Min(s_mexForUntitled, index);
                return s_setOfUntitled.Remove(index);
            }
            return false;
        }

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

        public static string GetNewUnsavedFile(string extension) {
            return $"unsaved{s_lastUnsafed++}{extension}";
        }

        public static string GetNewPreviouslyOpened() {
            return $"PreviouslyOpenedFile{s_previouslyOpened++}";
        }

        public static string GetNewBackupFile(string fileName) {
            return $"{Path.GetFileNameWithoutExtension(fileName)}_" +
                $"{DateTime.Now.ToString("dd:MM:yyyy_hh:mm", DateTimeFormatInfo.InvariantInfo)}" +
                $"{Path.GetExtension(fileName)}";
        }
    }
}
