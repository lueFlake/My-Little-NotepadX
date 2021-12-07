using System;
using System.IO;
using System.Windows.Forms;
using WinFormsLibrary.Tools;
using CSharpLibrary;

namespace NotepadApplication {
    /// <summary>
    /// Форма настройки компилятора C#.
    /// </summary>
    public partial class CompilerForm : Form {
        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public CompilerForm() {
            InitializeComponent();
            textBox1.Text = ConfigurationSetter.CompilerPath;
            label3.Text = PromptRunner.CheckCompiler(textBox1.Text);
        }

        /// <summary>
        /// Инициализация формы на экран.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void CompilleForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }
        /// <summary>
        /// Открытие диалога выбора файла.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog() {
                InitialDirectory = Path.GetDirectoryName(textBox1.Text),
                Filter = "Windows Executable (*.exe)|*.exe",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            if (File.Exists(dialog.FileName))
                textBox1.Text = dialog.FileName;
        }
        /// <summary>
        /// Подтверждение выбранного компилятора.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e) {
            label3.Text = PromptRunner.CheckCompiler(textBox1.Text);
            if (label3.Text != "undefined") {
                ConfigurationSetter.CompilerPath = textBox1.Text;
                Close();
            }
            else {
                MessageBox.Show("Выбран недействительный путь к csc.exe");
            }
        }
    }
}
