﻿using System;
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
			if (parseTree is List<Object>) {
				List<Object> tokenList = parseTree as List<Object>;
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
							String name = (string)tokenList [1];
							Value val = eval (tokenList [2], env);
							env.enviroment [name] = val;
							return new Void();
						}
					case "cond":
						{
							Object list = tokenList.ElementAt (1);
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
							if (tokenList.Count () == 3) {
								Object list1 = tokenList.ElementAt (1);
								Object list2 = tokenList.ElementAt (2);
								Value val_1 = eval (list1, env);
								Value val_2 = eval (list2, env);
								return new PairValue (val_1, val_2);
							}
							break;
						}
					case "car":
						{
							if (tokenList.Count () == 2) {
								Object list1 = tokenList.ElementAt (1);
								PairValue pairVal = (PairValue)eval (list1, env);
								return pairVal.val_1;
							}
							break;
						}
					case "cdr":
						{
							if (tokenList.Count () == 2) {
								Object list1 = tokenList.ElementAt (1);
								PairValue pairVal = (PairValue)eval (list1, env);
								return pairVal.val_2;
							}
							break;
						}
					case "atom": 
						{
							if (tokenList.Count () == 2) {
								Object list1 = tokenList.ElementAt (1);
								return new SymbolValue ((String)list1);
							}
							break;
						}
					case "eq?": 
						{
							if (tokenList.Count != 3) {
								throw new Exception ("Wrong Argument For Eq?");
							}
							Value v1 = eval(tokenList [1], env);
							Value v2 = eval(tokenList [2], env);
							if (v1.GetType().Equals(v2.GetType())) {
								if (v1 is NilValue) {
									return new BoolValue (true);
								}
								if (v1 is SymbolValue) {
									return new BoolValue(((SymbolValue)v1).value
										== ((SymbolValue)v2).value);
								}
								if (v1 is NumValue) {
									return new BoolValue (((NumValue)v1).value
										== ((NumValue)v2).value);
								} 
								if (v1 is BoolValue) {
									return new BoolValue (((BoolValue)v1).value
									== ((BoolValue)v2).value);
								}
								return new BoolValue (v1 == v2);
							}
							return new BoolValue (false);                               
						}
					case "lambda": 
						{
							Object[] paramList = ((List<Object>)tokenList.ElementAt (1)).ToArray ();
							Object exp = tokenList.ElementAt (2);
							return new Closure (env, exp, paramList);
						}
					case "let":
						{
							List<Object> clauses = (List<Object>)tokenList.ElementAt (1);
							var pairs = new List<Tuple<String, Value>> ();
							foreach (List<Object> pair in clauses) {
								pairs.Add (new Tuple<String, Value> ((String)pair [0], eval (pair [1], env)));
							}
							Env newEnv = Env.ExtendEnv (pairs, env);
							return eval (tokenList [2], newEnv);
						}
					default:
						{
							try {
							Closure c = (Closure)eval (obj, env);
								return EvalClosure (parseTree, env, c);
							} catch (InvalidCastException e) {
								throw new Exception (String.Format ("Unresolved Name: {0} isn's a valid procedure.", obj as string));
							}
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

		public static Value evalProgaram (Object program, Env env)
		{
			return evalSequence (program, env);
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
