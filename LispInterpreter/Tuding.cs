using System;
using System.IO;

namespace LispInterpreter
{
	public class Tuding
	{
		static void Main (String[] args)
		{
			if (args.Length < 1) {
				Repl.Main (args);
			}
			string path = args[0];
			string code = File.ReadAllText (path);
			Env env = new Env ();
			Parse parse = new Parse (code);
			parse.showParseTree (" ", parse.parseTree);
			Value value = Eval.evalProgaram (parse.parseTree, env);
			Console.WriteLine (value.ToString ());
		}
	}
}

