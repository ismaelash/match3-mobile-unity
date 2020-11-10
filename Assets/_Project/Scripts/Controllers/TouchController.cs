using UnityEngine;
using Utils;

namespace IsmaelNascimento.Controllers
{
	public class TouchController : SingletonMonoBehaviour<TouchController>
	{
		public Vector3 touchPosition;
		public bool disabled = false;
		private ITouchable elementClicked;
		private Vector3 lastPosition;

		private void Update()
		{
			if (disabled)
            {
				return;
			}

#if UNITY_EDITOR || UNITY_WEBGL
		HandleInputDesktop();
#elif UNITY_ANDROID || UNITY_IOS
		HandleInputMobile();
#endif
		}

		private void HandleInputMobile()
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				touchPosition = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);

				RaycastHit2D raycastHit = Physics2D.Raycast(touchPosition, Vector3.forward, Mathf.Infinity);

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
						elementClicked = raycastHit.collider.GetComponent<ITouchable>();
					}

					if (elementClicked != null)
                    {
						elementClicked.TouchDown();
					}
				}
			}
			else
			{
				ClearElementClicked();
			}
		}

		private void HandleInputDesktop()
		{
			lastPosition = touchPosition;
			touchPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit2D raycastHit = Physics2D.Raycast(touchPosition, Vector3.forward, Mathf.Infinity);

			if (elementClicked != null)
			{
				if (Input.GetMouseButton(0))
				{
					if (lastPosition != touchPosition)
                    {
						elementClicked.TouchDrag();
					}
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
					elementClicked = raycastHit.collider.GetComponent<ITouchable>();
				}

				if (elementClicked != null)
                {
					elementClicked.TouchDown();
				}
			}
		}

		public void ClearElementClicked()
		{
			Instance.elementClicked = null;
		}
	}
}