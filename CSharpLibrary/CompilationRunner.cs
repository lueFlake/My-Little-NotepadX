using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSharpLibrary {
    public class CompilationRunner {

        private static Process compiler = new();

        public static string Build(string path, string source, string executable) {
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.CreateNoWindow = true;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.RedirectStandardError = true;

            compiler.StartInfo.FileName = path;
            compiler.StartInfo.Arguments = $"{source} /t:exe /out:{executable}";

            try {
                compiler.Start();
            }
            catch {
                return null;
            }

            return compiler.StandardOutput.ReadToEnd();
        }

        public static bool Run(string executable) {
            compiler.StartInfo.FileName = "CMD.EXE";
            compiler.StartInfo.Arguments = $"/K {executable}";
            compiler.StartInfo.RedirectStandardOutput = false;
            compiler.StartInfo.CreateNoWindow = false;


            try {
                compiler.Start();
            }
            catch { 
                return false; 
            }

            return true;
        }

        public static string CheckCompiler(string path) {
            compiler.StartInfo.FileName = path;
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.CreateNoWindow = true;
            try {
                compiler.Start();
            }
            catch {
                return "undefined";
            }
            var output = compiler.StandardOutput.ReadToEnd();
            if (!output.StartsWith("Microsoft (R) Visual C# Compiler"))
                return "undefined";
            return output.Split(" ")[6];
        }

    }
}
