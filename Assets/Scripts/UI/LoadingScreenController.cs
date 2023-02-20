using DG.Tweening;
using RotatingRoutes.Managers;
using RotatingRoutes.Util.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RotatingRoutes.UI
{

    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] Sprite[] _sprites;

        private List<Image> _images = new();
        public static LoadingScreenController Instance;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private void Start()
        {
            Application.targetFrameRate = 240;
            foreach (Transform child in transform)
            {
                Image childImageComp = child.GetComponent<Image>();
                _images.Add(childImageComp);
                childImageComp.DOColor(Color.white, .1f);
            }
            _canvasGroup = GetComponent<CanvasGroup>();
            ShuffleImages();
            _canvasGroup.DOFade(0, .5f).SetDelay(2f);
        }

        public void ResetGame()
        {
            _canvasGroup.DOFade(1, .5f).OnComplete(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
            ShuffleImages();
            _canvasGroup.DOFade(0, .5f).SetDelay(2.5f).OnComplete(() => GameManager.ResetGame());
        }


        private void ShuffleImages()
        {
            List<Sprite> sprites = _sprites.ToList();
            for (int i = 0; i < _images.Count; i++)
            {
                int spriteIndex = Random.Range(0, sprites.Count);
                Sprite nextSprite = sprites[spriteIndex];
                _images[i].sprite = nextSprite;
                sprites.RemoveAt(spriteIndex);
                RectTransform imageRect = _images[i].rectTransform;
                float delay = .25f + Random.Range(.2f, .35f);
                imageRect.DOComplete();
                imageRect.DOScale(1.1f, .3f).SetLoops(2, LoopType.Yoyo).SetDelay(delay);
                //imageRect.DOPunchPosition(Vector3.one * Random.Range(1.25f, 1.5f), Random.Range(.1f, .25f), Random.Range(5, 10)).SetLoops(5);
                imageRect.DOAnchorPosY(imageRect.anchoredPosition.y + 25, .6f)
                         .SetLoops(2, LoopType.Yoyo)
                         .SetDelay(delay)
                         .OnComplete(() => imageRect.DOKill());
                //imageRect.DORotate(new Vector3(0, 180, 0), 1f).SetLoops(2, LoopType.Yoyo).SetDelay(delay);
            }
        }
    }
}