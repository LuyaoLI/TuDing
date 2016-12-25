using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
	class Eval
	{
		public static Value eval (Object parseTree, Env env)
		{
			NumValue sumVal = null;
			if (parseTree is List<Object>) {
				Object obj = ((List<Object>)parseTree).ElementAt (0);
				if (obj is String) { // operator processing
					switch ((String)obj) {
					case "+":
						{
							return new NumValue (EvalFoldr (parseTree, env, (v1, v2) => v1 + v2));
						}
					case "-":
						{
							return new NumValue (EvalFoldr (parseTree, env, (v1, v2) => v1 - v2));
						}
					case "*":
						{
							return new NumValue (EvalFoldr (parseTree, env, (v1, v2) => v1 * v2));
						}
					case "/":
						{
							return new NumValue (EvalFoldr (parseTree, env, (v1, v2) => v1 / v2));
						}
					case "=": 
						{
							return new BoolValue (EvalEqualNum (parseTree, env));
						}
					case "!=":
						{
							return new BoolValue (!EvalEqualNum (parseTree, env));
						}
					case ">":
						{
							return new BoolValue (EvalCompare (parseTree, env, (a, b) => a > b));
						}
					case "<=":
						{
							return new BoolValue (EvalCompare (parseTree, env, (a, b) => a <= b));
						}
					case "<":
						{
							return new BoolValue (!EvalCompare (parseTree, env, (a, b) => a < b));
						}
					case ">=":
						{
							return new BoolValue (EvalCompare (parseTree, env, (a, b) => a >= b));
						}
					case "define":
						{
							String name = (string)((List<Object>)parseTree) [1];
							Value val = eval (((List<Object>)parseTree) [2], env);
							env.enviroment [name] = val;
							return new Void();
						}
					case "cond":
						{
							Object list = ((List<Object>)parseTree).ElementAt (1);
							if (list is List<Object>) {
                                    
								for (int k = 0; k < ((List<Object>)list).Count (); k++) {
									Object deci = ((List<Object>)list).ElementAt (k);
									BoolValue boolVal = (BoolValue)eval (((List<Object>)deci).ElementAt (0), env);
									if (boolVal.value == true) {
										return eval (((List<Object>)deci).ElementAt (1), env);
									}
								}
							}
							break;
						}
					case "cons":
						{
							if (((List<Object>)parseTree).Count () == 3) {
								Object list1 = ((List<Object>)parseTree).ElementAt (1);
								Object list2 = ((List<Object>)parseTree).ElementAt (2);
								Value val_1 = eval (list1, env);
								Value val_2 = eval (list2, env);
								return new PairValue (val_1, val_2);
							}
							break;
						}
					case "car":
						{
							if (((List<Object>)parseTree).Count () == 2) {
								Object list1 = ((List<Object>)parseTree).ElementAt (1);
								PairValue pairVal = (PairValue)eval (list1, env);
								return pairVal.val_1;
							}
							break;
						}
					case "cdr":
						{
							if (((List<Object>)parseTree).Count () == 2) {
								Object list1 = ((List<Object>)parseTree).ElementAt (1);
								PairValue pairVal = (PairValue)eval (list1, env);
								return pairVal.val_2;
							}
							break;
						}
					case "atom": 
						{
							if (((List<Object>)parseTree).Count () == 2) {
								Object list1 = ((List<Object>)parseTree).ElementAt (1);
								return new SymbolValue ((String)list1);
							}
							break;
						}
					case "eq?": 
						{
							Object nextParseTree = ((List<Object>)parseTree).ElementAt (1);
							SymbolValue val_1_tmp = (SymbolValue)eval (nextParseTree, env);
							for (int j = 2; j < ((List<Object>)parseTree).Count (); j++) {
								Object nextParseTree_ = ((List<Object>)parseTree).ElementAt (j);
								SymbolValue val_2_tmp = (SymbolValue)eval (nextParseTree_, env);
								if (!val_1_tmp.Equals (val_2_tmp))
									return new BoolValue (false);
							}
							return new BoolValue (true);                               
						}
					case "lambda": 
						{
							Object[] paramList = ((List<Object>)((List<Object>)parseTree).ElementAt (1)).ToArray ();
							Object exp = ((List<Object>)parseTree).ElementAt (2);
							return new Closure (env, exp, paramList);
						}
					case "let":
						{
							List<Object> clauses = (List<Object>)((List<Object>)parseTree).ElementAt (1);
							var pairs = new List<Tuple<String, Value>> ();
							foreach (List<Object> pair in clauses) {
								pairs.Add (new Tuple<String, Value> ((String)pair [0], eval (pair [1], env)));
							}
							Env newEnv = Env.ExtendEnv (pairs, env);
							return eval (((List<Object>)parseTree) [2], newEnv);
						}
					default:
						{
							Closure c = (Closure)eval (obj, env);
							return EvalClosure (parseTree, env, c);
						}
					}
				} else if (obj is List<Object>) {
					Value value = eval (obj, env);
					return EvalClosure (parseTree, env, (Closure)value);
				}
                    
			} else if (parseTree is String) {
				if (((String)parseTree).Equals ("false"))
					return new BoolValue (false);
				if (((String)parseTree).Equals ("true"))
					return new BoolValue (true);
				if (((String)parseTree).Equals ("nil"))
					return new NilValue ();
				try {
					return new NumValue (int.Parse ((String)parseTree));
				} catch (Exception e) {
					return env.LookupEnv ((String)parseTree);
				}
			}
			return null;
		}

		public static Value evalSequence (Object parseTree, Env env)
		{
			var result = ((List<Object>)parseTree).Select (elem => eval (elem, env)).ToList ();
			return result.Last ();
		}

		public static Value evalProgaram (Object program)
		{
			return evalSequence (program, new Env ());
		}

		public static Boolean EvalEqualNum (Object parseTree, Env env)
		{
			var tokens = ((List<Object>)parseTree)
				.Skip (1).ToList ();
			var firstVal = ((NumValue)eval (tokens [0], env)).value;
			return !tokens.Skip (1)
				.Select (elem => ((NumValue)eval (elem, env)).value)
				.All (value => value != firstVal);
		}

		public static Boolean EvalCompare (Object parseTree, Env env, Func<int, int, bool> comparator)
		{
			var values = ((List<Object>)parseTree)
				.Skip (1)
				.Select (elem => ((NumValue)eval (elem, env)).value)
				.ToList ();
			return values
				.Zip (values.Skip (1), comparator)
				.All (b => b);
		}

		public static int EvalFoldr (Object parseTree, Env env, Func<int, int, int> accumulator)
		{
			var values = ((List<Object>)parseTree)
				.Skip (1)
				.Select (elem => ((NumValue)eval (elem, env)).value)
				.ToList ();
			return values
				.Aggregate (accumulator);
		}

		public static Value EvalClosure (Object parseTree, Env env, Closure value)
		{
			int length = ((Closure)value).param.Length;
			if (((List<Object>)parseTree).Count () != length + 1)
				throw new Exception ("Params num insuiable");
			Closure closure = (Closure)value;
			List<Tuple<String, Value>> newEnv = new List<Tuple<string, Value>> ();
			for (int k = 1; k < ((List<Object>)parseTree).Count (); k++) {
				Value val = eval (((List<Object>)parseTree) [k], env);
				String str = (String)(closure.param [k - 1]);
				newEnv.Add (new Tuple<String, Value> (str, val));
			}
			Env tmpEnv = Env.ExtendEnv (newEnv, closure.env);
			return eval (closure.exp, tmpEnv);
		}
	}
}
