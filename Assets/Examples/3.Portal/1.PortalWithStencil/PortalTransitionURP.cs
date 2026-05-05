using UnityEngine;

public class PortalTransitionURP : MonoBehaviour
{
    public Transform interiorContainer;
    public Transform insideSkyboxCamera;

    public string PortalContentsLayer = "PortalContents";
    public string InsidePortalLayer = "InsidePortal";

    private int layerOutside;
    private int layerInside;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        layerOutside = LayerMask.NameToLayer(PortalContentsLayer);
        layerInside = LayerMask.NameToLayer(InsidePortalLayer);
    }


    void OnTriggerStay(Collider other)
    {
        if (other.transform != mainCam.transform) return;


        Vector3 nearPlanePos = mainCam.transform.position + (mainCam.transform.forward * mainCam.nearClipPlane);

        Vector3 localPos = transform.InverseTransformPoint(nearPlanePos);

        if (localPos.z < 0f)
        {
            SetLayerRecursively(interiorContainer.gameObject, layerInside);
            SetLayerRecursively(insideSkyboxCamera.transform.GetChild(0).gameObject, layerInside);
        }
        else
        {
            SetLayerRecursively(interiorContainer.gameObject, layerOutside);
            SetLayerRecursively(insideSkyboxCamera.transform.GetChild(0).gameObject, layerOutside);
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}