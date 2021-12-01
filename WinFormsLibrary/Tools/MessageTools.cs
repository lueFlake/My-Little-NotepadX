using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsLibrary.Tools {
    public static class MessageTools {
        private static SaveFileDialog s_dialog;

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
        public static void ShowException(string localLine, Exception ex) {
            string EOL = Environment.NewLine;
            MessageBox.Show(
                $"{localLine}{EOL}" +
                $"{ex.Message}{EOL}" +
                $"{ex.Data}{EOL}" +
                $"{ex.HelpLink}{EOL}"
                );
        }

        public static FileInfo ShowFileDialog() {
            if (s_dialog.ShowDialog() != DialogResult.OK) {
                return null;
            }

            var fileInfo = new FileInfo(s_dialog.FileName);
            if (!fileInfo.Directory.Exists)
                return null;

            return new FileInfo(s_dialog.FileName);
        }
    }
}
