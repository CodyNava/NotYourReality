using UnityEngine;

namespace Puzzle.Balloon_Matryoshka
{
    public class BalloonChecker : MonoBehaviour
    {
        [Space]
        [Tooltip("The Tier 1 balloon prefab")]
        [SerializeField] private GameObject balloonPrefab;
        [Tooltip("The amount of balloons that should initially spawn")]
        [SerializeField] private int amount;
        [Tooltip("The key that drops after the last balloon is destroyed")]
        [SerializeField] private GameObject key;
        private void Awake()
        {
            for (var i = 0; i < amount; i++)
            {
                Instantiate (balloonPrefab, transform.position, transform.rotation, transform);
            }
        }

        public void CheckWin()
        {
            Debug.Log(transform.childCount);
            if (transform.childCount > 1) return;
            Debug.Log("You Win!");
            Instantiate(key, transform.position, transform.rotation);
        }
    }
}