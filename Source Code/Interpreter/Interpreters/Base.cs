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

namespace Interpreter.Interpreters
{
    public partial class Base : UserControl
    {
        public Form1 parent;
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
        public Base()
        {
            InitializeComponent();
        }
        public void SetParent(Form1 f)
        {
            parent = f;
        }
        public void Update()
        {
            fastColoredTextBox1.Clear();
            fastColoredTextBox1.Text = Newtonsoft.Json.JsonConvert.SerializeObject(parent.basecode, Newtonsoft.Json.Formatting.Indented);
        }
        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            #region moo
            e.ChangedRange.ClearStyle(RedStyle, GreennStyle, GreenStyle, BlueStyle, BoldStyle, GrayStyle, MagentaStyle, BrownStyle, MaroonStyle);
            #endregion
            #region syntax highlighting
            e.ChangedRange.SetFoldingMarkers("{", "}");
            //e.ChangedRange.SetStyle(MagentaStyle, @"@|<|>");
            e.ChangedRange.SetStyle(BlueStyle, "true|false|null", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(BrownStyle, @"""([^""]*)""");
            e.ChangedRange.SetStyle(GreenStyle, "//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle, "(/\\*.*?\\*/)|(/\\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(GreenStyle, "(/\\*.*?\\*/)|(.*\\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
           // e.ChangedRange.SetStyle(GreennStyle, "/p|/a");
            fastColoredTextBox1.LeftBracket = '{';
            fastColoredTextBox1.RightBracket = '}';
            fastColoredTextBox1.RightBracket2 = ']';
            fastColoredTextBox1.LeftBracket2 = '[';
            e.ChangedRange.SetStyle(RedStyle, "0|1|2|3|4|5|6|7|8|9|-");
            #endregion
        }
    }
}
