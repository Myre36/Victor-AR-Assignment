using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.transform.position = respawnPoint.transform.position;
        }
    }
}
