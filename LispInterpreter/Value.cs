using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispInterpreter
{
    class Value
    {
    }

    class NumValue : Value 
    {
        public int value { set; get; }

        public NumValue(int value) 
        {
            this.value = value;
        }
       
        public override String ToString() 
        {
            return value.ToString();
        }

    }

    class SymbolValue : Value 
    {
        public String value { set; get; }

        public SymbolValue(String value) 
        {
            this.value = value;
        }

        public Boolean Equals(SymbolValue val_2) 
        {
            if (this.value.Equals(val_2.value))
                return true;
            else return false;
        }

        public override String ToString() 
        {
            return "\'"+value;
        }
    }

    class BoolValue : Value 
    {
        public Boolean value { set; get; }
        public BoolValue(Boolean value) 
        {
            this.value = value;
        }

        public override String ToString() 
        {
            return value.ToString();
        }
    }

    class NilValue : Value 
    {
        public override String ToString()
        {
            return "nil";
        }
    }

    class PairValue : Value 
    {
        public Value val_1 { set; get; }
        public Value val_2 { set; get; }

        public PairValue(Value val_1, Value val_2) 
        {
            this.val_1 = val_1;
            this.val_2 = val_2;
        }

        public override String ToString()
        {
            return "( " + val_1.ToString() + " . " + val_2.ToString() + " )";
        }
    }
}
