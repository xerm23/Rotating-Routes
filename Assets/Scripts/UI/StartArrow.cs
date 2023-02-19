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

        RectTransform _rectTransform;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => StartGame(left));
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.DOScale(1.25f, 1).SetLoops(-1, LoopType.Yoyo);
            _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 25, 1).SetLoops(-1, LoopType.Yoyo);
        }

        private void StartGame(bool left)
        {
            transform.parent.gameObject.SetActive(false);
            GameManager.GameStarted(left ? StartSide.Left : StartSide.Right);
        }
    }
}