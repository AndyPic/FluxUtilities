using System.Text;
using Unity.Collections;

namespace Flux.Core.Extentions
{
    public static class NativeArrayExtentions
    {
        public static string ToStringExt<T>(NativeArray<T> array) where T : struct
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