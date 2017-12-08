using System;
using System.Collections.Generic;
using System.IO;

namespace Simplex
{
    public static class Parser
    {
        public static (Function target, List<Restriction> restrictions) Parse(Function target, List<Restriction> restrictions,
            string path)
        {
            bool flag = false;
            string text = "";

            using (StreamReader fs = new StreamReader(path))
            {
                while (true)
                {
                    // Читаем строку из файла во временную переменную.
                    string temp = fs.ReadLine();
                    // Если достигнут конец файла, прерываем считывание.
                    if (temp == null) break;

                    var i = 0;
                    var line = temp.Split();
                    var sign = line[i];
                    if (flag == false)
                    {
                        target.coefficients = new List<double>();
                        while (sign != "min" && sign != "max")
                        {
                            target.coefficients.Add(Convert.ToDouble(sign));
                            i++;
                            sign = line[i];
                        }
                        target.way = sign;
                        flag = true;
                        continue;
                    }

                    Restriction tempRestriction = new Restriction();
                    i = 0;
                    line = temp.Split();
                    sign = line[i];
                    tempRestriction.coefficients=new List<double>();
                    while (sign != "<=" && sign != ">=" && sign != "=")
                    {
                        tempRestriction.coefficients.Add(Convert.ToDouble(sign));
                        i++;
                        sign = line[i];
                    }
                    tempRestriction.sign = sign;
                    tempRestriction.rvalue = Convert.ToDouble(line[i+1]);

                    restrictions.Add(tempRestriction);
                }
            }

            return (target: target, restrictions: restrictions);
        }

        public static void ShowParse(Function target, List<Restriction> restrictions)
        {
            for (int i = 0; i < target.coefficients.Count; i++)
            {
                Console.Write($"{target.coefficients[i]} ");
            }
            Console.WriteLine(target.way);

            for (int i = 0; i < restrictions.Count; i++)
            {
                for (int j = 0; j < restrictions[i].coefficients.Count; j++)
                {
                    Console.Write($"{restrictions[i].coefficients[j]} "); 
                }
                Console.WriteLine($"{restrictions[i].sign} {restrictions[i].rvalue}");
            }
            Console.WriteLine($"---------------------------------");
        }
    }
}