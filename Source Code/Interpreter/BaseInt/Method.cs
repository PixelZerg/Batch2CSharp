using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt
{
    public class Method
    {
        public List<CodeTypes.Var> args = new List<CodeTypes.Var>();
        public Type returntype = typeof(void);
        public Visibility visibility = Visibility.PUBLIC;
        public string name;
        public List<string> options = new List<string>();
        public List<Code> code = new List<Code>();
        public List<string> rawcode = new List<string>();
        public bool endf = false;
        public Method(string s)
        {
            name = s;
        }
        public Method()
        {
        }
    }
}
