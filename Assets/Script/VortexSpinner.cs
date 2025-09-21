using UnityEngine;

public class VortexSpinner : MonoBehaviour
{
    // Adjust the speed of the spin in the Inspector
    [SerializeField] private float spinSpeed = 0.1f;

    private Material material;

    void Start()
    {
        // Get the material of the quad
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Adjust the texture's offset to create a spinning effect
        material.mainTextureOffset += new Vector2(spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime);
    }
}