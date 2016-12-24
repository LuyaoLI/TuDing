using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
    class Repl{
   
        static void Main(String[] args)
        {
          //  while (true) 
            //{ ( + 1 ( + 1 1 ))       (cond (( false 2 )( true nil ))   (cdr (car (cons (cons 1 2 ) 3)))  (eq? (atom ab) (atom abc))
			String exp = "(* 3 (/ 2 1) 3 )";
                Parse parse = new Parse(exp);
                parse.showParseTree("",parse.parseTree);
                Value value= Eval.eval(((List<Object>)parse.parseTree).ElementAt(0), new Env());
                Console.WriteLine(value.ToString());
                Console.ReadLine();
           // }

            
        }
    }
}
