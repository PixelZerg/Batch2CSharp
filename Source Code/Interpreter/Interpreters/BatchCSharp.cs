using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;
using Interpreter.BaseInt;
using Interpreter.BaseInt.CodeTypes.Commands;
using Interpreter.BaseInt.CodeTypes.Console;
using Interpreter.BaseInt.CodeTypes;

namespace Interpreter.Interpreters
{
    public partial class BatchCSharp : UserControl
    {
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
        public bool echooff = false;
        private Random rnd = new Random();
        public List<BatchMethod> BatchExec = new List<BatchMethod>();
        public List<BatchMethod> Methods = new List<BatchMethod>();
        public List<CSharpMethod> CSharpMethods = new List<CSharpMethod>();
        public BatchCSharp()
        {
            InitializeComponent();
            // fastColoredTextBox2_TextChanged(null, null);
        }

        private void fastColoredTextBox2_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            this.fastColoredTextBox2.LeftBracket = '(';
            this.fastColoredTextBox2.RightBracket = ')';
            this.fastColoredTextBox2.LeftBracket2 = '\0';
            this.fastColoredTextBox2.RightBracket2 = '\0';
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
        }

        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            #region syntax highlighting
            e.ChangedRange.SetFoldingMarkers(":", ":");
            e.ChangedRange.SetStyle(this.MagentaStyle, @"@|<|>");
            e.ChangedRange.SetStyle(this.BlueStyle, "echo|set|pause|sleep|title|if|goto|color|cls", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(this.BrownStyle, "%([^%]*)%");
            e.ChangedRange.SetStyle(this.GreennStyle, "/p|/a");
            e.ChangedRange.SetStyle(this.RedStyle, "0|1|2|3|4|5|6|7|8|9");
            #endregion
            try
            {
                Console.Clear();
            }
            catch { }
            BatchExec.Clear();
            CSharpMethods.Clear();
            fastColoredTextBox2.Clear();
            Methods.Clear();
            echooff = false;
            try
            {
                //  new System.Threading.Thread(()=>
                Parse(fastColoredTextBox1.Lines);
                // );
            }
            catch (Exception moo)
            {
                fastColoredTextBox2.Text = "/*" + Environment.NewLine + "Parse error: " + moo.Message + Environment.NewLine + moo.StackTrace + Environment.NewLine + Environment.NewLine
                    + moo.InnerException + Environment.NewLine + Environment.NewLine + "Possible Causes: " + Fixes(moo) + Environment.NewLine + "*/";
            }
        }
        public string Fixes(Exception e)
        {
            string ret = " the batch file itself doesn't compile";
            if (e.Message.Contains("Index was out of range. Must be non-negative and less than the size of the collection."))
            {
                return " your function in the batch file was only 1 line long" + Environment.NewLine +
                    "your function in the batch file had no code inside it";
            }
            if (e.Message.Contains("The given key was not present in the dictionary"))
            {
                return " you are changing into a non-existent color";
            }
            return ret;
        }
        private void Parse(IList<string> rawloc)
        {
            //Console.Write("parsing...");
            GetMethods(rawloc);
            foreach (var item in Methods)
            {
                //   Console.WriteLine(item);
                ConvertMethod(item);
            }
            WriteMethods();
        }
        public void WriteMethods()
        {
            string ret = "";
            ret += "using System;" + Environment.NewLine+Environment.NewLine;
            ret += "public class Program" + Environment.NewLine + "{";

            foreach (var method in CSharpMethods)
            {
                ret += Environment.NewLine + "\tpublic static void " + method.MethodName + "()" + Environment.NewLine + "\t{";
                foreach (var loc in method.code)
                {
                    ret += Environment.NewLine +"\t\t"+ loc;
                }
                ret += Environment.NewLine + "\t}";
            }

            ret += Environment.NewLine + "}";
            fastColoredTextBox2.Text = ret;
        }
        public void ConvertMethod(BatchMethod bm)
        {
            CSharpMethod csm = new CSharpMethod(bm.MethodName);
            if (csm.MethodName == "Main")
            {
                csm.code.Add("" + "Console.Clear(); //required line of code");
            }
            foreach (var loc in bm.code)
            {
                
                if (!echooff && !loc.StartsWith("@"))
                {

                    csm.code.Add("" + "Console.Write(Environment.NewLine);" + Environment.NewLine + "\t\t" + "Console.WriteLine(System.Environment.CurrentDirectory+\">" + loc + "\"); //echo is on so printing command");
                }
                csm.code.Add(ConvertLoc(loc));
            }
            bool hasgoto = false;
            foreach (var loc in bm.code)
            {
                if (loc.ToLower().StartsWith("goto"))
                {
                    hasgoto = true;
                    continue;
                }
            }
            if (!hasgoto)
            {
                try
                {
                    BatchMethod nextmethod = Methods[Methods.IndexOf(bm) + 1];
                    csm.code.Add("\t"+nextmethod.MethodName + "(); //proceed to next method");
                }
                catch { csm.code.Add("" + "Environment.Exit(0); //end of program"); }
            }
            CSharpMethods.Add(csm);
        }
        public string ParseColour(string raw)
        {
            string ret = "";
            Dictionary<string, string> colours = new Dictionary<string, string>();
            colours.Add("0", "ConsoleColor.Black");
            colours.Add("1", "ConsoleColor.DarkBlue");
            colours.Add("2", "ConsoleColor.DarkGreen");
            colours.Add("3", "ConsoleColor.DarkCyan");
            colours.Add("4", "ConsoleColor.DarkRed");
            colours.Add("5", "ConsoleColor.DarkMagenta");
            colours.Add("6", "ConsoleColor.DarkYellow");
            colours.Add("7", "ConsoleColor.Gray");
            colours.Add("8", "ConsoleColor.Gray");
            colours.Add("9", "ConsoleColor.Blue");
            colours.Add("A", "ConsoleColor.Green");
            colours.Add("B", "ConsoleColor.Cyan");
            colours.Add("C", "ConsoleColor.Red");
            colours.Add("D", "ConsoleColor.Magenta");
            colours.Add("E", "ConsoleColor.Yellow");
            colours.Add("F", "ConsoleColor.White");
            ret = "Console.BackgroundColor = " + colours[raw[0].ToString()]+";";
            string no = RandomString(21);
            string curt = RandomString(22);
            string curl = RandomString(23);
            string na = RandomString(20);
            ret+=Environment.NewLine+@"
            int "+no+@" = 0;int "+curl+" = Console.CursorLeft;int "+curt+" = Console.CursorTop;while ("+no+@" != Console.WindowHeight)
            {int "+na+" = 0;while ("+na+@" != Console.WindowWidth){Console.Write("" "");"+na+@"++;}"+no+"++;}Console.SetCursorPosition("+curl+", "+curt+");";
            ret += Environment.NewLine + "Console.ForegroundColor = " + colours[raw[1].ToString()]+";";
            return ret;
        }
        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        private string RandomString(int size)
        {
            
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        public string ConvertLoc(string batchloc)
        {
            if (batchloc.ToLower().StartsWith("cls"))
            {
                return @"Console.Clear();";
            }
            if (string.IsNullOrWhiteSpace(batchloc))
            {
                return "";
            }
            //string ret = "";
            if (batchloc.ToLower().StartsWith("title"))
            {
                //Console.Title = @"foo";
                return @"Console.Title = @""" + batchloc.Split(' ')[1] + @""";";
            }
            if (batchloc.ToLower().StartsWith("goto"))
            {
                return batchloc.Split(' ')[1] + @"();";
            }
            if (batchloc.ToLower().StartsWith("color"))
            {
                return ParseColour(batchloc.Split(' ')[1]);
            }
            if (batchloc.ToLower().StartsWith("set /p ") && batchloc.Contains("="))
            {
                //set /p var=
                string varname = batchloc.Split(' ')[2].Replace("=", "");
                //var moo = Console.ReadLine();
                return "var @" + varname + " = Console.ReadLine();";
            }
            if (batchloc.ToLower().StartsWith("@echo off"))
            {
                echooff = true;
                return "//echo is off from this point onwards!";
            }
            if (batchloc.ToLower().StartsWith("@echo on"))
            {
                echooff = false;
                return "//echo is on from this point onwards!";
            }
            if (batchloc.ToLower().StartsWith("echo"))
            {
                #region
                //echo you said %var%, how interesting!
                //Console.WriteLine("you said"+var+", how interesting");
                string rett = "Console.WriteLine(";
                rett += @"@""";
                bool pair = false;
                foreach (var item in batchloc.Replace("echo ", ""))
                {
                    if (item == '%')
                    {
                        if (!pair)
                        {
                            //"+
                            rett += @"""+@";
                            pair = true;
                        }
                        else
                        {
                            //+"
                            rett += "+@\"";
                            pair = false;
                        }
                    }
                    else
                    {
                        rett += item;
                    }
                }
                return rett + @""");";
                #endregion
            }
            if (batchloc.ToLower().StartsWith("pause"))
            {
                return "Console.WriteLine(\"Press any key to continue...\"); Console.ReadKey();";
            }
            //   return ret;
            if (batchloc.StartsWith("@"))
            {
                return ConvertLoc(batchloc.Substring(1));
            }
            return "//error converting line: " + batchloc;
        }
        public int FindMethod(string mname, IList<string> rawloc)
        {
            int no = 0;
            foreach (var item in rawloc)
            {
                if (item.Replace(" ", "").StartsWith(":" + mname))
                {
                    return no;
                }
                no++;
            }
            throw new InvalidOperationException(@"method """ + mname + @""" doesn't exist");
        }
        public string RemStartSpace(string raw)
        {
            string ret = "";
            bool done = false;
            foreach (var sym in raw)
            {
                if (!done)
                {
                    //nothing
                    if (sym != ' ')
                    {
                        done = true;
                        ret += sym;
                    }
                }
                else
                {
                    ret += sym;
                }
            }
            return ret;
        }
        private void GetMethods(IList<string> rawloc)
        {
            Methods.Add(new BatchMethod("Main"));
            int lineno = 0;
            // bool boool = false;
            while (lineno != rawloc.Count)
            {

                if (rawloc[lineno].Replace(" ", "").ToLower().StartsWith(":"))
                {
                    foreach (var method in Methods)
                    {
                        if (!method.endf)
                        {
                            method.endf = true;
                            continue;
                        }
                    }
                    Methods.Add(new BatchMethod(rawloc[lineno].Replace(" ", "").Replace(":", "")));
                    lineno += 1;

                }
                //  if (!boool)
                {
                    foreach (var method in Methods)
                    {
                        if (!method.endf)
                        {
                            method.code.Add(RemStartSpace(rawloc[lineno]));
                            continue;
                        }
                    }
                }
                //  else { boool = true; }
                lineno++;
            }
        }
        private void CalculateExec(IList<string> rawloc)
        {
            //deprecated because too many forever loop situations :(
            BatchMethod main = new BatchMethod("Main");
            BatchExec.Add(main);
            int lineno = 0;
            while (lineno != rawloc.Count)
            {
                if (rawloc[lineno].ToLower().Replace(" ", "").StartsWith("goto"))
                {
                    if (rawloc[lineno].ToLower().Replace(" ", "").Length < "goto".Length + 2)
                    {
                        throw new InvalidOperationException(@"Null error happened on batchfile line: " + lineno);
                    }
                    lineno = FindMethod(rawloc[lineno].Replace(" ", "").Replace("goto", ""), rawloc);

                    //  BatchExec.Add(new BatchMethod(rawloc[lineno].Replace(" ", "").Replace(":", "")));
                    //  BatchExec.Add(new BatchMethod(rawloc[lineno].Replace(" ", "").Replace("goto", "")));
                    lineno++;
                }
                if (rawloc[lineno].Replace(" ", "").StartsWith(":"))
                {
                    BatchExec.Add(new BatchMethod(rawloc[lineno].Replace(" ", "").Replace(":", "")));
                }
                // Console.Clear();
                lineno++;
            }
        }

        private void csrun_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "CSC.exe file|*.exe";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = choofdlog.FileName;
                #region baaaaaaaaaaa


             //   System.Diagnostics.Process p = System.Diagnostics.Process.Start(startInfo);
               // p.StandardInput.WriteLine("pause");
                string path = System.IO.Path.GetTempFileName()+".cs";
                //MessageBox.Show(path);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
                {
                    sw.WriteLine(fastColoredTextBox2.Text);
                }
                    ProcessStartInfo start = new ProcessStartInfo("cmd.exe", "/c " + sFileName + " /t:exe /out:MyApplication.exe "+path+" && MyApplication.exe");
                    Process.Start(start);



                #endregion
                // string[] arrAllFiles = choofdlog.FileNames; //used when Multiselect = true           
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetTempFileName() + ".bat";
            //MessageBox.Show(path);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
            {
                sw.WriteLine(fastColoredTextBox1.Text);
            }
            ProcessStartInfo start = new ProcessStartInfo("cmd.exe", "/c "+path);
            Process.Start(start);
        }
    }
    public class BatchMethod
    {
        public List<string> code = new List<string>();
        public string MethodName ="unnamed"+new Random().NextDouble().ToString();
        public BatchMethod(string name)
        {
            MethodName = name;
        }
        public override string ToString()
        {
            return MethodName+"{"+string.Join(";",code)+"}";
        }
        public bool endf = false;
    }
    public class CSharpMethod
    {
        public List<string> code = new List<string>();
        public string MethodName ="unnamed"+new Random().NextDouble().ToString();
        public CSharpMethod(string name)
        {
            MethodName = name;
        }
        public override string ToString()
        {
            return MethodName+"{"+string.Join(";",code)+"}";
        }
        public bool endf = false;
    }
}
