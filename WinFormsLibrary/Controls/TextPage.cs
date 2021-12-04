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

        private RichTextBox _localBackup;

        protected bool isSaved;
        protected bool empty;
        protected string untitled;
        protected FileInfo? fileInfo;
        protected RichTextBox textBox;

        public string FileName => !IsUntitled ? fileInfo.Name : untitled;
        public string FileFullName => !IsUntitled ? fileInfo.FullName : "";

        public bool IsUntitled => fileInfo == null;
        public bool IsSaved => isSaved;
        public bool Empty => empty;

        public RichTextBox TextBox {
            get => textBox;
            set => textBox = value;
        }
        public static ColorStyle TextBoxDefaultColor { get; set; }
        public static ContextMenuStrip ContextMenuStripForTextBoxes { get; set; }

        protected TextPage(FileInfo? file = null) : base() {
            fileInfo = file;

            textBox = new RichTextBox() {
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
                InitializeTextBox();
                isSaved = true;
            }
            else {
                untitled = NamingManager.GetNewUntitled();
                isSaved = false;
            }


            empty = IsUntitled;
            textBox.TextChanged += TextBoxChangeEventHandler;
            Controls.Add(textBox);
            ColorStyle.ChangeColorScheme(TextBoxDefaultColor, this);
            UpdateText();
        }

        protected TextPage(FileInfo? bufferedFile, string outputFile, string text, bool isSaved, bool empty) : this(bufferedFile) {
            fileInfo = outputFile == "" ? null : new FileInfo(outputFile);
            if (IsUntitled)
                untitled = text;
            this.isSaved = isSaved;
            this.empty = empty;
            UpdateText();
        }

        protected virtual void InitializeTextBox() {
            textBox.Text = File.ReadAllText(fileInfo.FullName);
        }

        public TextPage SaveFile(FileInfo file) {
            if (file.Extension == ".rtf" && fileInfo.Extension == ".rtf") {
                textBox.SaveFile(file.FullName);
            }
            else {
                File.WriteAllText(file.FullName, textBox.Text);
            }
            fileInfo = file;
            _localBackup = textBox;
            isSaved = true;
            UpdateText();
            return file.Extension switch {
                ".rtf" => new RTFTextPage(fileInfo),
                ".cs" => new CSharpTextPage(fileInfo),
                _ => new AnyFileTextPage(fileInfo)
            };
        }

        public TextPage SaveFile() {
            return SaveFile(fileInfo);
        }

        public void Reload() {
            textBox = _localBackup;
            if (!File.Exists(FileFullName)) {
                if (!IsUntitled) {
                    MessageBox.Show("Путь файла более не существует. Файл остается несохраненным!");
                }
                else {
                    empty = true;
                }
            }
            else {
                isSaved = true;
            }
            UpdateText();
        }

        public override bool Equals(object? obj) {
            return (obj is TextPage) && Text == (obj as TextPage)?.Text;
        }

        protected virtual void TextBoxChangeEventHandler(object sender, EventArgs e) {
            empty = false;
            isSaved = false;
            UpdateText();
        }

        protected void UpdateText() {
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
