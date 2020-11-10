using UnityEngine;
using Utils;

namespace IsmaelNascimento.Controllers
{
	public class TouchController : Singleton<TouchController>
	{
		#region VARIABLES

		// Privates
		private ITouchable elementClicked;
		private Vector3 lastPosition;

		// Properties
		public Vector3 TouchPosition { get; set; }
		public bool IsDisabled { get; set; }

		#endregion

		#region MONOBEHAVIOUR_METHODS

		private void Update()
		{
			if (IsDisabled)
			{
				return;
			}

#if UNITY_EDITOR || UNITY_WEBGL
			HandleInputDesktop();
#elif UNITY_ANDROID || UNITY_IOS
		HandleInputMobile();
#endif
		}

		#endregion

		#region PUBLIC_METHODS

		public void ClearElementClicked()
		{
			Instance.elementClicked = null;
		}

		#endregion

		#region PRIVATE_METHODS

		private void HandleInputMobile()
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				TouchPosition = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
				RaycastHit2D raycastHit = Physics2D.Raycast(TouchPosition, Vector3.forward, Mathf.Infinity);

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
			lastPosition = TouchPosition;
			TouchPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D raycastHit = Physics2D.Raycast(TouchPosition, Vector3.forward, Mathf.Infinity);

			if (elementClicked != null)
			{
				if (Input.GetMouseButton(0))
				{
					if (lastPosition != TouchPosition)
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

		#endregion
	}
}