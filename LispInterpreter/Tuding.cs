using System;
using System.IO;

namespace LispInterpreter
{
	public class Tuding
	{
		static void Repl(Env env) {
			Console.WriteLine ("TuDing Lisp Interpreter REPL");
			while (true) {
				try {
				Console.Write (">> ");
				String exp = Console.ReadLine();
				Parse parse = new Parse (exp);
				Value value = Eval.evalProgaram (parse.parseTree, env);
				Console.WriteLine (value.ToString ());
				} catch (Exception e) {
					Console.WriteLine ("Invalid Statement");
				}
			}
		}

		static void Main (String[] args)
		{
			Env env = new Env ();
			string sys = File.ReadAllText ("System.td");
			Eval.evalProgaram (new Parse (sys).parseTree, env);
			if (args.Length < 1) {
				Repl (env);
			}
			string path = args[0];
			string code = File.ReadAllText (path);
			Parse parse = new Parse (code);
			Value value = Eval.evalProgaram (parse.parseTree, env);
			Console.WriteLine (value.ToString ());
		}
	}
}

