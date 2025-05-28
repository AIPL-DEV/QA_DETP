using System;
using System.Data;

namespace DETP.helpers
{
    public class EvalUtils
    {
        public static double Eval(String expression)
        {
            DataTable table = new System.Data.DataTable();
            return Convert.ToDouble(table.Compute(expression, String.Empty));
        }
    }
}
