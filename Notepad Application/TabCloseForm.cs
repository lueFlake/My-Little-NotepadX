using System;
using System.Windows.Forms;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    /// <summary>
    /// Форма сохраниения файла перед закрытием вкладки.
    /// </summary>
    public partial class TabCloseForm : Form {
        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public TabCloseForm() {
            InitializeComponent();
        }

        /// <summary>
        /// Согласие на сохранение.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Yes;
            Close();
        }
        /// <summary>
        /// Отказ от сохраниения.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.No;
            Close();
        }
        /// <summary>
        /// Инициализация формы на экран.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void TabCloseForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }
    }
}
