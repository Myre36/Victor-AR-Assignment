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
        Vector3 islandTransform = new Vector3(rotationPoint.transform.position.x, rotationPoint.transform.position.y - 1f, rotationPoint.transform.position.z);

        this.transform.position = islandTransform;
        //this.transform.rotation = rotationPoint.transform.rotation;
    }
}
