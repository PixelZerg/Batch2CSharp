using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.BaseInt
{
    public class Code
    {
        public Code(Type type)
        {
            CodeType = type;
            this.code = Activator.CreateInstance(type, new object[] { });
        }
        public Type CodeType;
        public List<string> options = new List<string>();
        public object code;
        public dynamic Get()
        {
            dynamic changedObj = Convert.ChangeType(code, CodeType);
            return changedObj;
        }
        public T To<T>()
        {
            //object o = this;
            return (T)code;
        }
        public T Create<T>()
        {
            this.code = Activator.CreateInstance<T>();
            return this.To<T>();
        }
        //public Type GetType
    }
}
