using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CSharpLibrary;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    /// <summary>
    /// Форма выбора цветов интерфейса и подсветки кода.
    /// </summary>
    public partial class StyleForm : Form {

        /// <summary>
        /// Диалог выбора цвета.
        /// </summary>
        private static ColorDialog s_colorDialog = new ColorDialog();
        /// <summary>
        /// Диалог выбора основного шрифта.
        /// </summary>
        private static FontDialog s_fontDialog = new FontDialog();
        /// <summary>
        /// Выбранная цветовая расцветка.
        /// </summary>
        private ColorStyle _colorTheme = ConfigurationSetter.ColorTheme;

        /// <summary>
        /// Возвращаемая цветовая расцаетка.
        /// </summary>
        public ColorStyle ColorStyleCallback { get; private set; }
        /// <summary>
        /// Возвращаемый шрифт.
        /// </summary>
        public Font FontCallback { get; private set; }

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public StyleForm() {
            InitializeComponent();
            FontCallback = ConfigurationSetter.MainFont;
            ColorStyleCallback = ConfigurationSetter.ColorTheme;
        }

        /// <summary>
        /// Установка тёмной темы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            ColorStyleCallback = new ColorStyle((31, 31, 31), (240, 240, 240));
            Close();
        }
        /// <summary>
        /// Установка светлой темы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button2_Click(object sender, EventArgs e) {
            ColorStyleCallback = new ColorStyle("Control", "Black");
            Close();
        }

        /// <summary>
        /// Изменение цвета кнопки.
        /// </summary>
        /// <param name="button">Кнопка.</param>
        /// <param name="backColor">Выбранный цвет.</param>
        private void ChangeButtonColor(Button button, Color backColor) {
            button.BackColor = backColor;
            button.ForeColor = button.BackColor.R + button.BackColor.G + button.BackColor.B < 382 ? Color.White : Color.Black; 
        }

        /// <summary>
        /// Инициализация формы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void StyleForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
            button3.BackColor = button4.ForeColor = _colorTheme.MainBodyBackcolor;
            button4.ForeColor = button4.BackColor = _colorTheme.MainBodyForecolor;
            foreach (Control control in Controls) {
                if (control is Button && control.Tag != null) {
                    string firstTag = ((Button)control).Tag.ToString().Split('|').First();
                    ChangeButtonColor((Button)control, SyntaxHighlight.SyntaxTokenColors[firstTag]);
                }
            }
        }

        /// <summary>
        /// Изменение цвета синтаксиса.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button_Click(object sender, EventArgs e) {
            if (s_colorDialog.ShowDialog() != DialogResult.OK)
                return;
            foreach (var tag in ((Button)sender).Tag.ToString().Split('|')) {
                SyntaxHighlight.SyntaxTokenColors[tag] = s_colorDialog.Color;
                ChangeButtonColor((Button)sender, s_colorDialog.Color);
            }
        }

        /// <summary>
        /// Изменение цветов синтаксиса.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button3_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            _colorTheme.MainBodyBackcolor = s_colorDialog.Color;
            ChangeButtonColor((Button)sender, s_colorDialog.Color);
        }
        /// <summary>
        /// Изменение второстепенного цвета.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button4_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            _colorTheme.MainBodyForecolor = s_colorDialog.Color;
            ChangeButtonColor((Button)sender, s_colorDialog.Color);
        }
        /// <summary>
        /// Изменение основного шрифта редактора.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button13_Click(object sender, EventArgs e) {
            s_fontDialog.ShowColor = false;
            s_fontDialog.ShowDialog();
            FontCallback = s_fontDialog.Font;
        }
    }
}
