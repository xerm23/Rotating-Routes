using DG.Tweening;
using RotatingRoutes.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace RotatingRoutes.UI
{
    public class FinishCanvas : MonoBehaviour
    {
        [SerializeField] Button _replayButton;
        CanvasGroup _canvasGroup;
        private bool _clicked;

        // Start is called before the first frame update
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            GameManager.OnGameStateChange += ShowFinalCanvas;
            _replayButton.onClick.AddListener(ResetScene);
        }

        private void ResetScene()
        {
            if (_clicked)
                return;
            AudioManager.Instance.PlayPopSound();
            _clicked = true;
            _replayButton.transform.DOPunchScale(new Vector3(.25f, .25f, .25f), .1f);
            _replayButton.transform.DORotate(new Vector3(0, 0, -180), .25f)
                .OnComplete(() => LoadingScreenController.Instance.ResetGame());

        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChange -= ShowFinalCanvas;
        }

        private void ShowFinalCanvas(GameState gameState)
        {
            if (gameState != GameState.GameCompleted && gameState != GameState.GameOver)
                return;
            _canvasGroup.DOFade(1, .25f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

    }
}