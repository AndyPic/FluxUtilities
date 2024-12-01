using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Flux.Core.Collections
{
    public class SparseList<T>
    {
        public List<T> Data { get; }
        public List<bool> Mask { get; }

        private readonly Stack<int> emptyIndices;

        public int Length => Data.Count;
        public int Count => Length - emptyIndices.Count;

        public bool HasGaps
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => emptyIndices.Count > 0;
        }

        public SparseList(int initialCapacity = 256)
        {
            Data = new(initialCapacity);
            Mask = new(initialCapacity);

            emptyIndices = new();
        }

        public T GetNextEmpty(out int index)
        {
            index = emptyIndices.Pop();
            Mask[index] = true;
            return Data[index];
        }

        public int Add(T toAdd)
        {
            int index;

            if (HasGaps)
            {
                index = emptyIndices.Pop();

                Data[index] = toAdd;
                Mask[index] = true;
            }
            else
            {
                index = Length;

                Data.Add(toAdd);
                Mask.Add(true);
            }

            return index;
        }

        public void RemoveAt(int index)
        {
            Mask[index] = false;
            emptyIndices.Push(index);
        }

        public bool CanRemove(int index)
        {
            return index >= 0 && index < Length && Mask[index];
        }

        public void RemoveAt_CheckValid(int index)
        {
            if (!CanRemove(index))
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid index or index empty.");

            Mask[index] = false;
            emptyIndices.Push(index);
        }
    }
}
