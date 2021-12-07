using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WinFormsLibrary.Controls;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    /// <summary>
    /// Форма, вызываемая при закрытии текущей главной формы.
    /// </summary>
    public partial class AppCloseForm : Form {
        /// <summary>
        /// Список несохраненных страниц.
        /// </summary>
        private List<TextPage> _textPages;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        /// <param name="textPages">Документы, требующие сохранения.</param>
        public AppCloseForm(List<TextPage> textPages) {
            InitializeComponent();
            _textPages = textPages;
            foreach (var page in textPages)
                listBox1.Items.Add(page.FileName);
        }

        /// <summary>
        /// Отмена закрытия формы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Выбор несохранения оставшихся открытых документов.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Ignore;
            Close();
        }

        /// <summary>
        /// Сохранение выбранной формы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            int index = listBox1.SelectedIndex;

            if (index == -1) {
                MessageBox.Show("Не выбран документ.");
                return;
            }

            FileInfo savePath;
            if (!File.Exists(_textPages[index].FileFullName)) {
                savePath = MessageTools.ShowSaveFileDialog();
            }
            else {
                savePath = new FileInfo(_textPages[index].FileFullName);
            }

            if (savePath != null) {
                _textPages[index].SaveFile(savePath);
                _textPages.RemoveAt(index);
                listBox1.Items.RemoveAt(index);
            }
            if (listBox1.Items.Count == 0) {
                DialogResult = DialogResult.Ignore;
                Close();
            }
        }

        /// <summary>
        /// Инициализация формы на экран.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void AppCloseForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }
    }
}
