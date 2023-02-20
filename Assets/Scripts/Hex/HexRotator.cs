using DG.Tweening;
using RotatingRoutes.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    public class HexRotator : MonoBehaviour
    {
        private readonly Dictionary<Transform, float> _rotatingTweens = new();

        public void RotateHex(Transform hex)
        {
            HexTile hexTile = hex.GetComponent<HexTile>();
            if (hexTile == null || !hexTile.UsableStatus)
                return;
            AudioManager.Instance.PlayPopSound();
            hex.DOKill();
            hex.DOPunchScale(Vector3.one * .2f, .1f).OnComplete(() => hex.DOScale(1, .1f));
            hex.DOMoveY(1, .1f).OnComplete(() => hex.DOMoveY(0, .1f).SetDelay(.1f));

            _rotatingTweens.TryGetValue(hex, out float rotateGoal);
            _rotatingTweens[hex] = Mathf.Approximately(rotateGoal, 0) ? hex.transform.eulerAngles.y + 60 : rotateGoal + 60;
            hex.DORotate(new Vector3(0, _rotatingTweens[hex], 0), .05f)
               .SetDelay(.1f)
               .OnComplete(() => _rotatingTweens.Remove(hex));

        }

    }
}