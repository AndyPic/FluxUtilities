using System;
using System.Text;

namespace Flux.Core.Extensions
{
    public static class ArrayExtentions
    {
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns>
        /// True, if <paramref name="index"/> is a valid index for <paramref name="array"/>.
        /// </returns>
        public static bool IsValidIndex(this Array array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns>
        /// The index of <paramref name="item"/> in <paramref name="array"/>, if present.
        /// If not found, returns -1.
        /// </returns>
        public static int IndexOf<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item);
        }

        public static string ToStringExt<T>(this T[] array)
        {
            var sb = new StringBuilder();

            sb.Append("[");
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i].ToString());

                if (i < array.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}
