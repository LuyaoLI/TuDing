using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
	class Repl
	{
   
		public static void Main (String[] args)
		{
			Env env = new Env ();
			Console.WriteLine ("TuDing Lisp Interpreter REPL");
			while (true) {
				Console.Write (">>");
				String exp = Console.ReadLine();
				Parse parse = new Parse (exp);
				Value value = Eval.evalProgaram (parse.parseTree, env);
				Console.WriteLine (value.ToString ());
			}   
		}
	}
}
