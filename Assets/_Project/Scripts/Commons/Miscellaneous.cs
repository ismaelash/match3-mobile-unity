using System.Collections.Generic;
using UnityEngine;

namespace Match3.Commons
{
    public static class Miscellaneous
    {
		#region PUBLIC_METHODS

		public static List<T> GetList<T>(this T[,] arr)
		{
			List<T> list = new List<T>();

			for (int index1 = 0; index1 < arr.GetLength(0); ++index1)
			{
				for (int index2 = 0; index2 < arr.GetLength(1); ++index2)
				{
					list.Add(arr[index1, index2]);
				}
			}

			return list;
		}

		public static T Choose<T>(List<T> chances)
		{
			return Choose(chances.ToArray());
		}

		public static T[,] ShuffleMatrix<T>(T[,] arr)
		{
			int m = arr.GetLength(0);
			int n = arr.GetLength(1);

			for (int i = m * n - 1; i > 0; --i)
			{
				int j = Random.Range(0, i + 1);

                T temp = arr[i / n, i % n];
				arr[i / n, i % n] = arr[j / n, j % n];
				arr[j / n, j % n] = temp;
			}

			return arr;
		}

		public static void SetCameraOrthographicSizeByWidth(Camera camera, float width)
		{
			camera.orthographicSize = width / camera.aspect / 2;
		}

		#endregion

		#region PRIVATE_METHODS

		// Get a random element
		private static T Choose<T>(T[] chances)
		{
			return chances[Random.Range(0, chances.Length)];
		}

		#endregion
	}
}