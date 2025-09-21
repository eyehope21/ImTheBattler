using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraToFollow;
    public Vector3 offset = new Vector3(0, 0, 5);

    void Update()
    {
        // Follows the camera's position
        transform.position = cameraToFollow.position + cameraToFollow.forward * offset.z;

        // Keeps the original, upright rotation
        transform.rotation = Quaternion.identity;
    }
}