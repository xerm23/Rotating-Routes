using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RotatingRoutes.Hex
{
    public class HexRaycaster : MonoBehaviour
    {
        [SerializeField] private LayerMask _hexLayerMask;
        private HexRotator _hexRotator;

        private Camera _mainCam;
        private void Awake()
        {
            _mainCam = Camera.main;
            _hexRotator = GetComponent<HexRotator>();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetHit();
            }
        }

        private void GetHit()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 15f;
            mousePos = _mainCam.ScreenToWorldPoint(mousePos);
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            if (!Physics.Raycast(ray, out RaycastHit hitinfo, 150, _hexLayerMask, QueryTriggerInteraction.Ignore))
                return;
            _hexRotator.RotateHex(hitinfo.transform);
        }
    }
}