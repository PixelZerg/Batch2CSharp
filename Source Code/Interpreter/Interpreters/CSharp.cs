using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using Interpreter.BaseInt;
using Interpreter.BaseInt.CodeTypes.Console;
using Interpreter.BaseInt.CodeTypes.Commands;
using Interpreter.BaseInt.CodeTypes;


namespace Interpreter.Interpreters
{
    public partial class CSharp : UserControl
    {
        Form1 parent;
        public CSharp()
        {
            InitializeComponent();
        }

        #region moo
        private TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        private TextStyle GreennStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);

        private TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);

        private TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);

        private TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);

        private TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);

        private TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);

        private TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);

        private MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        #endregion
        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            #region pretty
            this.fastColoredTextBox1.LeftBracket = '(';
            this.fastColoredTextBox1.RightBracket = ')';
            this.fastColoredTextBox1.LeftBracket2 = '\0';
            this.fastColoredTextBox1.RightBracket2 = '\0';
            e.ChangedRange.ClearStyle(new Style[]
			{
				this.BlueStyle,
				this.BoldStyle,
				this.GrayStyle,
				this.MagentaStyle,
				this.GreenStyle,
				this.BrownStyle
			});
            e.ChangedRange.SetStyle(this.BrownStyle, "\"\"|@\"\"|''|@\".*?\"|(?<!@)(?<range>\".*?[^\\\\]\")|'.*?[^\\\\]'");
            e.ChangedRange.SetStyle(this.GreenStyle, "//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(this.GreenStyle, "(/\\*.*?\\*/)|(/\\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(this.GreenStyle, "(/\\*.*?\\*/)|(.*\\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
            e.ChangedRange.SetStyle(this.MagentaStyle, "\\b\\d+[\\.]?\\d*([eE]\\-?\\d+)?[lLdDfF]?\\b|\\b0x[a-fA-F\\d]+\\b");
            e.ChangedRange.SetStyle(this.GrayStyle, "^\\s*(?<range>\\[.+?\\])\\s*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(this.BoldStyle, "\\b(class|struct|enum|interface)\\s+(?<range>\\w+?)\\b");
            e.ChangedRange.SetStyle(this.BlueStyle, "\\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\\b|#region\\b|#endregion\\b");
            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers("{", "}");
            e.ChangedRange.SetFoldingMarkers("#region\\b", "#endregion\\b");
            e.ChangedRange.SetFoldingMarkers("/\\*", "\\*/");
            #endregion
        }
        public void SetParent(Form1 f)
        {
            parent = f;
        }
        public void Update()
        {
            fastColoredTextBox1.Clear();
            fastColoredTextBox1.Text += "using System;" + Environment.NewLine + Environment.NewLine +
                   "namespace " + parent.basecode.name + Environment.NewLine + "{";
            foreach (var claa in parent.basecode.classes)
            {
                WriteClass(claa);
            }
            fastColoredTextBox1.Text += Environment.NewLine + "}";
        }
        public void WriteClass(Class cla)
        {
            fastColoredTextBox1.Text += Environment.NewLine +"\t"+
                string.Join(" ",cla.Options)+" class " + cla.name + Environment.NewLine + "\t{" + Environment.NewLine;
            foreach (var method in cla.methods)
            {
                WriteMethod(method);
            }
            fastColoredTextBox1.Text += Environment.NewLine + "\t}";
        }
        public void WriteMethod(Method meth)
        {
            fastColoredTextBox1.Text += Environment.NewLine + "\t\t"+meth.visibility.ToString().ToLower() + " " +
                string.Join(" ", meth.options);
            if (meth.returntype == typeof(void))
            {
                fastColoredTextBox1.Text += "void ";
            }
            else
            {
                fastColoredTextBox1.Text += meth.returntype.Name + " ";
            }
            fastColoredTextBox1.Text+= meth.name + "()" + Environment.NewLine + "\t\t{";
            foreach (var loc in meth.code)
            {
                try
                {
                    fastColoredTextBox1.Text += Environment.NewLine+"\t\t\t" + ConvertCode(loc);
                }
                catch (Exception e) { fastColoredTextBox1.Text += "\t\t\t/*Error:" + e.Message + Environment.NewLine + e.InnerException + "*/"; }
            }
            fastColoredTextBox1.Text += Environment.NewLine + "\t\t}";

        }
        public string ConvertCode(Code code)
        {
            Type t = code.code.GetType();
            if (t == typeof(callmethod))
            {
                return code.To<callmethod>().methodname + "();";
            }
            if (t == typeof(Output))
            {
                if (code.To<Output>().outstring.ToString().EndsWith(Environment.NewLine))
                {
                    return @"Console.WriteLine(""" + code.To<Output>().outstring.ToString().Replace(Environment.NewLine, "") + @""");";
                }
                else
                {
                    return @"Console.Write(""" + code.To<Output>().outstring.ToString() + @""");";
                }
            }
            if (t == typeof(comment))
            {
                if (code.To<comment>().multiline)
                {
                    return @"/*" + code.To<comment>().commentedtext + @"*/";
                }
                else
                {
                    return @"//" + code.To<comment>().commentedtext;
                }
            }
            if (t == typeof(DeclareVariable))
            {
                string ret = "";
                DeclareVariable d = code.To<DeclareVariable>();
                if (d.variable.isdeclared)
                {
                    ret += d.variable.type.Name + " ";
                }
                ret += d.variable.name;
                if (!d.setvariabledata)
                {
                    ret += ";";
                    
                }
                else
                {
                    ret += @" = """ + d.variable.data + @""";";
                }
                return ret;
            }
            if (t == typeof(Input))
            {
                if (code.To<Input>().storevar)
                {
                    if (
                    code.To<Input>().var.isdeclared
                        )
                    {
                        return code.To<Input>().var.name + " = Console.ReadLine();";
                    }
                    else
                    {
                        return "//TODO!";
                    }
                }
                else
                {
                    return "Console.ReadLine();";
                }
            }
            if (t == typeof(pause))
            {
                string ret = "";
                ret += @"Console.WriteLine(""Press any key to continue..."");";
                ret+=Environment.NewLine+"\t\t\tConsole.ReadKey();";
                return ret;
            }

            return "//code convert error here, whilst trying to parse code of type: "+t.Name;
        }
    }
}
