using UnityEngine;

public class RotateIsland : MonoBehaviour
{
    public GameObject rotationPoint;

    // Update is called once per frame
    void Update()
    {
        if (rotationPoint == null)
        {
            return;
        }
        this.transform.position = rotationPoint.transform.position;
        this.transform.rotation = rotationPoint.transform.rotation;
    }
}
