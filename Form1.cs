using System.Diagnostics;
using System.Windows.Forms;
using System.Numerics;
using System.Data;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsApp
{
    public partial class Form1 : Form
    {
        string dispResult = "0";
        int lineCount = 0;
        public Form1()
        {
            InitializeComponent();
        }
        public static Dictionary<string, Func<double, double>> funcDict = new Dictionary<string, Func<double, double>>(){
            {"sin", i => Math.Sin(i)},
            {"cos", i => Math.Cos(i)},
            {"tan", i => Math.Cos(i)},
            {"asin", i => Math.Asin(i)},
            {"acos", i => Math.Acos(i)},
            {"atan", i => Math.Atan(i)},
            {"sinh", i => Math.Sinh(i)},
            {"cosh", i => Math.Cosh(i)},
            {"tanh", i => Math.Cosh(i)},
            {"sqrt", i => Math.Sqrt(i)},
            {"abs", i => Math.Abs(i)},
            {"exp", i => Math.Exp(i)},
            {"log", i => Math.Log10(i)},
            {"ln", i => Math.Log(i)}
        };
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string currLine = calcConsole.Lines[lineCount];
            if (currLine == "0")
            {
                calcConsole.Lines[lineCount] = "";
            };
            Button button = (Button)sender;
            calcConsole.Text = calcConsole.Text + button.Text;
        }
        private void enter_Click(object sender, EventArgs e)
        {
            string expr = (calcConsole.Lines[lineCount].Remove(0, 4).ToLower());
            expr = Regex.Replace(expr, "ans", "(" + dispResult + ")");
            lineCount += 2;
            dispResult = new Parser().parseExpr(expr).ToString();
            calcConsole.AppendText(Environment.NewLine);
            calcConsole.AppendText(dispResult);
            calcConsole.SelectionAlignment = HorizontalAlignment.Right;
            calcConsole.AppendText(Environment.NewLine);
            calcConsole.SelectionAlignment = HorizontalAlignment.Left;
            calcConsole.AppendText(">>> ");
            calcConsole.SelectionStart = calcConsole.TextLength;
            calcConsole.ScrollToCaret();
            calcConsole.DeselectAll();
        }

        private void del_Click(object sender, EventArgs e)
        {
            var Lines = calcConsole.Lines;
            string currLine = Lines[lineCount];
            int length = currLine.Length;
            if (length > 4)
            {
                currLine = currLine.Substring(0, length - 1);
            }
            else
            {
                currLine = ">>> ";
            }
            Lines[lineCount] = currLine;
            calcConsole.Lines = Lines;
        }

        private void Ans_Click(object sender, EventArgs e)
        {
            calcConsole.Text += "Ans";
        }
        private void operator_Click(object sender, EventArgs e)
        {
            string currLine = calcConsole.Lines[lineCount];
            Button button = (Button)sender;
            switch(button.Name)
            {
                case "timesButton":
                    dispSymbol(currLine, "*");
                    break;
                default:
                    button1_Click(sender, e);
                    break;
            }
        }
        private void dispSymbol(string currLine, string symbol)
        {
            if (currLine == "0")
            {
                calcConsole.Lines[lineCount] = "";
            };
            calcConsole.Text = calcConsole.Text + symbol;
        }
        private void rtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                enter_Click(sender, e);
                e.Handled = true;
            }
        }

        private void calcConsole_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void expButton_Click(object sender, EventArgs e)
        {

        }

        private void powButton_Click(object sender, EventArgs e)
        {

        }
    }
}
