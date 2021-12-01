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

namespace NotepadApplication {
    public partial class MainForm : Form {
        private TextPage _tabPage => textControl.SelectedTab;
        private AutoSaveForm _autoSaveForm = new AutoSaveForm();
        private TabCloseForm _tabSaveForm = new TabCloseForm();
        private StyleForm _styleForm = new StyleForm();
        private static int s_windowCount = 0;
        private static ColorStyle _colorStyle = ConfigurationSetter.ColorTheme;

        public ColorStyle FormStyle {
            get { return _colorStyle; }
            set {
                _colorStyle = value;
                ColorStyle.ChangeColorScheme(_colorStyle, this);
                TextPage.TextBoxDefaultColor = value;
                ConfigurationSetter.ColorTheme = _colorStyle;

                Graphics g = this.CreateGraphics();
                using (Pen selPen = new Pen(Color.Blue)) {
                    g.DrawRectangle(selPen, 10, 10, 50, 50);
                }
            }
        }

        public MainForm(bool empty = false) {
            s_windowCount++;
            InitializeComponent();
            timer1.Interval = 60000 * (int)ConfigurationSetter.AutoSaveFrequency;
            TextPage.ContextMenuStripForTextBoxes = contextMenuStrip2;
            FormStyle = ConfigurationSetter.ColorTheme;
            if (!empty) {
                InitializePrevioslyOpened();
            }
            else {
                textControl.TabPages.Add();
            }
            //backgroundWorker1.RunWorkerAsync();
            NamingManager.AllBusyUntitled = ConfigurationSetter.AllBusyUntitled;
        }

        private void InitializePrevioslyOpened() {
            var previousFiles = ConfigurationSetter.GetPreviouslyOpenedFiles();
            if (previousFiles.Count() != 0) {
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
            _tabPage.TextBox.SelectAll();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            _tabPage.TextBox.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            _tabPage.TextBox.Cut();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            _tabPage.TextBox.Paste();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            _tabPage.TextBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            _tabPage.TextBox.Redo();
        }

        // Конец меню правки.

        // Меню формата и панель изменения шрифта.

        private void formatFonlToolStripMenuItem_Click(object sender, EventArgs e) {
            fontDialog1.ShowDialog();
            _tabPage.TextBox.SelectionFont = fontDialog1.Font;
            _tabPage.TextBox.SelectionColor = fontDialog1.Color;
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
            var currentFont = _tabPage.TextBox.SelectionFont;
            if (currentFont == null) {
                MessageBox.Show("Невозможно применить: разные шрифты.");
                return;
            }
            _tabPage.TextBox.SelectionFont = new Font(
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
            _autoSaveForm.ShowDialog();
        }

        // Меню выбора темы.

        private void settingsColorSchemeToolStripMenuItem_Click(object sender, EventArgs e) {
            _styleForm.ShowDialog();
            FormStyle = _styleForm.Callback;
        }
        
        // Конец меню ывбора темы.

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
            if (_tabPage.IsUntitled || !File.Exists(_tabPage.FileFullName)) {
                fileSaveAsToolStripMenuItem_Click(sender, e);
            }
            else {
                _tabPage.SaveFile();
            }
        }

        private void fileSaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            FileInfo savePath = MessageTools.ShowFileDialog();
            if (savePath != null)
                _tabPage.SaveFile(savePath);
        }

        private void fileSaveOpenedToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (TextPage page in textControl.TabPages) {
                if (!page.IsUntitled)
                    page.SaveFile();
            }
        }

        //Конец меню файла.

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        /*private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            System.Threading.Thread.Sleep(60000 * (int)ConfigurationSetter.AutoSaveFrequency);
            timer1.
        }*/

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            List<TextPage> unsavedTextPages = textControl.TabPages.
                OfType<TextPage>().
                Where(page => !page.IsSaved).
                ToList();
            if (unsavedTextPages.Count > 0) {
                AppCloseForm appCloseForm = new AppCloseForm(unsavedTextPages);
                if (appCloseForm.ShowDialog() == DialogResult.Cancel) {
                    e.Cancel = true;
                }
            }
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_tabSaveForm.ShowDialog() == DialogResult.Yes) {
                FileInfo savePath = MessageTools.ShowFileDialog();
                if (savePath != null)
                    _tabPage.SaveFile(savePath);
            }
            NamingManager.RemoveUntitled(textControl.SelectedTab.FileName);
            textControl.Controls.RemoveAt(textControl.SelectedIndex);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            Point p = textControl.PointToClient(Cursor.Position);
            for (int i = 0; i < textControl.TabCount; i++) {
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

        private void timer1_Tick(object sender, EventArgs e) {
            fileSaveOpenedToolStripMenuItem_Click(sender, EventArgs.Empty);
            SimpleLogsProvider.WriteLine("Saved.");
            timer1.Interval = 60000 * (int)ConfigurationSetter.AutoSaveFrequency;
        }
    }
}
