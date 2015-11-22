using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt.CodeTypes
{
    public class While
    {
        public AdvancedBool condition;
        public List<Code> codetodo;
    }
}
