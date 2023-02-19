using DG.Tweening;
using RotatingRoutes.Managers;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RotatingRoutes.UI
{
    public class FinishCanvas : MonoBehaviour
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChange -= ShowFinalCanvas;
        }

        private void ShowFinalCanvas(GameState gameState)
        {
            if (gameState != GameState.GameCompleted)
                return;
            _canvasGroup.DOFade(1, .25f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

    }
}