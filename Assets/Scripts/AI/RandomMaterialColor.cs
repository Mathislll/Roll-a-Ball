using UnityEngine;

public class RandomMaterialColor : MonoBehaviour
{
    public Renderer targetRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            Material material = targetRenderer.material;
            material.color = new Color(Random.value, Random.value, Random.value);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
