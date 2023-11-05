using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureRandomizer : MonoBehaviour
{

    // Create a public list of materials
    public List<Material> materialsList;

    private void Start()
    {
        // Check if the materialsList is not empty
        if (materialsList.Count > 0)
        {
            // Generate a random index to select a material from the list
            int randomIndex = Random.Range(0, materialsList.Count);

            // Get the Renderer component attached to the GameObject
            Renderer rend = GetComponent<Renderer>();

            // Check if there is a Renderer component
            if (rend != null)
            {
                // Assign the randomly selected material to the object's renderer
                rend.material = materialsList[randomIndex];
            }
            else
            {
                Debug.LogError("No Renderer component found on this GameObject.");
            }
        }
    }

}
