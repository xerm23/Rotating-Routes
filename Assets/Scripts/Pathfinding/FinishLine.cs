using DG.Tweening;
using RotatingRoutes.Hex;
using RotatingRoutes.Managers;
using UnityEngine;

namespace RotatingRoutes.Pathfinding
{
    public class FinishLine : MonoBehaviour
    {
        [SerializeField] GameObject WallGatePrefab;
        [SerializeField] GameObject CastlePrefab;
        Transform _playerTransform;
        public void InvokeGameFinished(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            Instantiate(WallGatePrefab, transform);
            TweenAndMove();
        }

        void TweenAndMove()
        {
            Transform hex = GetComponentInParent<HexTile>().transform;

            hex.DOKill();
            hex.DOPunchScale(Vector3.one * .2f, .1f).OnComplete(() => hex.DOScale(1, .1f));
            hex.DOMoveY(1, .2f).OnComplete(() => hex.DOMoveY(0, .1f).SetDelay(.1f));
            hex.DORotate(new Vector3(0, 180, 0), .25f)
               .SetDelay(.2f)
               .OnComplete(() => MovePlayerToFinish());

        }

        void MovePlayerToFinish()
        {
            var castle = Instantiate(CastlePrefab, transform.parent);
            castle.transform.localPosition = new Vector3(0, -1, 7);
            castle.transform.DOMoveY(1, .2f);
            castle.transform.DOPunchScale(Vector3.one * .2f, .1f).SetDelay(.1f).OnComplete(() => castle.transform.DOScale(1, .1f));

            _playerTransform.DOMove(transform.GetChild(0).position, 1.5f).SetEase(Ease.OutCubic).OnComplete(() => GameManager.GameFinished());
        }



    }
}