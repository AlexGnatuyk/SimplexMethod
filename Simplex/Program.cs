using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    
    public class Program
    {
        static void Main(string[] args)
        {
            var target = new Function();
            var restrictions = new List<Restriction>();
            var basis = new List<int>();
            var simplexDiff = new List<double>();

            var result = Parser.Parse(target, restrictions, "example.txt");
            Parser.ShowParse(result.target,result.restrictions);
            var tm= Simplex.Norm_to_Kanon(result.target, result.restrictions, basis);
            Parser.ShowParse(tm.target,tm.restrictions);


            Console.Read();
        }
    }
}
