using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
	class Env
	{
		public Dictionary<String, Value> enviroment { set; get; }

		Env oldEnv { set; get; }

		public Env ()
		{
			this.enviroment = new Dictionary<String, Value> ();
		}

		public Env (Dictionary<String, Value> dict, Env oldEnv)
		{
			this.enviroment = dict;
			this.oldEnv = oldEnv;
		}

		public static Env ExtendEnv (List<Tuple<String, Value>> newEnv, Env oldEnv)
		{
			Env env = new Env (newEnv.ToDictionary (pair => pair.Item1, pair => pair.Item2), oldEnv);
			return env;            
		}

		public Value LookupEnv (String str)
		{
			if (enviroment.ContainsKey (str))
				return enviroment [str];
			if (oldEnv == null)
				throw new Exception ("Enviroment ERROR");
			return oldEnv.LookupEnv (str);
		}
               
	}
}
