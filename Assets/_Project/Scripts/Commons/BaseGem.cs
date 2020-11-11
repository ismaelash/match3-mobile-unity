using Match3.Controllers;
using Match3.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Match3.Commons
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BaseGem : MonoBehaviour, ITouchable
    {
        #region VARIABLES

        [HideInInspector] public GemData gemData = new GemData();

        private Coroutine moveToCoroutine = null;
        private SpriteRenderer spriteRenderer;

        public Func<BaseGem, bool> ValidateGem
        {
            get => gem => gem.gemData.type == gemData.type;
        }

        #endregion

        #region MONOBEHAVIOUR_METHODS
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        #endregion

        #region PUBLIC_METHODS

        public MatchInfo GetMatch()
        {
            return BoardController.Instance.GetCrossMatch(this, ValidateGem);
        }

        public void SetType(GemData gemData)
        {
            this.gemData.type = gemData.type;
            spriteRenderer.sprite = gemData.sprite;
            this.gemData.minMatch = gemData.minMatch;
        }

        public void SetPosition(Vector2Int position)
        {
            gemData.position = position;
            BoardController.Instance.GemBoard[position.x, position.y] = this;
        }

        public float MoveTo(Vector3 target, float speed, float delay = 0)
        {
            if (moveToCoroutine != null)
            {
                StopCoroutine(moveToCoroutine);
            }

            moveToCoroutine = StartCoroutine(MoveTo_Coroutine(target, speed, delay));
            return ((target - transform.position).magnitude / speed) + delay;
        }

        public void TouchDrag()
        {
            if (Vector2.Distance(transform.position, TouchController.Instance.TouchPosition) > .75f)
            {
                Vector2 delta = TouchController.Instance.TouchPosition - transform.position;
                BaseGem otherGem;

                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    int swapX = (int)(gemData.position.x + Mathf.Sign(delta.x));
                    otherGem = BoardController.Instance.GetGem(swapX, gemData.position.y);
                }
                else
                {
                    int swapY = (int)(gemData.position.y + Mathf.Sign(delta.y));
                    otherGem = BoardController.Instance.GetGem(gemData.position.x, swapY);
                }

                if (otherGem)
                {
                    BoardController.Instance.TryMatch(this, otherGem);
                    SoundController.Instance.PlaySfx(GameController.Instance.GameData.GetAudioClip(Constants.swapSfxAudioclipName));
                }

                TouchUp();
            }
        }

        public void TouchUp()
        {
            TouchController.Instance.ClearElementClicked();
        }

        #endregion

        #region COROUTINES

        private IEnumerator MoveTo_Coroutine(Vector3 target, float speed, float delay)
        {
            yield return new WaitForSeconds(delay);
            float distance = (target - transform.position).magnitude;

            while (!Mathf.Approximately(0.0f, distance))
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
                distance = (target - transform.position).magnitude;
            }

            transform.position = target;
        }

        #endregion
    }
}