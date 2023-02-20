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

        // Start is called before the first frame update
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            GameManager.OnGameStateChange += ShowFinalCanvas;
            _replayButton.onClick.AddListener(ResetScene);
        }

        private void ResetScene()
        {
            LoadingScreenController.Instance.ResetGame();
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