using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsLibrary.Tools;

namespace WinFormsLibrary.Controls {
    public class RTFTextPage : TextPage {
        public RTFTextPage(FileInfo? fileInfo) : base(fileInfo) { }

        public RTFTextPage(FileInfo? bufferedFile, string outputFile, string text, bool isSaved, bool empty) : 
            base(bufferedFile, outputFile, text, isSaved, empty) { }

        protected override void InitializeTextBox() {
            textBox.LoadFile(fileInfo.FullName); 
        }
    }
}
