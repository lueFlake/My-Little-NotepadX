using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsLibrary.Tools;

namespace WinFormsLibrary.Controls {
    public class AnyFileTextPage : TextPage {
        public AnyFileTextPage(FileInfo fileInfo) : base(fileInfo) { }

        public AnyFileTextPage(FileInfo bufferedFile, string outputFile, string text, bool isSaved, bool empty) :
            base(bufferedFile, outputFile, text, isSaved, empty) {
        }
    }
}
