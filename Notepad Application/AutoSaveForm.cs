using System;
using System.Windows.Forms;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    /// <summary>
    /// Форма настройки автосохранения.
    /// </summary>
    public partial class AutoSaveForm : Form {
        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public AutoSaveForm() {
            InitializeComponent();
            textBox1.Text = ConfigurationSetter.AutoSaveFrequency.ToString();
            checkBox1.Checked = ConfigurationSetter.AutoSaveEnabled;
            textBox2.Text = ConfigurationSetter.BackupSaveFrequency.ToString();
            checkBox2.Checked = ConfigurationSetter.BackupSaveEnabled;
            textBox1.Enabled = checkBox1.Checked;
        }

        /// <summary>
        /// Включение/выключение автосохранений.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            ConfigurationSetter.AutoSaveEnabled = checkBox1.Checked;
            textBox1.Enabled = checkBox1.Checked;
        }
        
        /// <summary>
        /// Включение/выключение автосохранений.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            try {
                if (ConfigurationSetter.AutoSaveEnabled)
                    ConfigurationSetter.AutoSaveFrequency = textBox1.Text;
                if (ConfigurationSetter.BackupSaveEnabled)
                    ConfigurationSetter.BackupSaveFrequency = textBox2.Text;
                Close();
            }
            catch (ArgumentException ex) {
                MessageTools.ShowException("Некорректное значение аргумента.", ex);
                textBox1.Undo();
                textBox2.Undo();
                if (ConfigurationSetter.AutoSaveEnabled)
                    ConfigurationSetter.AutoSaveFrequency = textBox1.Text;
                if (ConfigurationSetter.BackupSaveEnabled)
                    ConfigurationSetter.BackupSaveFrequency = textBox2.Text;
            }
        }

        /// <summary>
        /// Включение/выключение автосохранений.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            ConfigurationSetter.BackupSaveEnabled = checkBox2.Checked;
            textBox2.Enabled = checkBox2.Checked;
        }

        /// <summary>
        /// Инициализация формы на экран.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void AutoSaveForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }
    }
}
