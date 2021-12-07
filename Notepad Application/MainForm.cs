using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormsLibrary.Controls;
using WinFormsLibrary.Tools;
using System.Collections.Generic;
using CSharpLibrary;
using System.Diagnostics;

namespace NotepadApplication {
    /// <summary>
    /// Главная форма.
    /// </summary>
    public partial class MainForm : Form {
        /// <summary>
        /// Ссылка на выбранную вкладку.
        /// </summary>
        private TextPage SelectedPage => textControl.SelectedTab;

        /// <summary>
        /// 
        /// </summary>
        private static readonly AutoSaveForm s_autoSaveForm = new();

        /// <summary>
        /// Форма сохранения вкладки.
        /// </summary>
        private static readonly TabCloseForm s_tabSaveForm = new();

        /// <summary>
        /// Форма для изменения стиля синтаксиса при работе с исходными кодами C# 
        /// и просто изменения стиля формы.
        /// </summary>
        private static readonly StyleForm s_styleForm = new();

        /// <summary>
        /// Форма для изменения настроек компилятора.
        /// </summary>
        private static readonly CompilerForm s_compilerForm = new();

        /// <summary>
        /// Максимальное количество вкладок.
        /// </summary>
        private static readonly int s_maxTabCount = 20;

        /// <summary>
        /// Количество открытых окон главной формы.
        /// </summary>
        private static int s_windowCount = 0;

        /// <summary>
        /// Текцщий стиль формы.
        /// </summary>
        private static ColorStyle s_colorStyle = ConfigurationSetter.ColorTheme;
        
        /// <summary>
        /// Основной шрифт текста.
        /// </summary>
        private static Font s_mainFont = ConfigurationSetter.MainFont;
        
        /// <summary>
        /// Вспомогательный(временный) текстбокс для подсветки синтаксиса.
        /// </summary>
        private RichTextBox _temporaryTextBox;

        /// <summary>
        /// Свойство стиля формы.
        /// </summary>
        public ColorStyle FormStyle {
            get => s_colorStyle;
            set {
                s_colorStyle = value;
                TextPage.TextBoxDefaultColor = value;
                ColorStyle.ChangeColorScheme(s_colorStyle, this);
                ConfigurationSetter.ColorTheme = s_colorStyle;
            }
        }

        /// <summary>
        /// Свойство основного шрифта формы.
        /// </summary>
        public Font FormMainFont {
            get => s_mainFont;
            set {
                s_mainFont = value;
                TextPage.TextBoxDefaultFont = value;
                foreach (var page in textControl.TabPages) {
                    if (page.FileExtension != ".rtf")
                        page.TextBox.Font = s_mainFont;
                }
                _temporaryTextBox.Font = s_mainFont;
                ConfigurationSetter.MainFont = s_mainFont;
            }
        }

        /// <summary>
        /// Конструктор главной формы.
        /// </summary>
        /// <param name="empty">Должна ли форма быть пустой или содержать вкладки из предыдцщего запуска.</param>
        public MainForm(bool empty = false) {
            SyntaxHighlight.SyntaxTokenColors = ConfigurationSetter.SyntaxColors;

            InitializeComponent();
            s_windowCount++;

            _temporaryTextBox = TextPage.CreateTextBox();
            _temporaryTextBox.BackColor = ConfigurationSetter.ColorTheme.MainBodyBackcolor;
            _temporaryTextBox.ForeColor = ConfigurationSetter.ColorTheme.MainBodyForecolor;

            FormStyle = ConfigurationSetter.ColorTheme;
            FormMainFont = ConfigurationSetter.MainFont;
            timer1.Interval = 60000 * (int)ConfigurationSetter.AutoSaveFrequency;
            timer3.Interval = 60000 * (int)ConfigurationSetter.BackupSaveFrequency;

            TextPage.ContextMenuStripForTextBoxes = contextMenuStrip2;
            if (!empty) {
                InitializePrevioslyOpened();
            }
            else {
                textControl.TabPages.Add();
            }
            NamingManager.AllBusyUntitled = ConfigurationSetter.AllBusyUntitled;
            textControl_SelectedIndexChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Инициализация окон из предыдущего запуска.
        /// </summary>
        private void InitializePrevioslyOpened() {
            var previousFiles = ConfigurationSetter.GetPreviouslyOpenedFiles();
            if (previousFiles.Count != 0) {
                foreach (var page in previousFiles) {
                    textControl.TabPages.Add(page);
                }
                if (ConfigurationSetter.SelectedIndex < textControl.TabCount) {
                    textControl.SelectTab(ConfigurationSetter.SelectedIndex);
                }
                else {
                    textControl.SelectTab(0);
                }
            }
            else {
                textControl.TabPages.Add();
            }
        }

        // Меню правки.


        /// <summary>
        /// Выделяет весь текст.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.SelectAll();
        }

