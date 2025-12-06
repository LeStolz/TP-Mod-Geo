using UnityEngine;

public class LightManager : MonoBehaviour
{
    new Light light;
    MeshRenderer lightMeshRenderer;
    GameObject player;

    [SerializeField]
    float intensityMultiplier = 100f;
    [SerializeField]
    float meshMultiplier = 7f;
    [SerializeField]
    float maxIntensity = 20f;

    void Awake()
    {
        light = GetComponentInChildren<Light>();
        lightMeshRenderer = GetComponentInChildren<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player != null && light != null && lightMeshRenderer != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            light.intensity = Mathf.Clamp(intensityMultiplier / distance / distance / distance, 0, maxIntensity);
            lightMeshRenderer.material.SetColor("_EmissionColor", Color.white * Mathf.LinearToGammaSpace(light.intensity / meshMultiplier));
        }
    }
}
