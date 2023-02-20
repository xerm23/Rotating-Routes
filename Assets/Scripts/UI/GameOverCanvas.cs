using DG.Tweening;
using RotatingRoutes.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace RotatingRoutes.UI
{
    public class GameOverCanvas : MonoBehaviour
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
            _clicked = true;

            _replayButton.transform.DORotate(new Vector3(0, 0, -180), .25f)
                .OnComplete(() => LoadingScreenController.Instance.ResetGame());
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChange -= ShowFinalCanvas;
        }

        private void ShowFinalCanvas(GameState gameState)
        {
            if (gameState != GameState.GameOver)
                return;
            _canvasGroup.DOFade(1, .25f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

    }
}