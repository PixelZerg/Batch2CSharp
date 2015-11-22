using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt.CodeTypes
{
    public class Var
    {
        public string name;
        public Type type;
        public bool isdeclared = true;
        public bool isvardataset = false;
        public string data;
    }
}
