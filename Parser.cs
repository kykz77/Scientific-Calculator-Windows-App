using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WindowsApp
{
    public class Parser
    {
        public bool isOperator(char chr)
        {
            return (chr == '+' || chr == '-' || chr == '*' || chr == '/' || chr == '(' || chr == ')' || chr == '^');
        }
        public bool isFunction(string str)
        {
            return (Form1.funcDict.ContainsKey(str));
        }
        public List<string> tokenize(string expr)
        {
            expr = expr.ToLower();
            expr = Regex.Replace(expr, " ", "");
            expr = Regex.Replace(expr, "pi", Math.PI.ToString());
            int length = expr.Length;
            List<string> tokenList = new List<string>();
            string currStr = "";
            for (int i = 0; i < length; i++)
            {
                char currChar = expr[i];
                if (isOperator(currChar))
                {
                    if (currStr != "")
                    {
                        tokenList.Add(currStr);
                        currStr = "";
                    }
                    if ((currChar == '-') && ((i == 0) || (i < length - 1 && i > 0 && expr[i - 1] == '(' && !isOperator(expr[i + 1]))))
                    {
                        tokenList.Add("-1");
                        tokenList.Add("*");
                    }
                    else
                    {
                        tokenList.Add(currChar.ToString());
                    }
                }
                else
                {
                    currStr += currChar;
                }
            }
            if (currStr != "")
            {
                tokenList.Add(currStr);
            }
            return tokenList;
        }
        public List<List<int>> getGroupIndex(List<string> tokenList)
        {
            int length = tokenList.Count;
            Stack<int> indexStack = new Stack<int>();
            List<List<int>> groupIndex = new List<List<int>>();
            for (int i = 0; i < length; i++)
            {
                string currStr = tokenList[i];
                if (currStr == "(")
                {
                    indexStack.Push(i);
                }
                else if (currStr == ")")
                {
                    groupIndex.Add([indexStack.Peek(), i]);
                    indexStack.Pop();
                }
            }
            return groupIndex;
        }
        public int getGroupEnd(int startIndex, List<List<int>> groupIndex)
        {
            foreach (var item in groupIndex)
            {
                if (item[0] == startIndex)
                {
                    return item[1];
                }
            }
            return 0;
        }
        public List<Token> lex(List<string> tokenList)
        {
            List<List<int>> groupIndex = getGroupIndex(tokenList);
            List<Token> lexList = new List<Token>();
            int length = tokenList.Count;
            for (int i = 0; i < length; i++)
            {
                string currStr = tokenList[i];
                if (currStr == "(")
                {
                    int startIndex = i + 1;
                    int endIndex = getGroupEnd(i, groupIndex);
                    lexList.Add(new Group(lex(tokenList[startIndex..endIndex])));
                    i += endIndex - startIndex + 1;
                }
                else if (isOperator(currStr.ToCharArray()[0]) && currStr.ToCharArray().Length == 1 && currStr != ")")
                {
                    lexList.Add(new Operator(currStr));
                }
                else if (isFunction(currStr))
                {
                    lexList.Add(new Function(currStr));
                }
                else
                {
                    lexList.Add(new Constant(currStr));
                }
            }
            return lexList;
        }
        public List<List<int>> sortOperators(List<List<int>> operatorIndex)
        {
            int length = operatorIndex.Count;
            for (int i = 1; i < length; i++)
            {
                int j = i;
                while (j > 0 && operatorIndex[j - 1][0] > operatorIndex[j][0])
                {
                    List<int> temp = operatorIndex[j];
                    operatorIndex[j] = operatorIndex[j - 1];
                    operatorIndex[j - 1] = temp;
                    j--;
                }
            }
            return operatorIndex;
        }
        public int getOperatorIndex(List<Token> tokens)
        {
            int length = tokens.Count;
            List<List<int>> operatorIndex = new List<List<int>>();
            for (int i = 0; i < length; i++)
            {
                Token currToken = tokens[i];
                if (currToken is Operator)
                {
                    Operator currOperator = (Operator)currToken;
                    int precedence = currOperator.precedence;
                    operatorIndex.Add([precedence, i]);
                }
            }
            if (operatorIndex.Count == 0)
            {
                return 0;
            }
            operatorIndex = sortOperators(operatorIndex);
            return operatorIndex[0][1];
        }

        public double parseExpr(string expr)
        {
            List<string> tokenizedList = tokenize(expr);
            List<Token> lexList = lex(tokenizedList);
            Group wholeExpr = new Group(lexList);
            Constant result = wholeExpr.eval();
            return result.eval();
        }
    }
}
