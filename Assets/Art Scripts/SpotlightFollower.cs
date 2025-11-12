using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightFollower : MonoBehaviour
{
    // 1. This is a slot in the Inspector where you will drag the floor Material.
    [SerializeField]
    private Material floorMaterial;

    // 2. This is the exact Reference name from your Shader Graph (from Step 1).
    // MUST match what you found in the Shader Graph properties.
    private const string LightPositionPropertyName = "_FakeLightPosition";

    void Update()
    {
        // Check if we actually have a material assigned, to prevent errors
        if (floorMaterial != null)
        {
            // Get the current World Position of this GameObject (the SpotlightTarget)
            Vector3 targetPosition = transform.position;

            // 3. IMPORTANT: Lock the Y (height) position to the floor's Y.
            // If your floor is flat and at Y=0, this line keeps the spotlight grounded:
            targetPosition.y = 0f;

            // If your floor is at Y=-1.5, use: targetPosition.y = -1.5f;

            // Pass the modified position vector to the shader.
            // This is the line that actually updates the spotlight's center!
            floorMaterial.SetVector(LightPositionPropertyName, targetPosition);
        }
    }
}