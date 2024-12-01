using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Flux.Core.Collections
{
    // todo - make an actual burst compatible version

    public class SparseNativeList<T> : IDisposable where T : unmanaged
    {
        public NativeList<T> Data { get; private set; }
        public NativeList<bool> Mask { get; private set; }

        private readonly Stack<int> emptyIndices;

        public int Length => Data.Length;
        public int Count => Length - emptyIndices.Count;

        public SparseNativeList(int initialCapacity = 256)
        {
            Data = new(initialCapacity, Allocator.Persistent);
            Mask = new(initialCapacity, Allocator.Persistent);

            emptyIndices = new();
        }

        public void Dispose()
        {
            if (Data.IsCreated) { Data.Dispose(); }
            if (Mask.IsCreated) { Mask.Dispose(); }
        }

        public int Add(T toAdd)
        {
            int index;

            if (emptyIndices.Count > 0)
            {
                index = emptyIndices.Pop();

                unsafe
                {
                    void* dataPtr = Data.GetUnsafePtr();
                    UnsafeUtility.WriteArrayElement(dataPtr, index, toAdd);

                    void* maskPtr = Mask.GetUnsafePtr();
                    UnsafeUtility.WriteArrayElement(maskPtr, index, true);
                }
            }
            else
            {
                Data.Add(toAdd);
                Mask.Add(true);

                index = Length - 1;
            }

            return index;
        }

        public void RemoveAt(int index)
        {
            unsafe
            {
                void* maskPtr = Mask.GetUnsafePtr();
                UnsafeUtility.WriteArrayElement(maskPtr, index, false);
            }

            emptyIndices.Push(index);
        }
    }
}