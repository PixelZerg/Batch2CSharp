using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt.CodeTypes
{
    public class MathDeclareVariable
    {
        public List<Var> vars = new List<Var>();
        public List<Operator> mathematicaloperators = new List<Operator>();
        public Var outputvar;
    }
}
