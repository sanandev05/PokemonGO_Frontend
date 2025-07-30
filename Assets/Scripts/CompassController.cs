using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    public RectTransform pointer; 
    public Transform targetToTrack; 

    void Update()
    {
        if (targetToTrack == null || pointer == null)
            return;

        float yRotation = targetToTrack.eulerAngles.y;

        pointer.localRotation = Quaternion.Euler(0, 0, -yRotation);
    }
}
