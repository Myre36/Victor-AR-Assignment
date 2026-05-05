using UnityEngine;
using TMPro;

public class DoorScript : MonoBehaviour
{
    [SerializeField]
    private TMP_Text congratsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        congratsText = GameObject.Find("UI").GetComponentInChildren<TMP_Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.GetComponent<PlayerMovement>().hasKey == true)
            {
                Debug.Log("Door opened");
                congratsText.enabled = true;
            }
        }
    }
}
