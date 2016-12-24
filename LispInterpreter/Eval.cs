using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
    class Eval
    {
        public int eval(List<Object> parseTree, Closure closure) 
        {
            List<Object> list = new List<Object>();
            for (int i = 0; i < parseTree.Count(); i++)
            {
                Object tmp = parseTree.ElementAt(i);
                if (tmp.GetType().Equals(list.GetType())) 
                {
                    return 0;
                }
           
            }

            return 0;
        }


    }
}
