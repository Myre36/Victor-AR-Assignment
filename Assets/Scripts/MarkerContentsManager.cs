using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MarkerContentsManager : MonoBehaviour
{
    [SerializeField]
    private string markerName;

    [SerializeField]
    private GameObject contents;

    //[SerializeField]
    //private GameObject rotationPointPrefab;

    //private GameObject rotationPoint;

    private ARTrackedImageManager imageManager;

    private GameObject instancedContents;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
        imageManager.trackablesChanged.AddListener(TrackableChanged);
    }

    private void OnDestroy()
    {
        imageManager.trackablesChanged.RemoveListener(TrackableChanged);
    }

    private void TrackableChanged(ARTrackablesChangedEventArgs<ARTrackedImage> trackableImageArgs)
    {
        foreach (var addedImage in trackableImageArgs.added)
        {
            if (addedImage.referenceImage.name == markerName)
            {
                //rotationPoint = GameObject.Instantiate(rotationPointPrefab, addedImage.transform.position, addedImage.transform.rotation);
                //rotationPoint.transform.parent = addedImage.transform;

                Vector3 minusedPositions = new Vector3(addedImage.transform.position.x, addedImage.transform.position.y - 1f, addedImage.transform.position.z);

                instancedContents = GameObject.Instantiate(contents, addedImage.transform.position, addedImage.transform.rotation);
                //instancedContents.GetComponent<RotateIsland>().rotationPoint = rotationPoint;
                instancedContents.transform.parent = addedImage.transform;
            }
        }

        foreach(var removedImage in trackableImageArgs.removed)
        {
            if(removedImage.Value.referenceImage.name == markerName)
            {
                GameObject.Destroy(instancedContents);
            }
        }
    }
}
