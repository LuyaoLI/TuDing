using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
	class Parse
	{
		String str;
		List<String> tokens;

		public List<Object> parseTree { set; get; }

		public Parse (String str)
		{
			// Get Tokens fron string using regex
			this.str = str;
			this.tokens = str.Replace ("(", " ( ").Replace (")", " ) ").Split ().Where (x => x != "").ToList ();
			List<Object> list = new List<object> ();
			parse (list, 0);
			this.parseTree = list;
		}

		public int parse (List<Object> list, int index)
		{
			// Build Parse tree
			int i;
			for (i = index; i < tokens.Count (); i++) {
				String tmp = tokens.ElementAt (i);
				if (tmp.Equals ("(")) {
					i++;
					List<Object> newList = new List<object> ();
					list.Add (newList);
					i = parse (newList, i);
				} else if (tmp.Equals (")"))
					return i;
				else {
					list.Add (tmp);
				}
			}
			return i;
		}

		public void showParseTree (String indentication, List<Object> tree)
		{ 
			// Show the structure of Parse Tree to Console
			int i;
			String indentication_ = indentication + "    ";
			List<Object> list = new List<Object> ();
			for (i = 0; i < tree.Count (); i++) {
				Object tmp = tree.ElementAt (i);
				if (tmp.GetType ().Equals (list.GetType ())) {
					String toPrint = indentication_ + "*";
					Console.WriteLine (toPrint);
					showParseTree (indentication_, (List<Object>)tmp);
				} else {
					Console.WriteLine (indentication_ + (String)tmp);
				}
			}
			return;
		}
	}
}
