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
			String exp = "(define f (lambda (x) (cond (((= x 0) 1) (true (* x (f (- x 1)))))))) (f 5)";
             Parse parse = new Parse(exp);
                parse.showParseTree("",parse.parseTree);
			Value value = Eval.evalProgaram (parse.parseTree);
			Console.WriteLine(value.ToString());
                Console.ReadLine();
           // }

            
        }
    }
}
