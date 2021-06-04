using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace S_Team
{
    class CustomList<T> : IEnumerable<T>
    {
        private T[] arr;
        private static int InitialCapacity = 4;
        private int count;

        public int Count
        {
            get { return count; }
            private set { count = value; }
        }

        public CustomList()
        {
            this.arr = new T[InitialCapacity];
            this.count = 0;
        }

        public void Add(T item)
        {
            if (count == arr.Length)
            {
                Resize();
            }
            arr[this.count] = item;
            this.count++;
        }

        private void Resize()
        {
            T[] copy = new T[arr.Length * 2];
            Array.Copy(arr, copy, arr.Length);
            arr = copy;
        }

        public void Clear()
        {
            arr = new T[InitialCapacity];
            count = 0;
        }

        public int Find(T value)
        {
            int index = -1;

            for (int i = 0; i < count; i++)
            {
                if (arr[i].Equals(value))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public bool Remove(T value)
        {
            int index = Find(value);

            if (index >= 0 && index < count)
            {
                RemoveAt(index);
                return true;
            }
            else if (index == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Game not present in your library!");
            }
            return false;
        }

        public bool RemoveAt(int index)
        {
            if (index >= 0 && index < count)
            {
                for (int i = index; i < count; i++)
                {
                    if (i != count - 1)
                    {
                        arr[i] = arr[i + 1];
                    }
                    else
                    {
                        arr[i] = default;
                    }
                }
                count--;
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Index not present in library!");
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < count)
                {
                    return arr[index];
                }
                else
                {
                    throw new IndexOutOfRangeException("Index out of bounds");
                }
            }
            set
            {
                if (index >= 0 && index < count)
                {
                    arr[index] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException("Index out of bounds");
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return arr[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
