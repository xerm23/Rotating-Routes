using RotatingRoutes.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace RotatingRoutes.UI
{
    public class StartArrow : MonoBehaviour
    {
        [SerializeField] private bool left;
        [SerializeField] private CanvasGroup _otherArrow;
        private bool _clicked;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            GetComponent<Button>().onClick.AddListener(() => StartGame(left));
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.DOScale(1.25f, 1).SetLoops(-1, LoopType.Yoyo);
            _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 25, 1).SetLoops(-1, LoopType.Yoyo);
        }

        private void StartGame(bool left)
        {
            if (_clicked)
                return;
            _clicked = true;

            AudioManager.Instance.PlayPopSound();
            _otherArrow.DOFade(0, .1f);
            _rectTransform.DOKill();
            _rectTransform.DOAnchorPosX(0, .5f);
            _rectTransform.DOScale(1.25f, .1f).SetDelay(.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y - 25, .1f);
                _canvasGroup.DOFade(0, .2f).OnComplete(() => Destroy(transform.parent.gameObject));
                GameManager.GameStarted(left ? StartSide.Left : StartSide.Right);
            });
        }
    }
}