using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsLibrary.Tools;

namespace WinFormsLibrary.Controls {
    public class CSharpTextPage : TextPage {
        public CSharpTextPage(FileInfo? fileInfo) : base(fileInfo) { }

        public CSharpTextPage(FileInfo? bufferedFile, string outputFile, string text, bool isSaved, bool empty) :
            base(bufferedFile, outputFile, text, isSaved, empty) { }
    }
}