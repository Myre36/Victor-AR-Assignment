using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SkyboxTexture : MonoBehaviour
{
    // Material used by the quad to render the skybox rendered texture
    [SerializeField]
    private Material skyboxTextureMaterial;

    // Camera used to render the skybox to a render texture
    private Camera skyboxCamera;
    // Quad used to render the render texture onto
    private GameObject quad;
    // Variables used to resize the quad when needed
    private ScreenOrientation previousOrientation;
    private float previousFov, previousFarClip;

    public string PortalContentsLayer = "PortalContents";
    private int layerOutside;

    void Start()
    {
        skyboxCamera = GetComponent<Camera>();
        layerOutside = LayerMask.NameToLayer(PortalContentsLayer);
    }

    void Update()
    {
        // Recreate the quad when the orientation changes or the camera changes
        if(Screen.orientation != previousOrientation 
            || Camera.main.fieldOfView != previousFov 
            || Camera.main.farClipPlane != previousFarClip)
        {
            CreateFarPlaneQuad();
        }
    }

    private void CreateFarPlaneQuad()
    {
        // Clean the memory before re-creating the quad
        if (quad)
        {
            RenderTexture.ReleaseTemporary(skyboxCamera.targetTexture);
            DestroyImmediate(quad);
        }

        // Update camera values to AR camera values and orientation
        previousFarClip = skyboxCamera.farClipPlane = Camera.main.farClipPlane;
        previousFov = skyboxCamera.fieldOfView = Camera.main.fieldOfView;
        previousOrientation = Screen.orientation;

        // Create new quad
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.layer = layerOutside;
        var quadTr = quad.transform;
        quadTr.parent = this.transform;

        // The camera FOV is used in the vertical scale
        var verticalScale = Mathf.Tan(Mathf.Deg2Rad * skyboxCamera.fieldOfView / 2f) 
            * skyboxCamera.farClipPlane * 2;
        // The horizontal size depends on the actual screen width
        var horizontalScale = verticalScale * Screen.width / (float)Screen.height;

        // Apply the scale position and rotation to stay in the far plane
        quadTr.localScale = new Vector3(horizontalScale, verticalScale, 1f);
        var cTr = skyboxCamera.transform;
        quadTr.position = cTr.position + cTr.forward * skyboxCamera.farClipPlane * 0.999f;
        quadTr.rotation = Quaternion.LookRotation(cTr.forward, cTr.up);

        // Create the render texture and set it to the quad material
        skyboxCamera.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        if (skyboxTextureMaterial)
        {
            skyboxTextureMaterial.SetTexture("_BaseMap", skyboxCamera.targetTexture);
            quad.GetComponent<Renderer>().material = skyboxTextureMaterial;
        }
    }
}
