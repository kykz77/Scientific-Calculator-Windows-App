using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp
{
    public class Token
    {
    }
    public class Operator : Token
    {
        public string name;
        public int precedence;
        public Operator(string name)
        {
            this.name = name;
            this.precedence = getPrecedence();
        }
        public int getPrecedence()
        {
            int result = 0;
            switch (name)
            {
                case "+":
                    result = 1;
                    break;
                case "-":
                    result = 2;
                    break;
                case "*":
                    result = 3;
                    break;
                case "/":
                    result = 4;
                    break;
                case "^":
                    result = 5;
                    break;
            }
            return result;
        }
        public Constant eval(Constant left, Constant right)
        {
            double result = 0;
            switch (name)
            {
                case "+":
                    result = left.eval() + right.eval();
                    break;
                case "-":
                    result = left.eval() - right.eval();
                    break;
                case "*":
                    result = left.eval() * right.eval();
                    break;
                case "/":
                    result = left.eval() / right.eval();
                    break;
                case "^":
                    result = Math.Pow(left.eval(), right.eval());
                    break;
            }
            Constant constant = new Constant(result.ToString());
            return constant;
        }
    }
    public class Function : Token
    {
        public string name;
        public Function(string name)
        {
            this.name = name;
        }
        public Constant eval(Constant inputStruc)
        {
            double input = inputStruc.eval();
            double result = 0;
            result = Form1.funcDict[name](input);
            Constant constant = new Constant(result.ToString());
            return constant;
        }
    }
    public class Constant : Token
    {
        public string name;
        public Constant(string name)
        {
            this.name = name;
        }
        public double eval()
        {
            double result;
            if (name[0] == '-')
            {
                var fmt = new NumberFormatInfo();
                fmt.NegativeSign = "-";
                result = double.Parse(name, fmt);
            }
            else
            {
                result = double.Parse(name);
            }
            return result;
        }
    }
    public class Group : Token
    {
        public List<Token> tokens;
        public Parser parser = new Parser();
        public Group(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        public Constant eval()
        {
            Constant result = new Constant("");
            int operatorIndex = parser.getOperatorIndex(tokens);
            if (operatorIndex == 0)
            {
                Token currToken = tokens[0];
                if (currToken is Constant)
                {
                    result = (Constant)tokens[0];
                }
                else if (currToken is Group)
                {
                    Group subList = (Group)currToken;
                    result = subList.eval();
                }
                else if (currToken is Function)
                {
                    Function function = (Function)currToken;
                    Group input = (Group)tokens[1];
                    result = function.eval(input.eval());
                }
            }
            else
            {
                Operator node = (Operator)tokens[operatorIndex];
                Group leftGroup = new Group(tokens[0..operatorIndex]);
                Group rightGroup = new Group(tokens[(operatorIndex + 1)..^0]);
                result = node.eval(leftGroup.eval(), rightGroup.eval());
            }
            return result;
        }
    }
}
