using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpLibrary {
    public static class SyntaxHighlight {
        public static Dictionary<string, Color> SyntaxTokenColors { get; } = new();
        private static Color OtherTokenColor;
        private static MefHostServices host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
        private static AdhocWorkspace workspace = new(host);

        static SyntaxHighlight() {
            // Подсветка ключевых слов.
            SyntaxTokenColors["keyword"] = Color.FromArgb(86, 156, 214);
            // Подсветка названий при объявлении типов.
            SyntaxTokenColors["class name"] = Color.FromArgb(164, 0, 182);
            SyntaxTokenColors["record name"] = Color.FromArgb(164, 0, 182);
            SyntaxTokenColors["struct name"] = Color.FromArgb(164, 0, 182);
            SyntaxTokenColors["type identifier"] = Color.FromArgb(164, 0, 182);
            // Enum
            SyntaxTokenColors["enum name"] = Color.FromArgb(184, 214, 157);
            // Подсветка названий при объявлении методов.
            SyntaxTokenColors["method name"] = Color.FromArgb(75, 171, 111);
            SyntaxTokenColors["method identifier"] = Color.FromArgb(75, 171, 111);
            // Подсветка названий при объявлении локальных переменных и параметров.
            SyntaxTokenColors["local name"] = Color.FromArgb(75, 171, 111);
            SyntaxTokenColors["parameter name"] = Color.FromArgb(75, 171, 111);
            // Подсветка строк.
            SyntaxTokenColors["string"] = Color.FromArgb(214, 157, 133);
            // Подсветка комментариев.
            SyntaxTokenColors["comment"] = Color.FromArgb(87, 166, 74);
            // Подсветка других токенов.
            SyntaxTokenColors["other"] = Color.FromArgb(255, 255, 255);
        }

        public struct SimpleSyntaxToken {
            public int Start { get; private set; }
            public int Length { get; private set; }
            public string Classification { get; private set; }
            public string Content { get; private set; }
            public Color Color {
                get {
                    if (SyntaxTokenColors.ContainsKey(Classification))
                        return SyntaxTokenColors[Classification];
                    return SyntaxTokenColors["other"];
                }
            }

            public SimpleSyntaxToken(int start, int length, string classification, string content) {
                Start = start;
                Length = length;
                Classification = classification;
                Content = content;
            }
        }

        public async static Task<string> FormatCode(string code) {
            SourceText sourceText = SourceText.From(code);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
            return root.NormalizeWhitespace().ToFullString();
        }

        public async static Task<List<SimpleSyntaxToken>> GetSyntaxTokens(string code) {
            SourceText sourceText = SourceText.From(code);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
            CSharpCompilation compilation = CSharpCompilation.Create("Dummy").AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)).AddSyntaxTrees(syntaxTree);
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

            List<ClassifiedSpan> classifiedSpans = Classifier.GetClassifiedSpans(semanticModel, new TextSpan(0, code.Length), workspace).ToList();
            var result = new List<SimpleSyntaxToken>();
            classifiedSpans = classifiedSpans.Where(s => !ClassificationTypeNames.AdditiveTypeNames.Contains(s.ClassificationType)).ToList();
            for (var i = 0; i < classifiedSpans.Count; i++) {
                var currentSpan = classifiedSpans[i];
                var previousSpan = classifiedSpans[Math.Max(0, i - 1)];
                var nextSpan = classifiedSpans[Math.Min(i + 1, classifiedSpans.Count - 1)];

                string classifiedSpanType = currentSpan.ClassificationType;
                if (classifiedSpanType == "identifier" &&
                    sourceText.ToString(nextSpan.TextSpan) == "(" &&
                    sourceText.ToString(previousSpan.TextSpan) != "new") {

                    classifiedSpanType = "method identifier";
                }
                else if (classifiedSpanType == "identifier") {
                    classifiedSpanType = "type identifier";
                }

                TextSpan position = currentSpan.TextSpan;
                result.Add(new SimpleSyntaxToken(
                    int.Parse(position.Start.ToString()),
                    int.Parse(position.End.ToString()) - int.Parse(position.Start.ToString()) + 1,
                    classifiedSpanType,
                    sourceText.ToString(position)
                ));
            }
            return result;
        }
    }
}
