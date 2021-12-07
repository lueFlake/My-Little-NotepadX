using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormsLibrary.Tools;

namespace WinFormsLibrary.Controls {
    /// <summary>
    /// Класс вкладки редактора.
    /// </summary>
    public class TextPage : TabPage {

        /// <summary>
        /// Проверка на сохраненность файла.
        /// </summary>
        private bool _isSaved;
        /// <summary>
        /// Проверка на пустоту файла.
        /// </summary>
        private bool _empty;
        /// <summary>
        /// Имя полностью несохраненного файла.
        /// </summary>
        private string _untitled;
        /// <summary>
        /// Информация сохраненного файла.
        /// </summary>
        private FileInfo? _fileInfo;
        /// <summary>
        /// Поле редактора текта.
        /// </summary>
        private RichTextBox _textBox;
        /// <summary>
        /// Стандартный шрифт редактора.
        /// </summary>
        private static Font s_textBoxDefaultFont;

        /// <summary>
        /// Проверка на полноту сохранения файла.
        /// </summary>
        public bool IsUntitled => _fileInfo == null;
        /// <summary>
        /// Проверка на сохраненность файла.
        /// </summary>
        public bool IsSaved => _isSaved;
        /// <summary>
        /// Проверка на пустоту файла.
        /// </summary>
        public bool Empty => _empty;

        /// <summary>
        /// Название файла.
        /// </summary>
        public string FileName => !IsUntitled ? _fileInfo.Name : _untitled;
        /// <summary>
        /// Полный путь к файлу.
        /// </summary>
        public string FileFullName => !IsUntitled ? _fileInfo.FullName : "";
        /// <summary>
        /// Расширение файла.
        /// </summary>
        public string FileExtension => !IsUntitled ? _fileInfo.Extension : ".txt";
        /// <summary>
        /// Поле редактора текта.
        /// </summary>
        public RichTextBox TextBox {
            get => _textBox;
            set => _textBox = value;
        }
        /// <summary>
        /// Цветовая расцветка поля редактора.
        /// </summary>
        public static ColorStyle TextBoxDefaultColor {
            get; set;
        }
        /// <summary>
        /// Стандартный шрифт поля редактора.
        /// </summary>
        public static Font TextBoxDefaultFont {
            get => s_textBoxDefaultFont;
            set => s_textBoxDefaultFont = value;
        }
        /// <summary>
        /// Контекстное меню редактора.
        /// </summary>
        public static ContextMenuStrip ContextMenuStripForTextBoxes {
            get; set;
        }
        /// <summary>
        /// Создатель поля редактора текста.
        /// </summary>
        public static RichTextBox CreateTextBox() {
            var instance = new RichTextBox() {
                Dock = DockStyle.Fill,
                Multiline = true,
                TabIndex = 0,
                TabStop = false,
                ContextMenuStrip = ContextMenuStripForTextBoxes,
            };
            return instance;
        }
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="file">Информация об импортируемом файле.</param>
        public TextPage(FileInfo? file = null) : base() {
            _fileInfo = file;

            _textBox = CreateTextBox();

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

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="bufferedFile">Информация о сбуфферизированнои файле.</param>
        /// <param name="outputFile">Информация о файле, в котором хранился буфферизированный файл.</param>
        /// <param name="text">Название вкладки.</param>
        /// <param name="isSaved">Атрибут сохраненности файла.</param>
        /// <param name="empty">Атрибут пустоты файла.</param>
        public TextPage(FileInfo? bufferedFile, string outputFile, string text, bool isSaved, bool empty) : this(bufferedFile) {
            _fileInfo = outputFile == "" ? null : new FileInfo(outputFile);
            if (IsUntitled)
                _untitled = text;
            _isSaved = isSaved;
            _empty = empty;
            UpdatePage();
        }


        /// <summary>
        /// Сохранение файла по новому пути.
        /// </summary>
        /// <param name="file">Информация о файле.</param>
        /// <param name="pageChanging">Атрибут для вывода файла в новой вкладке.</param>
        public void SaveFile(FileInfo file, bool pageChanging = true) {
            if (IsUntitled && pageChanging) {
                NamingManager.RemoveUntitled(_untitled);
            } 

            if (FileExtension == ".rtf" && FileExtension == ".rtf") {
                _textBox.SaveFile(file.FullName);
            }
            else {
                File.Create(file.FullName).Close();
                File.WriteAllText(file.FullName, _textBox.Text);
            }
            if (pageChanging) {
                _fileInfo = file;
                _isSaved = true;
                UpdatePage();
            }
        }

        /// <summary>
        /// Сохранение файла в ранее созданный для этого файл.
        /// </summary>
        /// <param name="pageChanging">Атрибут для вывода файла в новой вкладке.</param>
        public void SaveFile(bool pageChanging = true) {
            SaveFile(_fileInfo, pageChanging);
        }

        /// <summary>
        /// Возвращение файла к сохраненной версии.
        /// </summary>
        public void Reload() {
            if (!File.Exists(FileFullName)) {
                if (!IsUntitled) {
                    MessageBox.Show("Путь файла более не существует. Файл остается несохраненным!");
                }
                else {
                    _empty = true;
                }
            }
            else {
                if (FileExtension == ".rtf") {
                    _textBox.LoadFile(FileFullName);
                }
                else {
                    _textBox.Text = File.ReadAllText(FileFullName);
                }
                _isSaved = true;
            }
            UpdatePage();
        }

        /// <summary>
        /// Проверка на совпадение объекта и текущего экземпляра класса.
        /// </summary>
        /// <param name="obj">Сравниваемый объект.</param>
        /// <returns>Результат сравнения: true, если объекты равны.</returns>
        public override bool Equals(object? obj) {
            return (obj is TextPage) && Text == (obj as TextPage)?.Text;
        }

        /// <summary>
        /// Обработчик изменения текста.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void TextBoxChangeEventHandler(object sender, EventArgs e) {
            _empty = false;
            _isSaved = false;
            UpdatePage();
        }

        /// <summary>
        /// Обновление текущего экземпляра.
        /// </summary>
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

        /// <summary>
        /// Нахождение hash-функции от объекта.
        /// </summary>
        /// <returns>Результат hash-функции в виде целого числа.</returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
