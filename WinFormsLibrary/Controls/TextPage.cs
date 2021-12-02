using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsLibrary.Tools;

namespace WinFormsLibrary.Controls {
    public class TextPage : TabPage {
        public static ContextMenuStrip ContextMenuStripForTextBoxes { get; set; }

        private RichTextBox _textBox;
        private RichTextBox _localBackup;

        private FileInfo _fileInfo;
        private bool _isSaved;
        private bool _empty;
        private string _untitled;

        public RichTextBox TextBox => _textBox;

        public bool IsUntitled => _fileInfo == null;

        public string FileName => !IsUntitled ? _fileInfo.Name : _untitled;
        public string FileFullName => !IsUntitled ? _fileInfo.FullName : "";

        public bool IsSaved => _isSaved;
        public bool Empty => _empty;

        public static ColorStyle TextBoxDefaultColor { get; set; }

        public TextPage(FileInfo file = null) : base() {
            _fileInfo = file;

            _textBox = new RichTextBox() {
                Dock = DockStyle.Fill,
                Multiline = true,
                ContextMenuStrip = ContextMenuStripForTextBoxes
            };

            _localBackup = new RichTextBox() {
                Dock = DockStyle.Fill,
                Multiline = true,
                ContextMenuStrip = ContextMenuStripForTextBoxes
            };

            if (!IsUntitled) {
                if (file.Extension is ".txt" or ".rtf" or ".cs") {
                    _textBox.LoadFile(file.FullName);
                }
                else {
                    _textBox.Text = File.ReadAllText(file.FullName);
                }
                _isSaved = true;
            }
            else {
                _untitled = NamingManager.GetNewUntitled();
                _isSaved = false;
            }
            _empty = IsUntitled;
            _textBox.TextChanged += TextBoxChangeEventHandler;
            Controls.Add(_textBox);
            ColorStyle.ChangeColorScheme(TextBoxDefaultColor, this);
            UpdateText();
        }

        public TextPage(FileInfo bufferedFile, string outputFile, string text, bool isSaved, bool empty) : this(bufferedFile) {
            _fileInfo = outputFile == "" ? null : new FileInfo(outputFile);
            if (IsUntitled)
                _untitled = text;
            _isSaved = isSaved;
            _empty = empty;
            UpdateText();
        }

        public void SaveFile(FileInfo file) {
            _fileInfo = file;
            _textBox.SaveFile(file.FullName);
            _localBackup = _textBox;
            _isSaved = true;
            UpdateText();
        }

        public void SaveFile() {
            SaveFile(_fileInfo);
        }

        public void Reload() {
            _textBox = _localBackup;
            if (!File.Exists(FileFullName)) {
                if (!IsUntitled) {
                    MessageBox.Show("Путь файла более не существует. Файл остается несохраненным!");
                }
                else {
                    _empty = true;
                }
            }
            else {
                _isSaved = true;
            }
            UpdateText();
        }

        public override bool Equals(object? obj) {
            return (obj is TextPage) && Text == (obj as TextPage)?.Text;
        }

        private void TextBoxChangeEventHandler(object sender, EventArgs e) {
            _empty = false;
            _isSaved = false;
            UpdateText();
        }

        private void UpdateText() {
            if (Empty || IsSaved) {
                Text = FileName;
            }
            else {
                Text = $"{FileName}*";
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
