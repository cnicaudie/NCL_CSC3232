using UnityEngine;

public class Player : MonoBehaviour
{
    // Player infos (health, score, ...)

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LevelPoint"))
        {
            Debug.Log("Entering level !");
        }
    }
}
