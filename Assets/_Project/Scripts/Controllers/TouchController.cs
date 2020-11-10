using UnityEngine;
using Utils;

namespace IsmaelNascimento.Controllers
{
	public class TouchController : SingletonMonoBehaviour<TouchController>
	{
		public static Vector3 touchPosition;
		public static bool disabled = false;
		ITouchable elementClicked;

		void Update()
		{
			if (disabled) return;

#if UNITY_EDITOR || UNITY_WEBGL
			HandleInputDesktop();
#elif UNITY_ANDROID || UNITY_IOS
		HandleInputMobile();
#endif
		}

		void HandleInputMobile()
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				touchPosition = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);

				RaycastHit2D raycastHit = Physics2D.Raycast(
					touchPosition, Vector3.forward, Mathf.Infinity
				);

				if (elementClicked != null)
				{
					switch (touch.phase)
					{
						case TouchPhase.Moved:
							elementClicked.TouchDrag();
							break;
						case TouchPhase.Ended:
							elementClicked.TouchUp();
							elementClicked = null;
							break;
					}
				}
				else if (touch.phase == TouchPhase.Began)
				{
					if (raycastHit)
					{
						elementClicked = raycastHit.collider
										.GetComponent<ITouchable>();
					}
					if (elementClicked != null)
						elementClicked.TouchDown();
				}
			}
			else
			{
				ClearElementClicked();
			}
		}

#if UNITY_EDITOR || UNITY_WEBGL
		Vector3 lastPosition;
		void HandleInputDesktop()
		{
			lastPosition = touchPosition;
			touchPosition = (Vector2)Camera.main
							.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit2D raycastHit = Physics2D.Raycast(
				touchPosition, Vector3.forward, Mathf.Infinity
			);

			if (elementClicked != null)
			{
				if (Input.GetMouseButton(0))
				{
					if (lastPosition != touchPosition)
						elementClicked.TouchDrag();
				}
				else
				{
					elementClicked.TouchUp();
					elementClicked = null;
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				if (raycastHit)
				{
					elementClicked = raycastHit.collider
									.GetComponent<ITouchable>();
				}

				if (elementClicked != null)
					elementClicked.TouchDown();

			}
		}

#endif

		//public static void ClearElementClicked(ITouchable other)
		//{
		//	if (Instance.elementClicked == other)
		//	{
		//		ClearElementClicked();
		//	}
		//}

		public static void ClearElementClicked()
		{
			Instance.elementClicked = null;
		}
	}
}