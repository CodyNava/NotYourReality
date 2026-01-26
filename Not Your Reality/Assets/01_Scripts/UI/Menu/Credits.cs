using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class Credits : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private Button backButton;
        
        [Header("Scroll Settings")]
        [SerializeField] private float scrollSpeed;

        private float _end;
        private bool _isScrolling = true;

        public bool IsScrolling
        {
            get => _isScrolling;
            set => _isScrolling = value;
        }

        private void OnEnable()
        {
            Time.timeScale = 1f;
            ResetCredits();
        }
        private void ResetCredits()
        {
            Canvas.ForceUpdateCanvases();
            
            var contentHeight = content.rect.height;
            var viewportHeight = viewport.rect.height;
            
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, -contentHeight);
            
            _end = viewportHeight;
            _isScrolling = true;
        }

        private void Update()
        {
            if (!_isScrolling)
            {
                return;
            }
            
            content.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);

            if (content.anchoredPosition.y >= _end)
            {
                backButton.onClick.Invoke();
            }
        }
    }
}
