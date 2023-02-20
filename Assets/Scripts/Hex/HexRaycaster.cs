using RotatingRoutes.Managers;
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
            if (Input.GetMouseButtonDown(0) && (GameManager.CurrentState == GameState.GameStarted
                                                || GameManager.CurrentState == GameState.Pregame))
            {
                GetHit();
            }
        }

        private void GetHit()
        {
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitinfo, 150, _hexLayerMask, QueryTriggerInteraction.Ignore))
                return;
            _hexRotator.RotateHex(hitinfo.transform);
        }
    }
}