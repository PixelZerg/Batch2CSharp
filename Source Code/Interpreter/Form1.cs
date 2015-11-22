using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class Form1 : Form
    {
        public List<Type> UC = new List<Type>();
        public List<Type> UC2 = new List<Type>();
        public List<Type> singles = new List<Type>();
        public BaseInt.Namespace basecode = new BaseInt.Namespace();
        public object selout;
        //public Interpreters.Base baa = new Interpreters.Base();
        public void Update()
        {
            //UC2[comboBox2.SelectedIndex].
            //dynamic bar = Convert.ChangeType(selout, UC2[comboBox2.SelectedIndex]);
               // selout.Update();
            base1.Update(); cSharp1.Update();
           // baa.Update();
        }
        public BaseInt.Namespace baseNamespace()
        {
            BaseInt.Namespace n = new BaseInt.Namespace();
            n.name = textBox1.Text;
            basecode = n;
            return n;
        }
        public Form1()
        {
            InitializeComponent();
            
     //       UC.Add(typeof(Interpreters.BatchCSharp));
       //     comboBox1.Items.Add(typeof(Interpreters.BatchCSharp));
           // AddInterpreter(typeof(Interpreters.Batch), typeof(Interpreters.Base));
           // singles.Add(typeof(Interpreters.BatchCSharp));
            
        }
        public void AddInterpreter(Type type1, Type type2)
        {
            UC.Add(type1);
            UC2.Add(type2);
            comboBox1.Items.Add(type1);
            comboBox2.Items.Add(type2);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainPanel.Controls.Clear();
            try
            {
                Control c = (Control)Activator.CreateInstance(UC[comboBox1.SelectedIndex]);
                c.Dock = DockStyle.Fill;
                if (singles.Contains(UC[comboBox1.SelectedIndex]))
                {
                    MainPanel.Controls.Add(c);
                    return;
                }
                c.Dock = DockStyle.Top;
                c.Size = new Size(c.Size.Width, MainPanel.Size.Height / 2);
                dynamic foo = Convert.ChangeType(c, UC[comboBox1.SelectedIndex]);
                try
                {
                    foo.SetParent(this);
                }
                catch { }
                MainPanel.Controls.Add(foo);
                Control d = (Control)Activator.CreateInstance(UC2[comboBox2.SelectedIndex]);
                d.Dock = DockStyle.Bottom;
                d.Size = new System.Drawing.Size(c.Size.Width, MainPanel.Size.Height / 2);
                dynamic bar = Convert.ChangeType(d, UC2[comboBox2.SelectedIndex]);
                try
                {
                    bar.SetParent(this);
                }
                catch { }
                selout = bar;
                MainPanel.Controls.Add(bar);
                return;
            }
            catch {
                Console.WriteLine("*hiccup*");
                Interpreters.BatchCSharp bcs= new Interpreters.BatchCSharp();
                bcs.Dock = DockStyle.Fill;
                MainPanel.Controls.Add(bcs);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            basecode.name = textBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainPanel.Controls.Clear();
            try
            {
                Control c = (Control)Activator.CreateInstance(UC[comboBox1.SelectedIndex]);
                c.Dock = DockStyle.Fill;
                if (singles.Contains(UC[comboBox1.SelectedIndex]))
                {
                    MainPanel.Controls.Add(c);
                    return;
                }
                c.Dock = DockStyle.Top;
                c.Size = new Size(c.Size.Width, MainPanel.Size.Height / 2);
                dynamic foo = Convert.ChangeType(c, UC[comboBox1.SelectedIndex]);
                try
                {
                    foo.SetParent(this);
                }
                catch { }
                MainPanel.Controls.Add(foo);
                c = (Control)Activator.CreateInstance(UC2[comboBox2.SelectedIndex]);
                c.Dock = DockStyle.Fill;
                dynamic bar = Convert.ChangeType(c, UC2[comboBox2.SelectedIndex]);
                try
                {
                    bar.SetParent(this);
                }
                catch { }
                selout = bar;
                MainPanel.Controls.Add(bar);
                return;
            }
            catch
            {
                Console.WriteLine("*hiccup*");
                Interpreters.BatchCSharp bcs = new Interpreters.BatchCSharp();
                bcs.Dock = DockStyle.Fill;
                MainPanel.Controls.Add(bcs);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            batch1.SetParent(this); base1.SetParent(this); cSharp1.SetParent(this);
            /*/
            MainPanel.Controls.Clear();
            Interpreter.Interpreters.BatchCSharp bcs = new Interpreters.BatchCSharp();
            bcs.Dock = DockStyle.Fill;
            MainPanel.Controls.Add(bcs);
            //*/
        }
    }
}
