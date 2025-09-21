using UnityEngine;

public class Spinner : MonoBehaviour
{
    // Adjust the speed of the spin in the Inspector
    [SerializeField] public float spinSpeed = 50f;

    void Update()
    {
        // Rotate the sphere around the Y-axis
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}