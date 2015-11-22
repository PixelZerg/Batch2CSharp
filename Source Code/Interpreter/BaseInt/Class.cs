using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt
{
    public enum Visibility
    {
        PRIVATE,
        PUBLIC,

    }
    public class Class
    {
        public string name;
        public List<string> Options = new List<string>();
        public List<Method> methods = new List<Method>();
    }
}
