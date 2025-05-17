using UnityEngine;
using UnityEngine.UI;

public class CameraToggle : MonoBehaviour
{
    [SerializeField] private GameObject rawImageObject; // Reference to the Raw Image GameObject
    private bool isCameraActive = false; // Track the camera state

    private void Start()
    {
        if (rawImageObject != null)
        {
            rawImageObject.SetActive(false); // Ensure the camera feed starts as hidden
        }
        else
        {
            Debug.LogError("Raw Image Object is not assigned in the Inspector.");
        }
    }

    public void ToggleCameraFeed()
    {
        if (rawImageObject != null)
        {
            isCameraActive = !isCameraActive; // Toggle the state
            rawImageObject.SetActive(isCameraActive); // Show or hide the camera feed
        }
    }
}
