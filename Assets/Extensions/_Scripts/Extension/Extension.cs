using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

namespace _Scripts.Extension
{
    public static class Extension
    {
        //shuffle arrays:
        public static void Shuffle<T>(this T[] array, int shuffleAccuracy)
        {
            for (int i = 0; i < shuffleAccuracy; i++)
            {
                int randomIndex = UnityEngine.Random.Range(1, array.Length);

                (array[randomIndex], array[0]) = (array[0], array[randomIndex]);
            }
        }
        //shuffle 2D arrays:
        public static void Shuffle<T>(this T[,] array)
        {
            var random = new Random();
            int lengthRow = array.GetLength(1);

            for (int i = array.Length - 1; i > 0; i--)
            {
                int i0 = i / lengthRow;
                int i1 = i % lengthRow;

                int j = random.Next(i + 1);
                int j0 = j / lengthRow;
                int j1 = j % lengthRow;

                (array[i0, i1], array[j0, j1]) = (array[j0, j1], array[i0, i1]);
            }
        }

        //shuffle lists:
        public static void Shuffle<T>(this List<T> list, int shuffleAccuracy)
        {
            for (int i = 0; i < shuffleAccuracy; i++)
            {
                int randomIndex;
                randomIndex = UnityEngine.Random.Range(list.Count, 1);
                randomIndex = UnityEngine.Random.Range(1, list.Count);

                (list[randomIndex], list[0]) = (list[0], list[randomIndex]);
            }
        }

        //get random element
        public static T GetRandom<T>(this List<T> list)
        {
            int randomIndex;
            randomIndex = UnityEngine.Random.Range(0, list.Count);
            randomIndex = UnityEngine.Random.Range(0, list.Count);

            return list[randomIndex];
        }
        
        public static T GetComponentT<T>(this Component compopent)
        {
            if (compopent == null) return default(T);
            var t = compopent.GetComponent<T>();
            if (t == null && compopent.transform.parent != null)
                return GetComponentT<T>(compopent.transform.parent);

            return t;
        }
    }
}