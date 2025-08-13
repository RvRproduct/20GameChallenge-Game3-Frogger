using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] Transform playerTransform;

    private void Awake()
    {
        mainCamera = Camera.main;   
    }

    private void Update()
    {
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x,
            playerTransform.position.y,
            mainCamera.transform.position.z);
    }
}
