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

			for (int i = 0; i < arr.GetLength(0); ++i)
			{
				for (int j = 0; j < arr.GetLength(1); ++j)
				{
					list.Add(arr[i, j]);
				}
			}

			return list;
		}

		public static float GetCurrentStateDuration(this Animator animator, int layerIndex = 0)
		{
			animator.Update(0);
			AnimatorClipInfo[] animatorClipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
			float speedMultiplier = animator.GetCurrentAnimatorStateInfo(0).speed;

			if (animatorClipInfos.Length == 0 || speedMultiplier == 0)
            {
				return 0;
			}

			float duration = animatorClipInfos[0].clip.length;
			return duration / Mathf.Abs(speedMultiplier);
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