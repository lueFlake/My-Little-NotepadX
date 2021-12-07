using System;
using System.IO;
using System.Windows.Forms;

namespace WinFormsLibrary.Tools {
    /// <summary>
    /// Класс для создания диалогов с пользователем.
    /// </summary>
    public static class MessageTools {
        /// <summary>
        /// Файловый диалог.
        /// </summary>
        private static SaveFileDialog s_dialog;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        static MessageTools() {
            s_dialog = new SaveFileDialog() {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Title = "save file as..",
                FileName = "untitled",
                DefaultExt = "txt",
                Filter = "Plain text file (.txt)|*.txt|Rich Text Format File (.rtf)|*.rtf|C# source file (.cs)|*.cs|Any file (*.*)|*.*",
                FilterIndex = 2
            };
        }

        /// <summary>
        /// Вывод сообщения об исключении.
        /// </summary>
        /// <param name="localLine">Сообщение об исключении.</param>
        /// <param name="ex">Вызванное исключение.</param>
        public static void ShowException(string localLine, Exception ex) {
            string EOL = Environment.NewLine;
            MessageBox.Show(
                $"{localLine}{EOL}" +
                $"{ex.Message}{EOL}" +
                $"{ex.Data}{EOL}" +
                $"{ex.HelpLink}{EOL}"
                );
        }

        /// <summary>
        /// Вывод диалога сохранения документа.
        /// </summary>
        /// <returns>Путь для сохранения.</returns>
        public static FileInfo? ShowSaveFileDialog() {
            if (s_dialog.ShowDialog() != DialogResult.OK) {
                return null;
            }

            var fileInfo = new FileInfo(s_dialog.FileName);
            if (fileInfo == null || !fileInfo.Directory.Exists)
                return null;

            return fileInfo;
        }
    }
}
