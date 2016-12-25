using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
	class Repl
	{
   
		static void Main (String[] args)
		{
			while (true) {
				String exp = Console.ReadLine();
				Parse parse = new Parse (exp);
				Value value = Eval.evalProgaram (parse.parseTree);
				Console.WriteLine (value.ToString ());
				Console.ReadLine ();
			}   
		}
	}
}
