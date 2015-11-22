using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.IO;
using System.Text;
using Interpreter.BaseInt;
using Interpreter.BaseInt.CodeTypes.Console;
using Interpreter.BaseInt.CodeTypes.Commands;
using Interpreter.BaseInt.CodeTypes;

namespace Interpreter.Interpreters
{
    public partial class Batch : UserControl
    {
        #region
        public TextStyle RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        public TextStyle GreennStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        public TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);

        public TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);

        public TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);

        public TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);

        public TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);

        public TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);

        public TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        #endregion
        public Form1 parent;
        public BaseInt.Class program = new BaseInt.Class();
        public Batch()
        {
            InitializeComponent();
            fastColoredTextBox1.TextChanged += fastColoredTextBox1_TextChanged;
        }
        public void SetParent(Form1 f)
        {
            parent = f;
        }
        void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            #region moo
            e.ChangedRange.ClearStyle(RedStyle, GreennStyle, GreenStyle, BlueStyle, BoldStyle, GrayStyle, MagentaStyle, BrownStyle, MaroonStyle);
            #endregion
            #region syntax highlighting
         e.ChangedRange.ClearStyle();
           e.ChangedRange.SetFoldingMarkers(":", ":");
         e.ChangedRange.SetStyle(MagentaStyle, @"@|<|>");
         e.ChangedRange.SetStyle(BlueStyle, "echo|set|pause|sleep|title|if|goto|color|cls", RegexOptions.IgnoreCase);
         e.ChangedRange.SetStyle(BrownStyle, "%([^%]*)%");
         e.ChangedRange.SetStyle(GreennStyle, "/p|/a");
         e.ChangedRange.SetStyle(RedStyle, "0|1|2|3|4|5|6|7|8|9");
            #endregion
         parent.baseNamespace();

             Parse(fastColoredTextBox1.Lines);
         
            parent.basecode.classes.Add(program);
            parent.Update();
        }
        private string RandomString(int size)
        {

            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * new Random((int)DateTime.Now.Ticks).NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
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
        public void Parse(IList<string> raw)
        {
            //Console.Clear();   
            program = new BaseInt.Class();
            program.name = "unnamed"+RandomString(5);
            ParseMethods(raw);
            foreach (var method in program.methods)
            {
                try{
                ParseRawLoc(method);
                    }
         catch(Exception ex)
         {
             Code c = new Code(typeof(comment));
             c.To<comment>().commentedtext = "Parsing error: " + ex.Message + Environment.NewLine+ex.Source+Environment.NewLine+ex.StackTrace+Environment.NewLine+ex.InnerException;
             c.To<comment>().multiline = true;
             method.code.Add(c);
         }
            }
        }
        public void ParseMethods(IList<string> raw)
        {
            program.methods.Add(new BaseInt.Method("Main"));
            int lineno = 0;
            // bool boool = false;
            while (lineno != raw.Count)
            {

                if (raw[lineno].Replace(" ", "").ToLower().StartsWith(":"))
                {
                    foreach (var method in program.methods)
                    {
                        if (!method.endf)
                        {
                            method.endf = true;
                            continue;
                        }
                    }
                    program.methods.Add(new Method(raw[lineno].Replace(" ", "").Replace(":", "")));
                   // Console.WriteLine("new method"+raw[lineno].Replace(" ", "").Replace(":", ""));
                    lineno += 1;

                }
                //  if (!boool)
                {
                    foreach (var method in program.methods)
                    {
                        if (!method.endf)
                        {
                            method.rawcode.Add(RemStartSpace(raw[lineno]));
                          //  Console.WriteLine(method.name + " added " + RemStartSpace(raw[lineno]));
                            continue;
                        }
                    }
                }
                //  else { boool = true; }
                lineno++;
            }
        }
        public void ParseRawLoc(Method m)
        {
            int no = 0;
            while (no != m.rawcode.Count)
            {
                if (m.rawcode[no].ToLower().StartsWith("goto"))
                {
                   Code c= new Code(typeof(callmethod));
                   c.To<callmethod>().methodname = m.rawcode[no].Replace("goto ", "");
                    m.code.Add(c);
                }
                if (m.rawcode[no].ToLower().StartsWith("pause"))
                {
                    m.code.Add(new Code(typeof(pause)));
                }
                if (m.rawcode[no].ToLower().StartsWith("echo "))
                {
                    Code c = new Code(typeof(Output));
                    c.To<Output>().outstring = m.rawcode[no].Replace("echo ", "") + Environment.NewLine;
                    if (m.rawcode[no].ToLower().StartsWith("echo."))
                    {
                        c.To<Output>().outstring = Environment.NewLine;
                    }
                    m.code.Add(c);
                }
                if (m.rawcode[no].ToLower().StartsWith("set"))
                {
                    #region set
                    Var v = new Var();
                    v.isdeclared = true;
                    //0  1
                    ///p var=
                    #region getvarname
                    foreach (var item in m.rawcode[no].Split(' '))
                    {
                        if (item.Contains("="))
                        {
                            v.name = item.Substring(0,item.IndexOf('='));
                        }
                    }
                    #endregion
                    
                    // 0   1  2
                    //set /p var=
                    #region getvartype
                    try
                    {
                        if (m.rawcode[no].Split(' ')[1].Contains("/a"))
                        {
                            v.type = typeof(int);
                        }
                        else
                        {
                            v.type = typeof(string);
                        }
                    }
                    catch { v.type = typeof(string); }
                    v.isvardataset = false;
                    #endregion
                    #region declare var
                    Code c = new Code(typeof(DeclareVariable));
                    #region set
                    if (!string.IsNullOrEmpty(m.rawcode[no].Split('=')[1]))
                    {
                        v.data = m.rawcode[no].Split('=')[1].Replace("=", "");
                        c.To<DeclareVariable>().setvariabledata = true;
                    }
                    #endregion
                   // c.To<DeclareVariable>().setvariabledata = false;
                    c.To<DeclareVariable>().variable = v;
                    m.code.Add(c);// string s;
                    #endregion

                    //----------------------------------------------------------
                    #region mathematical var definition mk1
                  //v super fancy comment switch
                    //*
                    if (m.rawcode[no].Split(' ')[1].Contains("/a"))
                    {
                        Code cc = new Code(typeof(MathDeclareVariable));
                        List<string> vars = new List<string>();
                        #region parse all vars involved
                        foreach (var item in m.rawcode[no].Split(' ')[2].Substring(
                            m.rawcode[no].Split(' ')[2].IndexOf('=') + 0, m.rawcode[no].Split(' ')[2].Length))
                        {
                            #region parsing stuff
                            if (item == '%')
                            {
                                string varname = "";
                                vars.Add(varname);
                            }
                            else if (new char[]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p'
                                ,'q','r','s','t','u','v','w','x','y','z'}.Contains(item.ToString().ToLower()[0]))
                            {
                                vars[vars.Count] += item;
                            }
                            if (item == '+')
                            {
                                cc.To<MathDeclareVariable>().mathematicaloperators.Add(Operator.add);
                            }
                            if (item == '-')
                            {
                                cc.To<MathDeclareVariable>().mathematicaloperators.Add(Operator.minus);
                            }
                            if (item == '/')
                            {
                                cc.To<MathDeclareVariable>().mathematicaloperators.Add(Operator.divide);
                            }
                            if (item == '*')
                            {
                                cc.To<MathDeclareVariable>().mathematicaloperators.Add(Operator.multiply);
                            }
                            #endregion
                        }
                        #region remove empty values
                        no = 0;
                        while (no != vars.Count)
                        {
                            if (string.IsNullOrEmpty(vars[no]))
                            {
                                vars.RemoveAt(no);
                            }
                            no++;
                        }
                        #endregion
                        #endregion
                        foreach (var va in vars)
                        {
                            Var var = new Var();
                            var.isdeclared = true;
                            var.isvardataset = true;//probably or else batch wont compile
                            var.name = va;
                            var.type = typeof(int);//""
                            cc.To<MathDeclareVariable>().vars.Add(var);

                        }
                        v.isvardataset = true;
                        cc.To<MathDeclareVariable>().outputvar = v;
                        m.code.Add(cc);
                    }
                     /*/
                    
                    //*
                    //
                    #endregion
                    #region mathematical var def mk2
                    if (m.rawcode[no].Split(' ')[1].Contains("/a"))
                    {
                        Code cc = new Code(typeof(MathDeclareVariable));
                        v.isvardataset = true;
                        cc.To<MathDeclareVariable>().outputvar = v;
                        cc.options.Add(m.rawcode[no].Split(' ')[1]
                    }
                    //*/
                    #endregion
                    if (m.rawcode[no].Split(' ')[1].Contains("/p"))
                    {
                        //set /p var=
                        Code cc = new Code(typeof(Input));
                        cc.To<Input>().storevar = true;
                        v.isvardataset = true;
                        cc.To<Input>().var = v;
                        //vvvv
                        //VARNAME = Console.ReadLine();
                        m.code.Add(cc);
                    }
                    #endregion
                }
                no++;
            }
        }
    }
}
