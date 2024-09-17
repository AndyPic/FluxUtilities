using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Flux.Core.Extensions
{
    public class UnsafeListExtentions : MonoBehaviour
    {
        public static string ToStringExt<T>(UnsafeList<T> list) where T : unmanaged
        {
            var sb = new StringBuilder();

            sb.Append("[");
            for (int i = 0; i < list.Length; i++)
            {
                sb.Append(list[i].ToString());

                if (i < list.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}