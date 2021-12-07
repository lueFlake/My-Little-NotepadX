using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using WinFormsLibrary;
using System.Text.RegularExpressions;
using WinFormsLibrary.Controls;
using WinFormsLibrary.Tools;
using System.Collections.Generic;
using CSharpLibrary;
using System.Threading.Tasks;

namespace NotepadApplication {
    public partial class MainForm : Form {
        private TextPage SelectedPage => textControl.SelectedTab;

        private static readonly AutoSaveForm s_autoSaveForm = new();
        private static readonly TabCloseForm s_tabSaveForm = new();
        private static readonly StyleForm s_styleForm = new();
        private static readonly CompilerForm s_compilerForm = new();
        private static readonly int s_maxTabCount = 20;
        private static int s_windowCount = 0;
        private static ColorStyle s_colorStyle = ConfigurationSetter.ColorTheme;
        private static Font s_mainFont = ConfigurationSetter.MainFont;
        private RichTextBox _temporaryTextBox;

        public ColorStyle FormStyle {
            get => s_colorStyle;
            set {
                s_colorStyle = value;
                TextPage.TextBoxDefaultColor = value;
                ColorStyle.ChangeColorScheme(s_colorStyle, this);
                ConfigurationSetter.ColorTheme = s_colorStyle;
            }
        }

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

        public MainForm(bool empty = false) {
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
            //backgroundWorker1.RunWorkerAsync();
        }

        private void InitializePrevioslyOpened() {
            var previousFiles = ConfigurationSetter.GetPreviouslyOpenedFiles();
            if (previousFiles.Count != 0) {
                foreach (var page in previousFiles) {
                    textControl.TabPages.Add(page);
                }
                textControl.SelectTab(ConfigurationSetter.SelectedIndex);
            }
            else {
                textControl.TabPages.Add();
            }
        }

        // Меню правки.


        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.SelectAll();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Cut();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Paste();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Redo();
        }

        // Конец меню правки.

        // Меню формата и панель изменения шрифта.

        private void formatFonlToolStripMenuItem_Click(object sender, EventArgs e) {
            fontDialog1.ShowDialog();
            SelectedPage.TextBox.SelectionFont = fontDialog1.Font;
            SelectedPage.TextBox.SelectionColor = fontDialog1.Color;
        }

        private void boldToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Bold);
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Italic);
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Underline);
        }

        private void crossedoutToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeFontStyle(FontStyle.Strikeout);
        }

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

        // Конец меню формата и панели изменения шрифта.

        // Меню настроек.

        private void settingsSaveFreqToolStripMenuItem_Click(object sender, EventArgs e) {
            s_autoSaveForm.ShowDialog();
        }

        private void compilerToolStripMenuItem_Click(object sender, EventArgs e) {
            s_compilerForm.ShowDialog();
        }

        private void settingsColorSchemeToolStripMenuItem_Click(object sender, EventArgs e) {
            s_styleForm.ShowDialog();
            FormStyle = s_styleForm.ColorStyleCallback;
            FormMainFont = s_styleForm.FontCallback;
        }

        // Конец меню настроек.

        // Меню файла.

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

        private void fileNewWindowToolStripMenuItem_Click(object sender, EventArgs e) {
            var form1 = new MainForm(empty: true);
            form1.Show();
            form1.Activate();
        }

        private void fileNewPageToolStripMenuItem_Click(object sender, EventArgs e) {
            textControl.SelectTab(textControl.TabPages.Add());
        }

        private void fileSaveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (SelectedPage.IsUntitled || !File.Exists(SelectedPage.FileFullName)) {
                fileSaveAsToolStripMenuItem_Click(sender, e);
            }
            else {
                SelectedPage.SaveFile();
            }
        }

        private void fileSaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            FileInfo savePath = MessageTools.ShowFileDialog();
            if (savePath != null)
                SelectedPage.SaveFile(savePath);
        }

        private void fileSaveOpenedToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (var page in textControl.TabPages) {
                if (!page.IsUntitled)
                    page.SaveFile();
            }
        }

        //Конец меню файла.

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {

        }

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

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e) {
            if (s_tabSaveForm.ShowDialog() == DialogResult.Yes) {
                FileInfo savePath = MessageTools.ShowFileDialog();
                if (savePath != null)
                    SelectedPage.SaveFile(savePath);
            }
            NamingManager.RemoveUntitled(textControl.SelectedTab.FileName);
            textControl.Controls.RemoveAt(textControl.SelectedIndex);
        }

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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (s_windowCount == 1) {
                ConfigurationSetter.ClearPreviouslyOpened();
                foreach (var page in textControl.TabPages) {
                    if (page != null) {
                        ConfigurationSetter.SetPage(page);
                    }
                }
                ConfigurationSetter.SelectedIndex = textControl.SelectedIndex;
                ConfigurationSetter.AllBusyUntitled = NamingManager.AllBusyUntitled;
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

        private void undoChangesToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.Reload();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (ConfigurationSetter.AutoSaveEnabled) {
                fileSaveOpenedToolStripMenuItem_Click(sender, EventArgs.Empty);
                SimpleLogsProvider.WriteLine("Saved.");
                timer1.Interval = 60000 * (int)ConfigurationSetter.AutoSaveFrequency;
            }
        }

        private async void timer2_Tick(object sender, EventArgs e) {
            int temporarySelectionStart = SelectedPage.TextBox.SelectionStart;
            int temporarySelectionLength = SelectedPage.TextBox.SelectionLength;

            if (_temporaryTextBox.Text == SelectedPage.TextBox.Text) {
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

        private void timer3_Tick(object sender, EventArgs e) {
            if (ConfigurationSetter.BackupSaveEnabled)
                ConfigurationSetter.CreateBackup(SelectedPage);
        }

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

        private void runCSharpFileButton_Click(object sender, EventArgs e) {
            CompilationRunner.Run($"{SelectedPage.FileFullName[..^3]}.exe");
        }

        private void buildCSharpFileButton_Click(object sender, EventArgs e) {
            panel1.Visible = true;
            if (!SelectedPage.IsSaved)
                fileSaveToolStripMenuItem_Click(sender, e);
            if (CompilationRunner.CheckCompiler(ConfigurationSetter.CompilerPath) == "undefined") {
                textBox1.Text = "Невозможно выполнить сборку, так как путь к csc.exe не задан.";
                compilerToolStripMenuItem_Click(sender, e);
            }
            else {
                textBox1.Text = CompilationRunner.Build(
                    ConfigurationSetter.CompilerPath,
                    SelectedPage.FileFullName,
                    $"{Path.GetFileNameWithoutExtension(SelectedPage.FileFullName)}.exe"
                );
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            panel1.Visible = false;
        }

        private void formatCodeToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectedPage.TextBox.Text = SyntaxHighlight.FormatCode(SelectedPage.TextBox.Text);
        }

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