        /// <summary>
        /// Копирует выделенный текст в буффер.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Copy();
        }

        /// <summary>
        /// Вырезает выделенный текст в буффер.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Cut();
        }

        /// <summary>
        /// Вставляет буфферизированный текст.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Paste();
        }

        /// <summary>
        /// Отменяет последнее действие.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Undo();
        }

        /// <summary>
        /// Повторяет последнее отмененное действие.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Redo();
        }

        // Конец меню правки.

        // Меню формата.

        /// <summary>
        /// Вызывает меню выбора шрифта для выделенного фрагмента.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void formatFonlToolStripMenuItem_Click(object sender, EventArgs e) {
            fontDialog1.ShowDialog();
            SelectedPage.TextBox.SelectionFont = fontDialog1.Font;
            SelectedPage.TextBox.SelectionColor = fontDialog1.Color;
        }

        /// <summary>
        /// Изменяет стиль шрифта для выделенного фрагмента на жирный/не жирный.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void boldToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Bold);
        }

        /// <summary>
        /// Изменяет стиль шрифта для выделенного фрагмента на курсив/не курсив.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void italicToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Italic);
        }

        /// <summary>
        /// Изменяет стиль шрифта для выделенного фрагмента на подчернутый/не подчеркнутый.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void underlineToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Underline);
        }

        /// <summary>
        /// Изменяет стиль шрифта для выделенного фрагмента на зачернутый/не зачеркнутый.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void crossedoutToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Strikeout);
        }


        /// <summary>
        /// Добавление/удаление стиля выделенного фрагмента.
        /// Добавлеяет, если стиль еще не применен, иначе удаляет.
        /// </summary>
        /// <param name="fontStyle">Добавляемый/удаляемый стиль.</param>
        private void ChangeFontStyle(FontStyle fontStyle) {
            var currentFont = SelectedPage.TextBox.SelectionFont;
            if (currentFont == null) {
                MessageBox.Show("Невозможно применить: разные шрифты.");
                return;
            }
            SelectedPage.TextBox.SelectionFont = new Font(
                currentFont.FontFamily,
                currentFont.Size,
                currentFont.Style.HasFlag(fontStyle) ?
                    currentFont.Style ^ fontStyle :
                    currentFont.Style | fontStyle
            );
        }

        // Конец меню формата.

        // Меню настроек.

        /// <summary>
        /// Вызов формы изменения настроек автосохранения.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void settingsSaveFreqToolStripMenuItem_Click(object sender, EventArgs e) {
            s_autoSaveForm.ShowDialog();
        }

        /// <summary>
        /// Вызов формы изменения настроек компиляции.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void compilerToolStripMenuItem_Click(object sender, EventArgs e) {
            s_compilerForm.ShowDialog();
        }

        /// <summary>
        /// Вызов формы изменения настроек компиляции.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void settingsColorSchemeToolStripMenuItem_Click(object sender, EventArgs e) {
            s_styleForm.ShowDialog();
            FormStyle = s_styleForm.ColorStyleCallback;
            FormMainFont = s_styleForm.FontCallback;
        }

        // Конец меню настроек.

        // Меню файла.

        /// <summary>
        /// Вызов формы изменения настроек компиляции.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog() {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Filter = "Plain text file (*.txt)|*.txt|Rich Text Format File (*.rtf)|*.rtf|C# source file (*.cs)|*.cs|Any file (*.*)|*.*",
                FilterIndex = 2
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            if (File.Exists(dialog.FileName))
                textControl.SelectTab(textControl.TabPages.Add(new FileInfo(dialog.FileName)));
        }

        /// <summary>
        /// Создание нового пустого окна.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void fileNewWindowToolStripMenuItem_Click(object sender, EventArgs e) {
            var form1 = new MainForm(empty: true);
            form1.Show();
            form1.Activate();
        }

        /// <summary>
        /// Создание новой пустой вкладки.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void fileNewPageToolStripMenuItem_Click(object sender, EventArgs e) {
            textControl.SelectTab(textControl.TabPages.Add());
        }

        /// <summary>
        /// Сохранение текущей вкладки.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void fileSaveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (SelectedPage.IsUntitled || !File.Exists(SelectedPage.FileFullName)) {
                fileSaveAsToolStripMenuItem_Click(sender, e);
            }
            else {
                SelectedPage.SaveFile();
            }
        }

        /// <summary>
        /// Создание новой пустой вкладки в заданную директорию.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void fileSaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            FileInfo savePath = MessageTools.ShowSaveFileDialog();
            if (savePath != null)
                SelectedPage.SaveFile(savePath);
        }

        /// <summary>
        /// Сохранение открытых вкладок, имеющих путь.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void fileSaveOpenedToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (var page in textControl.TabPages) {
                if (!page.IsUntitled)
                    page.SaveFile();
            }
        }

        //Конец меню файла.

        /// <summary>
        /// Вызов помощи.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            string pathToDoc = Path.GetFullPath("../../../Resources/My_Little_Notepad_Documentation.pdf");
            var startInfo = new ProcessStartInfo(pathToDoc) {
                Arguments = pathToDoc,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(startInfo);
        }

        /// <summary>
        /// Обработчик закрытия формы и вызов соответствующей формы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            List<TextPage> unsavedTextPages = textControl.TabPages.
                OfType<TextPage>().
                Where(page => !page.IsSaved).
                ToList();
            if (unsavedTextPages.Count > 0) {
                var appCloseForm = new AppCloseForm(unsavedTextPages);
                if (appCloseForm.ShowDialog() == DialogResult.Cancel) {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Обработчик процесса закрытия вкладки и вызов соответствующей формы.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e) {
            if (s_tabSaveForm.ShowDialog() == DialogResult.Yes) {
                FileInfo savePath = MessageTools.ShowSaveFileDialog();
                if (savePath != null)
                    SelectedPage.SaveFile(savePath);
            }
            NamingManager.RemoveUntitled(textControl.SelectedTab.FileName);
            textControl.Controls.RemoveAt(textControl.SelectedIndex);
        }

        /// <summary>
        /// Обработчик открытия контекстного меню вкладки.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            Point p = textControl.PointToClient(Cursor.Position);
            for (var i = 0; i < textControl.TabCount; i++) {
                Rectangle r = textControl.GetTabRect(i);
                if (r.Contains(p)) {
                    textControl.SelectTab(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Обработчик закрытия формы и сохранение текущей конфигурации.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (s_windowCount == 1) {
                ConfigurationSetter.ClearPreviouslyOpened();
                foreach (var page in textControl.TabPages) {
                    if (page != null) {
                        ConfigurationSetter.SavePage(page);
                    }
                }
                ConfigurationSetter.SelectedIndex = textControl.SelectedIndex;
                ConfigurationSetter.AllBusyUntitled = NamingManager.AllBusyUntitled;
                ConfigurationSetter.SyntaxColors = SyntaxHighlight.SyntaxTokenColors;
                ConfigurationSetter.Save();
            }
            else {
                foreach (var page in textControl.TabPages) {
                    if (page != null) {
                        NamingManager.RemoveUntitled(page.FileName);
                    }
                }
            }
            s_windowCount--;
        }

        /// <summary>
        /// Открытие исходного файла вкладки заново.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void undoChangesToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.Reload();
        }

        /// <summary>
        /// Обработчик таймера для автосохранений.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void timer1_Tick(object sender, EventArgs e) {
            if (ConfigurationSetter.AutoSaveEnabled) {
                fileSaveOpenedToolStripMenuItem_Click(sender, EventArgs.Empty);
                timer1.Interval = 60000 * (int)ConfigurationSetter.AutoSaveFrequency;
            }
        }

        /// <summary>
        /// Обработчик таймера для подсветки синтаксиса.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private async void timer2_Tick(object sender, EventArgs e) {
            int temporarySelectionStart = SelectedPage.TextBox.SelectionStart;
            int temporarySelectionLength = SelectedPage.TextBox.SelectionLength;

            if (_temporaryTextBox.Text == SelectedPage.TextBox.Text && !sender.Equals(this)) {
                return;
            }

            _temporaryTextBox.Text = SelectedPage.TextBox.Text;
            List<SyntaxHighlight.SimpleSyntaxToken> syntaxTokens;
            syntaxTokens = await SyntaxHighlight.GetSyntaxTokens(SelectedPage.TextBox.Text);
            foreach (var token in syntaxTokens) {
                _temporaryTextBox.Select(token.Start, token.Length);
                _temporaryTextBox.SelectionColor = token.Color;
            }
            SelectedPage.TextBox.Rtf = _temporaryTextBox.Rtf;

            SelectedPage.TextBox.SelectionStart = temporarySelectionStart;
            SelectedPage.TextBox.SelectionLength = temporarySelectionLength;
        }

        /// <summary>
        /// Обработчик таймера для сохранения откатов.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void timer3_Tick(object sender, EventArgs e) {
            if (ConfigurationSetter.BackupSaveEnabled)
                ConfigurationSetter.CreateBackup(SelectedPage);
        }

        /// <summary>
        /// Изменение вида при обновлении номера текущей вкладки.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void textControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (textControl.TabCount == 0) {
                Close();
                return;
            }

            if (textControl.TabCount >= s_maxTabCount) {
                MessageBox.Show($"Превышено доступное количество вкладок({s_maxTabCount})");
                textControl.TabPages.RemoveAt(textControl.SelectedIndex);
                return;
            }

            timer3.Stop();
            timer3.Start();

            bool enabledCSharpView = SelectedPage.FileExtension == ".cs";
            bool enabledRTFView = SelectedPage.FileExtension == ".rtf";
            bool enabledTextView = SelectedPage.FileExtension == ".txt";

            runCSharpFileButton.Visible = enabledCSharpView;
            buildCSharpFileButton.Visible = enabledCSharpView;
            formatCodeToolStripMenuItem.Enabled = enabledCSharpView;

            boldTextButton.Visible = enabledRTFView;
            italicTextButton.Visible = enabledRTFView;
            underlineTextButton.Visible = enabledRTFView;
            crossoutTextButton.Visible = enabledRTFView;

            contextFontToolStripMenuItem.Enabled = enabledRTFView;
            formatFonlToolStripMenuItem.Enabled = enabledRTFView;


            if (enabledCSharpView) {
                timer2.Start();
            }
            else {
                timer2.Stop();
            }
        }

        /// <summary>
        /// Запустить программу из текущего документа.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void runCSharpFileButton_Click(object sender, EventArgs e) {
            PromptRunner.Run($"{SelectedPage.FileFullName[..^3]}.exe");
        }

        /// <summary>
        /// Собрать программу из текущего документа.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void buildCSharpFileButton_Click(object sender, EventArgs e) {
            panel1.Visible = true;
            if (!SelectedPage.IsSaved)
                fileSaveToolStripMenuItem_Click(sender, e);
            if (PromptRunner.CheckCompiler(ConfigurationSetter.CompilerPath) == "undefined") {
                textBox1.Text = "Невозможно выполнить сборку, так как путь к csc.exe не задан.";
                compilerToolStripMenuItem_Click(sender, e);
            }
            else {
                textBox1.Text = PromptRunner.Build(
                    ConfigurationSetter.CompilerPath,
                    SelectedPage.FileFullName,
                    $"{SelectedPage.FileFullName[..^3]}.exe"
                );
            }
        }

        /// <summary>
        /// Выключение панели вывода компиляции.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void button1_Click(object sender, EventArgs e) {
            panel1.Visible = false;
        }

        /// <summary>
        /// Форматировать код.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void formatCodeToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Text = SyntaxHighlight.FormatCode(SelectedPage.TextBox.Text);
        }

        /// <summary>
        /// Выбор версии документа текущей вкладки.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
        private void chooseVersionToolStripMenuItem_Click(object sender, EventArgs e) {
            var backupLoadForm = new BacupLoadForm(SelectedPage);
            if (backupLoadForm.ShowDialog() == DialogResult.OK) {
                FileInfo backup = backupLoadForm.Callback;
                SelectedPage.TextBox.Text = File.ReadAllText(backup.FullName);
                SelectedPage.SaveFile();
            }
        }
    }
}
