using System.Collections.Generic;
using UnityEngine;

public class Artwork : MonoBehaviour
{
    [SerializeField] List<Renderer> renderers;
    [SerializeField] ParticleSystem auraParticles;
    [SerializeField] GameObject infoPanel;
    [SerializeField] Material fresnelMaterial;

    public void ToggleHighlight(bool on)
    {
        if (renderers == null || renderers.Count == 0) return;

        foreach (var renderer in renderers)
        {
            MaterialPropertyBlock haloEffect = new();

            // Check if the second material is the fresnel material if not, set it
            if (renderer.materials.Length < 2)
            {
                renderer.materials = new Material[] { renderer.material, fresnelMaterial };
            }

            renderer.GetPropertyBlock(haloEffect, 1);
            haloEffect.SetFloat("_BaseAlpha", on ? 1 : 0);
            renderer.SetPropertyBlock(haloEffect, 1);
        }

        if (auraParticles)
        {
            if (on && !auraParticles.isPlaying) auraParticles.Play();
            if (!on && auraParticles.isPlaying) auraParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (infoPanel)
        {
            infoPanel.SetActive(on);
        }
    }
}
