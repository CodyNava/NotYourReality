using UnityEngine;

namespace Puzzle.Balloon_Matryoshka
{
    public class Balloon : MonoBehaviour
    {
        [Space]
        [Tooltip("The following ballon prefab that should spawn\n" +
                 "For Tier 1: Put Tier 2 Prefab in here\n" +
                 "For Tier 2: Put Tier 3 Prefab in here\n" +
                 "For Tier 3: Put anything in here, it doesn't matter")]
        [SerializeField] private GameObject balloonPrefab;
        [Tooltip("The amount of balloons that are spawned from each destroyed balloon\n" +
                 "For Tier 1: 2 Balloons\n" +
                 "For Tier 2: 3 Balloons\n" +
                 "<b>Important</b> For Tier 3: <b> Amount needs to be 0 </b>")]
        [SerializeField] private int spawnAmount;
        [Tooltip("The scale the balloon should grow to after spawning")]
        [SerializeField] private Vector3 size;
        
        private BalloonChecker _balloonChecker;
        private const float Speed = 1.5f;
        private bool _sizeReached;
        private SpringJoint _rope;
        private Rigidbody _rb;

        private void Start()
        {
            //TODO: Add Rope Rigidbody
            /*_rb = GetComponent<Rigidbody>();
            _rope = GetComponent<SpringJoint>();
            _rope.connectedBody = GameObject.FindWithTag("Rope").GetComponent<Rigidbody>();*/
            _balloonChecker = GetComponentInParent<BalloonChecker>();
        }

        private void Update()
        {
            if (_sizeReached) return;
            transform.localScale = Vector3.Lerp(transform.localScale, size, Speed * Time.deltaTime);
            var difference = size.x - transform.localScale.x;
            _sizeReached = difference <= 0.05f;
        }

        //TODO: Enable when rope asset is there
        /*private void FixedUpdate()
        {
            _rb.AddForce(Vector3.up, ForceMode.Force);
        }*/

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Scissors")) return;
            for (var i = 0; i < spawnAmount; i++)
            {
                Instantiate(balloonPrefab, transform.position, transform.rotation, _balloonChecker.transform);
            }
            _balloonChecker.CheckWin();
            Destroy(gameObject);
        }
    }
}