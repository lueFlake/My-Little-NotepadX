﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    public partial class AutoSaveForm : Form {
        
        public AutoSaveForm() {
            InitializeComponent();
            textBox1.Text = ConfigurationSetter.AutoSaveFrequency.ToString();
            checkBox1.Checked = ConfigurationSetter.AutoSaveEnabled;
            textBox2.Text = ConfigurationSetter.BackupSaveFrequency.ToString();
            checkBox2.Checked = ConfigurationSetter.BackupSaveEnabled;
            textBox1.Enabled = checkBox1.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            ConfigurationSetter.AutoSaveEnabled = checkBox1.Checked;
            textBox1.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                if (ConfigurationSetter.AutoSaveEnabled)
                    ConfigurationSetter.AutoSaveFrequency = textBox1.Text;
                if (ConfigurationSetter.BackupSaveEnabled)
                    ConfigurationSetter.BackupSaveFrequency = textBox2.Text;
                ConfigurationSetter.Save();
                Close();
            }
            catch (ArgumentException ex) {
                MessageTools.ShowException("Некорректное значение аргумента.", ex);
                textBox1.Undo();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            ConfigurationSetter.BackupSaveEnabled = checkBox2.Checked;
            textBox2.Enabled = checkBox2.Checked;
        }
    }
}