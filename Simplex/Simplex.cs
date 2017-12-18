using System;
using System.Collections.Generic;

namespace Simplex
{
    public static class Simplex
    {
        public static  void Norm_to_Kanon(ref Function target,ref List<Restriction> restrictions, ref List<int> basis)
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
                   
                    restrictions[i].sign = "=";
                }
            }
        }

        public static void ShowTable(ref Function target,ref List<Restriction> restrictions,ref List<int> basis,ref List<double> simplex_diff, ref bool run)
        {
            
           Console.Write($"Basis:    ");
            for (int i = 0; i < target.coefficients.Count; i++)
            {
                Console.Write($"{target.coefficients[i]} ");
            }
            Console.WriteLine("\n---------------------------------\n");
            Console.Write("               B  ");

            for (int i = 0; i < target.coefficients.Count; i++)
            {
                Console.Write($"A{i+1} ");
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
                Console.WriteLine("\n---------------------------------" );
            }
            Console.Write("     Z=");

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
                run = false;

            }

            // Проверка на неограниченность функции сверху
            for (int i = 0; i < simplex_diff.Count; ++i)
            {
                if (simplex_diff[i] <= 0)
                {
                    if (Is_Unbounded_Function(restrictions, i))
                    {
                        Console.WriteLine("Функция неограниченна сверху" );
                        
                    }
                }
            }
        }

        public  static bool Is_Unbounded_Function(List<Restriction> restrictions, int i)
        {
            int minus_cnt = 0;
            for (int j = 0; j < restrictions.Count; j++)
            {
                if (restrictions[j].coefficients[i] <= 0)
                    minus_cnt++;
            }
            if (minus_cnt == restrictions.Count)
                return true;
            else return false;
        }

        public  static void UpdateTable(ref List<Restriction> restrictions,ref List<int> basis,ref List<double> simplex_diff, bool blender)
        {
            // Поиск минимальной симплексной разности
            int ind_min_diff = 0;
            double tmp_diff = simplex_diff[0];
            if (blender)
            {
                for (int i = 0; i < simplex_diff.Count; i++)
                {
                    tmp_diff = simplex_diff[i];
                    if (tmp_diff < 0)
                    {
                        ind_min_diff = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 1; i < simplex_diff.Count; i++)
                {
                    if (simplex_diff[i] < tmp_diff)
                    {
                        tmp_diff = simplex_diff[i];
                        ind_min_diff = i;
                    }
                }
            }

            // Поиск направляющего элемента
            int ind_min_restr = 0;
            double tmp_restr = 0;

            // Ищем первый положительный элемент столбца, чтобы присвоить начальные значения
            for (int i = 0; i < restrictions.Count; i++)
            {
                if (restrictions[i].coefficients[ind_min_diff] > 0)
                {
                    tmp_restr = restrictions[i].rvalue / restrictions[i].coefficients[ind_min_diff];
                    ind_min_restr = i;
                    break;
                }
            }

            // Начиная с него ищем направляющий элемент
            for (int i = ind_min_restr; i < restrictions.Count; i++)
            {
                if ((restrictions[i].coefficients[ind_min_diff] > 0) && (restrictions[i].rvalue / restrictions[i].coefficients[ind_min_diff] < tmp_restr))
                {
                    ind_min_restr = i;
                    tmp_restr = restrictions[i].rvalue / restrictions[i].coefficients[ind_min_diff];
                }
            }

            // Меянем базис
            basis[ind_min_restr] = ind_min_diff + 1;

            // Копируем элементы rvalue во временный вектор
            List<double> tmp_rvalue = new List<double>();
            for (int i = 0; i < restrictions.Count; i++)
            {
                tmp_rvalue.Add(restrictions[i].rvalue);
            }


            double guiding = restrictions[ind_min_restr].coefficients[ind_min_diff]; // Значение направляющего элемента
            // Меняем остальные коэф-ты
            for (int i = 0; i < restrictions.Count; i++)
            {
                if (i != ind_min_restr)
                {
                    // Копируем строку
                    List<double> tmp = new List<double>(); // Временный вектор для значений изменяемой строки
                    for (int k = 0; k < restrictions[i].coefficients.Count; k++)
                    {
                        tmp.Add(restrictions[i].coefficients[k]);
                    }

                    // Обновляем строку
                    for (int j = 0; j < restrictions[i].coefficients.Count; j++)
                    {
                        restrictions[i].coefficients[j] = tmp[j] - restrictions[ind_min_restr].coefficients[j] * tmp[ind_min_diff] / guiding;
                    }

                    // Меняем правую часть
                    restrictions[i].rvalue = tmp_rvalue[i] - restrictions[ind_min_restr].rvalue * tmp[ind_min_diff] / guiding;
                    tmp.Clear();
                }
            }

            // Меняем направляющую строку
            for (int i = 0; i < restrictions[ind_min_restr].coefficients.Count; i++)
            {
                restrictions[ind_min_restr].coefficients[i] /= guiding;
            }
            restrictions[ind_min_restr].rvalue /= guiding;
            tmp_rvalue.Clear();
        }
    }
}