using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt.CodeTypes
{
    public enum Operator
    {
        add,
        minus,
        greater,
        lessthan,
        greaterthanorqueal,
        lessthanorequal,
        equals,
        multiply,
        divide,
    }
    public class AdvancedBool
    {
        public List<Var> items = new List<Var>();
        public List<string> comparisonoptions = new List<string>();
        public List<Operator> comparisonoperators = new List<Operator>();
    }
}
