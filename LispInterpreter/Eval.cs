using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
    class Eval
    {
        public static Value eval(Object parseTree, Env env)
        {
            NumValue sumVal = null;
            int sum = 0;
            if (parseTree is List<Object>)
            {
                Object obj = ((List<Object>)parseTree).ElementAt(0);
                if (obj is String) // operator processing
                {
                    switch ((String)obj)
                    {
                        case "+":
                            {
                                for (int j = 1; j < ((List<Object>)parseTree).Count(); j++)
                                {
                                    Object nextParseTree = ((List<Object>)parseTree).ElementAt(j);
                                    Value val = eval(nextParseTree, env);
                                    if (val is NumValue)
                                    {
                                        sum += ((NumValue)val).value;
                                    }
                                }
                                sumVal = new NumValue(sum);
                                break;
                            }
                            //TODO - * /  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        case "cond":
                            {
                                Object list = ((List<Object>)parseTree).ElementAt(1);
                                if (list is List<Object>) 
                                {
                                    
                                    for (int k = 0; k < ((List<Object>)list).Count(); k++) 
                                    {
                                        Object deci = ((List<Object>)list).ElementAt(k);
                                        BoolValue boolVal = (BoolValue)eval(((List<Object>)deci).ElementAt(0), env);
                                        if (boolVal.value == true)
                                        {
                                            return eval(((List<Object>)deci).ElementAt(1), env);
                                        }
                                    }
                                }
                                break;
                            }
                        case "cons":
                            {
                                if (((List<Object>)parseTree).Count() == 3) 
                                {
                                    Object list1 = ((List<Object>)parseTree).ElementAt(1);
                                    Object list2 = ((List<Object>)parseTree).ElementAt(2);
                                    Value val_1 = eval(list1, env);
                                    Value val_2 = eval(list2, env);
                                    return new PairValue(val_1, val_2);
                                }
                                break;
                            }
                        case "car":
                            {
                                if (((List<Object>)parseTree).Count() == 2)
                                {
                                    Object list1 = ((List<Object>)parseTree).ElementAt(1);
                                    PairValue pairVal = (PairValue)eval(list1, env);
                                    return pairVal.val_1;
                                }
                                break;
                            }
                        case "cdr":
                            {
                                if (((List<Object>)parseTree).Count() == 2)
                                {
                                    Object list1 = ((List<Object>)parseTree).ElementAt(1);
                                    PairValue pairVal = (PairValue)eval(list1, env);
                                    return pairVal.val_2;
                                }
                                break;
                            }
                        case "atom": 
                            {
                                if (((List<Object>)parseTree).Count() == 2)
                                {
                                    Object list1 = ((List<Object>)parseTree).ElementAt(1);
                                    return new SymbolValue((String)list1);
                                }
                                break;
                            }
                        case "eq?": 
                            {
                                Object nextParseTree = ((List<Object>)parseTree).ElementAt(1);
                                SymbolValue val_1_tmp = (SymbolValue)eval(nextParseTree, env);
                                for (int j = 2; j < ((List<Object>)parseTree).Count(); j++)
                                {
                                    Object nextParseTree_ = ((List<Object>)parseTree).ElementAt(j);
                                    SymbolValue val_2_tmp = (SymbolValue)eval(nextParseTree_, env);
                                    if (!val_1_tmp.Equals(val_2_tmp))
                                        return new BoolValue(false);
                                }
                                return new BoolValue(true);                               
                            }
                        case "lambda": 
                            {
                                Object[] paramList = ((List<Object>)((List<Object>)parseTree).ElementAt(1)).ToArray();
                                Object exp = ((List<Object>)parseTree).ElementAt(2);
                                return new Closure(env, exp, paramList);
                            }
                    }
                    return sumVal;
                }
                else if (obj is List<Object>) 
                {
                    Value value = eval(obj, env);
                    if (value is Closure) 
                    {
                        int length = ((Closure)value).param.Length;
                        if (((List<Object>)parseTree).Count() != length + 1) throw new Exception("Params num insuiable");
                        Closure closure = (Closure)value;
                        List<Tuple<String, Value>> newEnv = new List<Tuple<string, Value>>();
                        for (int k = 1; k < ((List<Object>)parseTree).Count(); k++) 
                        {
                            Value val = eval(((List<Object>)parseTree)[k], env);
                            String str = (String)(closure.param[k-1]);
                            newEnv.Add(new Tuple<String, Value>(str, val));
                        }
                        Env tmpEnv = Env.ExtendEnv(newEnv, closure.env);
                        return eval(closure.exp, tmpEnv);
                    }
                }
                    
            }
            else if (parseTree is String)
            {
                if (((String)parseTree).Equals("false"))
                    return new BoolValue(false);
                if (((String)parseTree).Equals("true"))
                    return new BoolValue(true);
                if (((String)parseTree).Equals("nil"))
                    return new NilValue();
                try {
                    return new NumValue(int.Parse((String)parseTree));
                }catch(Exception e){
                    return env.LookupEnv((String)parseTree);
                }
            }
            return null;
        }
    }
}
