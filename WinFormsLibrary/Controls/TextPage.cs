using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpLibrary;
using WinFormsLibrary.Tools;

namespace WinFormsLibrary.Controls {
    public class TextPage : TabPage {

        private RichTextBox _localBackup;

        private bool _isSaved;
        private bool _empty;
        private string _untitled;
        private FileInfo? _fileInfo;
        private RichTextBox _textBox;
        private static Font s_textBoxDefaultFont;

        public bool IsUntitled => _fileInfo == null;
        public bool IsSaved => _isSaved;
        public bool Empty => _empty;

        public string FileName => !IsUntitled ? _fileInfo.Name : _untitled;
        public string FileFullName => !IsUntitled ? _fileInfo.FullName : "";
        public string FileExtension => !IsUntitled ? _fileInfo.Extension : ".txt";

        public RichTextBox TextBox {
            get => _textBox;
            set => _textBox = value;
        }
        public static ColorStyle TextBoxDefaultColor {
            get; set;
        }

        public static Font TextBoxDefaultFont {
            get => s_textBoxDefaultFont;
            set => s_textBoxDefaultFont = value;
        }

        public static ContextMenuStrip ContextMenuStripForTextBoxes {
            get; set;
        }

        public static RichTextBox CreateTextBox() {
            return new RichTextBox() {
                Dock = DockStyle.Fill,
                Multiline = true,
                TabIndex = 0,
                TabStop = false,
                ContextMenuStrip = ContextMenuStripForTextBoxes,
            };
        }

        public TextPage(FileInfo? file = null) : base() {
            _fileInfo = file;

            _textBox = CreateTextBox();

            _localBackup = CreateTextBox();

            _empty = IsUntitled;
            _textBox.TextChanged += TextBoxChangeEventHandler;
            Controls.Add(_textBox);

            if (!IsUntitled) {
                if (FileExtension == ".rtf") {
                    _textBox.LoadFile(FileFullName);
                }
                else {
                    _textBox.Text = File.ReadAllText(FileFullName);
                }
                _isSaved = true;
            }
            else {
                _untitled = NamingManager.GetNewUntitled();
                _isSaved = false;
            }
            if (FileExtension == ".rtf") {
                ColorStyle.ChangeColorScheme(new ColorStyle("Window", "Black"), this);
            }
            else if (FileExtension != ".rtf") {
                ColorStyle.ChangeColorScheme(TextBoxDefaultColor, this);
                TextBox.Font = TextBoxDefaultFont;
            }
            UpdatePage();
        }

        public TextPage(FileInfo? bufferedFile, string outputFile, string text, bool isSaved, bool empty) : this(bufferedFile) {
            _fileInfo = outputFile == "" ? null : new FileInfo(outputFile);
            if (IsUntitled)
                _untitled = text;
            _isSaved = isSaved;
            _empty = empty;
            UpdatePage();
        }


        public void SaveFile(FileInfo file, bool pageChanging = true) {
            if (FileExtension == ".rtf" && FileExtension == ".rtf") {
                _textBox.SaveFile(file.FullName);
            }
            else {
                File.Create(file.FullName).Close();
                File.WriteAllText(file.FullName, _textBox.Text);
            }
            if (pageChanging) {
                _fileInfo = file;
                _localBackup = _textBox;
                _isSaved = true;
                UpdatePage();
            }
        }

        public void SaveFile(bool pageChanging = true) {
            SaveFile(_fileInfo, pageChanging);
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
            UpdatePage();
        }

        public override bool Equals(object? obj) {
            return (obj is TextPage) && Text == (obj as TextPage)?.Text;
        }

        private void TextBoxChangeEventHandler(object sender, EventArgs e) {
            _empty = false;
            _isSaved = false;
            UpdatePage();
        }

        private void UpdatePage() {
            if (Text is not null && Text != "") {
                string currentExtension = Path.GetExtension(Text);
                if (Text[^1] == '*')
                    currentExtension = Path.GetExtension(Text[..^1]);

                if (FileExtension == ".rtf" && currentExtension != ".rtf") {
                    TextBox.Font = new Font(new FontFamily("Calibri"), 12, FontStyle.Regular);
                    ColorStyle.ChangeColorScheme(new ColorStyle("Window", "Black"), this);
                }
                else if (FileExtension != ".rtf" && currentExtension == ".rtf") {
                    TextBox.Font = TextBoxDefaultFont;
                    ColorStyle.ChangeColorScheme(TextBoxDefaultColor, this);
                }
            }

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
