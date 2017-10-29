// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;

namespace Xavalon.XamlStyler.Core.Extensions
{
    public static class ListExtensions
    {
        public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }

            int count = list.Count;

            for (int j = 1; j < count; j++)
            {
                T key = list[j];
                int i = j - 1;

                for (; (i >= 0) && (comparison(list[i], key) > 0); i--)
                {
                    list[i + 1] = list[i];
                }

                list[i + 1] = key;
            }
        }
    }
}
