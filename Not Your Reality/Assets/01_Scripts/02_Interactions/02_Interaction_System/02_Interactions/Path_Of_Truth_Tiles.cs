using UnityEngine;

public class Path_Of_Truth_Tiles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch (gameObject.tag)
        {
            case "Safe":
            case "Not Safe" when !other.gameObject.CompareTag("Player"):
                return;
            case "Not Safe" when other.gameObject.CompareTag("Player"):
                Debug.Log("Reset Hit");
                break;
        }
    }
}
