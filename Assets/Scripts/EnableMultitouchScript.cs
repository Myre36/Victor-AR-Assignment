using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class EnableMultitouchScript : MonoBehaviour
{
    void Awake()
    {
        EnhancedTouchSupport.Enable();
    }
}
