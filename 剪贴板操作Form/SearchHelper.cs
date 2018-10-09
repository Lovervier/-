using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BestString
{
    public static class SearchHelper
    {
        private const int sizex = 5000;

        private const int sizey = 100;

        private static readonly Dictionary<int, int[,]> _cPool = new Dictionary<int, int[,]>();

        private static readonly bool[] _emptyFlags = new bool[sizex];

        private static readonly Dictionary<int, bool[]> _flagsPool = new Dictionary<int, bool[]>();

        private static readonly object _syncRoot = new object();

        private static HashSet<int> _flags = new HashSet<int>();

        /// <summary>
        /// 计算两个字符串之间的编辑距离。 编辑距离，是指一个字符串，每次只能通过插入一个字符、删除一个字符或者修改一个字符的方法，变成另外一个字符串的最少操作次数。
        /// </summary>
        /// <param name="str1"> 字符串1 </param>
        /// <param name="str2"> 字符串2 </param>
        /// <returns> 编辑距离 </returns>
        private static int Distance(string str1, string str2)
        {
            int n = str1.Length;
            int m = str2.Length;
            int[,] C = new int[n + 1, m + 1];
            int i, j, x, y, z;
            for (i = 0; i <= n; i++)
                C[i, 0] = i;
            for (i = 1; i <= m; i++)
                C[0, i] = i;
            for (i = 0; i < n; i++)
                for (j = 0; j < m; j++)
                {
                    x = C[i, j + 1] + 1;
                    y = C[i + 1, j] + 1;
                    if (str1[i] == str2[j])
                        z = C[i, j];
                    else
                        z = C[i, j] + 1;
                    C[i + 1, j + 1] = Math.Min(Math.Min(x, y), z);
                }
            return C[n, m];
        }

        private static void GetArrayFromPool(out bool[] flags, out int[,] C, int length)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            lock (_syncRoot)
            {
                if (_flags.Contains(id))
                {
                    flags = _flagsPool[id];
                    C = _cPool[id];
                    Array.Copy(_emptyFlags, flags, length);
                }
                else
                {
                    _flagsPool[id] = flags = new bool[sizex];
                    _cPool[id] = C = new int[sizex, sizey];
                    _flags.Add(id);
                }
            }
        }

        /// <summary>
        /// 字符串相似度算法(计算两个字符串之间的编辑距离)。
        /// </summary>
        /// <param name="sentence">  </param>
        /// <param name="words">    多个关键字。长度必须大于0，必须按照字符串长度升序排列。 </param>
        /// <returns>  </returns>
        private static int Mul_LnCS_Length(string sentence, string[] words)
        {
            int sLength = sentence.Length;
            int result = sLength;
            bool[] flags = new bool[sLength];
            int[,] C = new int[sLength + 1, words[words.Length - 1].Length + 1];
            //GetArrayFromPool(out flags, out C, sLength);
            foreach (string word in words)
            {
                int wLength = word.Length;
                int first = 0, last = 0;
                int i = 0, j = 0, LCS_L;
                //foreach 速度会有所提升，还可以加剪枝
                for (i = 0; i < sLength; i++)
                {
                    for (j = 0; j < wLength; j++)
                    {
                        if (sentence[i] == word[j])
                        {
                            C[i + 1, j + 1] = C[i, j] + 1;
                            if (first < C[i, j])
                            {
                                last = i;
                                first = C[i, j];
                            }
                        }
                        else
                            C[i + 1, j + 1] = Math.Max(C[i, j + 1], C[i + 1, j]);
                    }
                }
                LCS_L = C[i, j];
                if (LCS_L <= wLength >> 1)
                    return -1;

                while (i > 0 && j > 0)
                {
                    if (C[i - 1, j - 1] + 1 == C[i, j])
                    {
                        i--;
                        j--;
                        if (!flags[i])
                        {
                            flags[i] = true;
                            result--;
                        }
                        first = i;
                    }
                    else if (C[i - 1, j] == C[i, j])
                        i--;
                    else// if (C[i, j - 1] == C[i, j])
                        j--;
                }

                if (LCS_L <= (last - first + 1) >> 1)
                    return -1;
            }

            return result;
        }

        public static string[] Search(string param, string[] items)
        {
            if (string.IsNullOrWhiteSpace(param) || items == null || items.Length == 0)
                return new string[0];

            string[] words = param
                                .Split(new char[] { ' ', '\u3000' }, StringSplitOptions.RemoveEmptyEntries)
                                .OrderBy(s => s.Length)
                                .ToArray();

            var q = from sentence in items.AsParallel()
                    let MLL = Mul_LnCS_Length(sentence, words)
                    where MLL >= 1
                    orderby (MLL + 0.5) / sentence.Length, sentence
                    select sentence;

            return q.ToArray();
        }
    }
}