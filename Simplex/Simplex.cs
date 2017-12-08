using System;
using System.Collections.Generic;

namespace Simplex
{
    public static class Simplex
    {
        public static (Function target, List<Restriction> restrictions, List<int> basis) Norm_to_Kanon(Function target, List<Restriction> restrictions, List<int> basis)
        {
            const double EPS = 0.0001;
            if (target.way == "min")
            {
                for (int i = 0; i < target.coefficients.Count; i++)
                {
                    if ((target.coefficients[i] > EPS) || (target.coefficients[i] < EPS))
                        target.coefficients[i] *= -1;
                }
                target.way = "max";
            }

            for (int i = 0; i < restrictions.Count; i++)
            {
                if (restrictions[i].sign == ">=")
                {
                    for (int j = 0; j < restrictions[i].coefficients.Count; j++)
                    {
                        if ((restrictions[i].coefficients[j] > EPS) || (restrictions[i].coefficients[j] < EPS))
                        {
                            restrictions[i].coefficients[j] *= -1;
                        }
                    }
                    if ((restrictions[i].rvalue > EPS) || (restrictions[i].rvalue < EPS))
                    {
                        restrictions[i].rvalue *= -1;
                    }
                    restrictions[i].sign = "<=";
                }

                if (restrictions[i].sign == "<=")
                {

                    target.coefficients.Add(0);
                    restrictions[i].coefficients.Add(1);
                    basis.Add(restrictions[i].coefficients.Count);
                    for (int k = 0; k < restrictions.Count; k++)
                    {
                        if (i != k)
                        {
                            restrictions[k].coefficients.Add(0);
                        }
                    }
                    // Если rvalue отрицательно
                    /*if (restrictions[i].rvalue < -0.0001)
                    {
                        for (int j = 0; j < restrictions[i].coefficients.size(); j++)
                        {
                            restrictions[i].coefficients[j] *= -1;
                        }
                        restrictions[i].rvalue *= -1;
                    }*/
                    restrictions[i].sign = "=";
                }
            }
            return (target: target, restrictions: restrictions,basis: basis);

        }

        public static ShowTable(Function target, List<Restriction> restrictions, List<int> basis, List<double> simplex_diff)
        {
            
           Console.Write($"Basis:");
            for (int i = 0; i < target.coefficients.Count; i++)
            {
                Console.Write($"{target.coefficients[i]} ");
            }
            Console.WriteLine("---------------------------------");
            Console.Write("               B  ");

            for (int i = 0; i < target.coefficients.Count; i++)
            {
                Console.Write($"A {i+1} ");
            }
            Console.WriteLine();
            Console.WriteLine("---------------------------------");
            for (int i = 0; i < restrictions.Count; i++)
            {
              Console.Write($"  x {basis[i]}    {target.coefficients[basis[i]-1]}   {restrictions[i].rvalue}"); 
                for (int j = 0; j < restrictions[i].coefficients.Count; j++)
                {
                    if (restrictions[i].coefficients[j] < 0)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write("  ");
                        Console.Write(restrictions[i].coefficients[j]);
                    }
                    
                }
                Console.WriteLine("---------------------------------\n" );
            }
            Console.WriteLine("         Z=");

            // Считаем значение целевой функции в точке
            double fun_value = 0;
            for (int i = 0; i < basis.Count; i++)
            {
                fun_value += target.coefficients[basis[i] - 1] * restrictions[i].rvalue;
            }
            Console.Write(fun_value);

            // Считаем симплекс-разности
            simplex_diff.Clear();
            int diff_cnt = 0; // Счетчик отрицательных симплекс-разностей
            for (int i = 0; i < target.coefficients.Count; i++)
            {
                double tmp = 0;
                for (int j = 0; j < basis.Count; j++)
                {
                    tmp += target.coefficients[basis[j] - 1] * restrictions[j].coefficients[i];
                }
                tmp -= target.coefficients[i];
                simplex_diff.Add(tmp);
                if (simplex_diff[i] >= 0)
                {
                    Console.Write("  ");
                    diff_cnt++;
                }
                else
                {
                    Console.Write(" ");
                }
                Console.Write(simplex_diff[i]);
            }
            Console.WriteLine("\n---------------------------------\n\n");

            // Проверка на оптимальность решения
            if (diff_cnt == simplex_diff.Count)
            {
                Console.Write("Решение X(");
                double[] x = new double[target.coefficients.Count];
                for (int i = 0; i < basis.Count; i++)
                {
                    x[basis[i] - 1] = restrictions[i].rvalue;
                }
                for (int i = 0; i < target.coefficients.Count - 1; i++)
                {
                    Console.Write($"{x[i]}, ");
                }
                Console.Write(x[target.coefficients.Count - 1]);
                Console.WriteLine(") оптимально.");


                system("PAUSE");
                exit(0);
            }

            // Проверка на неограниченность функции сверху
            for (int i = 0; i < simplex_diff.size(); ++i)
            {
                if (simplex_diff[i] <= 0)
                {
                    if (Is_Unbounded_Function(restrictions, i))
                    {
                        cout << "Функция неограниченна сверху" << endl;
                        system("PAUSE");
                        exit(0);
                    }
                }
            }
        }
    }
}