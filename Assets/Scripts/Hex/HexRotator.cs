using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    public class HexRotator : MonoBehaviour
    {
        private readonly Dictionary<Transform, Tween> _rotatingTweens = new();

        public void RotateHex(Transform hex)
        {
            if (_rotatingTweens.TryGetValue(hex, out Tween rotatingTween))
            {
                rotatingTween.Complete();
            }

            hex.DOKill();

            hex.DOPunchScale(Vector3.one * .1f, .1f).OnComplete(() => hex.DOScale(1, .1f));
            hex.DOMoveY(1, .15f).OnComplete(() => hex.DOMoveY(0, .1f).SetDelay(.1f));
            Tween rotateTween = hex.DORotate(hex.rotation.eulerAngles + new Vector3(0, 60, 0), .1f).SetDelay(.15f).OnComplete(() => _rotatingTweens.Remove(hex));
            _rotatingTweens.Add(hex, rotateTween);
        }

    }
}