using System;
using System.IO;

namespace WinFormsLibrary.Tools {
    public static class SimpleLogsProvider {
        private const int s_logsCount = 20;
        private static FileInfo s_currentLog;
        private static DirectoryInfo s_logsDirectory;

        static SimpleLogsProvider() {
            s_logsDirectory = Directory.CreateDirectory("..\\..\\..\\logs");
            string path = $"{s_logsDirectory.FullName}\\logs{DateTime.Now.Ticks}.txt";

            if (s_logsDirectory.GetFileSystemInfos().Length > s_logsCount) {
                var files = s_logsDirectory.GetFileSystemInfos();
                for (int i = 0; i < files.Length; i++) {
                    files[i].Delete();
                }
            }

            File.Create(path).Close();
            s_currentLog = new FileInfo(path);
        }

        public static void WriteLine(string line, bool returnTime = true) {
            Write($"{line}{Environment.NewLine}", returnTime);
        }

        public static void Write(string text, bool returnTime = true) {
            if (returnTime)
                File.AppendAllText(s_currentLog.FullName, $"[{DateTime.Now.ToString()}]: ");
            File.AppendAllText(s_currentLog.FullName, text);
        }
    }
}