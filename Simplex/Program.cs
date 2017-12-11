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
            var run = true;
            var simplexDiff = new List<double>();

            Parser.Parse(ref target,ref restrictions, "example.txt");
            Parser.ShowParse(target,restrictions);
            Simplex.Norm_to_Kanon(ref target, ref restrictions, ref basis);
            Parser.ShowParse(target,restrictions);

            while (run)
            {
                Simplex.ShowTable(ref target, ref restrictions, ref basis, ref simplexDiff, ref run);
                Simplex.UpdateTable(ref restrictions, ref basis, ref simplexDiff, true);
            }


            Console.Read();
        }
    }
}
