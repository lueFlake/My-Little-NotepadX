using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinFormsLibrary.Controls;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    /// <summary>
    /// Форма выбора отката.
    /// </summary>
    public partial class BacupLoadForm : Form {
        /// <summary>
        /// Список файлов откатов.
        /// </summary>
        private List<FileInfo> _backups;

        /// <summary>
        /// Возвращаемый формой откат.
        /// </summary>
        public FileInfo Callback { get; set; } 

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        /// <param name="textPage">Выбранная вкладка для составления списка откатов.</param>
        public BacupLoadForm(TextPage textPage) {
            InitializeComponent();
            string usedDirectory = textPage.FileFullName.GetHashCode().ToString();
            _backups = Directory.GetFiles($"../../../Backups/{usedDirectory}").
                Select(x => new FileInfo(x)).
                ToList();
            foreach (var backup in _backups)
                listBox1.Items.Add(backup.Name);
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Инициализация формы на экран.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void BacupLoadForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }

        /// <summary>
        /// Выбор отката.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            int index = listBox1.SelectedIndex;

            if (index == -1) {
                MessageBox.Show("Не выбран документ.");
                return;
            }

            DialogResult = DialogResult.OK;
            Callback = _backups[index];
        }

        /// <summary>
        /// Удаление отката.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e) {
            int index = listBox1.SelectedIndex;

            if (index == -1) {
                MessageBox.Show("Не выбран документ.");
                return;
            }
            
            _backups[index].Delete();
            listBox1.Items.RemoveAt(index);
        }

        /// <summary>
        /// Удаление всех откатов.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e) {
            foreach (var backup in _backups)
                backup.Delete();
            listBox1.Items.Clear();
        }
    }
}
