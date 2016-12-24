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
            int sum = 0;
            List<Object> list = new List<Object>();
            for (int i = 0; i < parseTree.Count(); i++)
            {
                Object tmp = parseTree.ElementAt(i);
                String str = "";
                if (tmp.GetType().Equals(list.GetType()))
                {
                    String opt = (String)((List<Object>)tmp).ElementAt(0);
                    int result = eval((List<Object>)tmp, closure);
                }
                else if (tmp.GetType().Equals(str.GetType()))
                {
                    String opt = (String)tmp;
                    sum += int.Parse((String)tmp);
                }
                else 
                {
                    Console.WriteLine("ERROR HERE");
                }
            }
            return sum;
        }


    }
}
