using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu
{
    public class InteractionUI : MonoBehaviour
    {
        public static InteractionUI Instance { get; private set; }
        private readonly  List<GameObject> _children = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            foreach (var child in transform.GetComponentsInChildren<Transform>())
            {
                if (child != gameObject.transform)
                {
                    _children.Add(child.gameObject);
                }
            }
            Hide();
        }

        public void Show(string tooltip)
        {
            switch (tooltip)
            {
                case "TV":
                    _children.Find(x => x.name == "TV").gameObject.SetActive(true);
                    break;
                case "Inspection":
                    _children.Find(x => x.name == "Inspection").gameObject.SetActive(true);
                    break;
            }
        }

        public void Hide()
        {
            foreach  (var child in _children)
                child.gameObject.SetActive(false);
        }
    }
}