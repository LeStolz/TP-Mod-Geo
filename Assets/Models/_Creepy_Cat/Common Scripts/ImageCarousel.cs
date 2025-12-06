using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageCarouselCrossfade : MonoBehaviour
{
    public List<Texture> imageList; // La liste des textures pour le carrousel
    public Renderer quadRenderer; // Le Renderer du quad sur lequel afficher les textures
    public float fadeDuration = 1f; // Durée du fondu enchaîné
    public float displayTime = 2f; // Temps d'affichage de chaque image
    public Color emissionColor = Color.white; // Couleur de l'auto-illumination fixe

    private int currentImageIndex = 0;
    private Material quadMaterial;
    private Material secondQuadMaterial;
    private bool isFading = false;

    private void Start()
    {
        if (imageList.Count > 0 && quadRenderer != null)
        {
            // Duplique le matériau pour gérer le fondu entre deux images
            quadMaterial = quadRenderer.material;
            secondQuadMaterial = new Material(quadMaterial);

            // Active l'auto-illumination dans le shader du matériau
            quadMaterial.EnableKeyword("_EMISSION");
            secondQuadMaterial.EnableKeyword("_EMISSION");

            StartCoroutine(StartCarousel());
        }
        else
        {
            Debug.LogError("Assurez-vous d'avoir ajouté des images et assigné un Renderer.");
        }
    }

    private IEnumerator StartCarousel()
    {
        while (true)
        {
            // Définit la texture de l'image actuelle
            Texture currentTexture = imageList[currentImageIndex];
            int nextImageIndex = (currentImageIndex + 1) % imageList.Count;
            Texture nextTexture = imageList[nextImageIndex];

            // Applique la texture actuelle et la suivante au matériau
            quadMaterial.mainTexture = currentTexture;
            secondQuadMaterial.mainTexture = nextTexture;

            // Applique la texture d'émission
            quadMaterial.SetTexture("_EmissionMap", currentTexture);
            quadMaterial.SetColor("_EmissionColor", emissionColor);
            secondQuadMaterial.SetTexture("_EmissionMap", nextTexture);
            secondQuadMaterial.SetColor("_EmissionColor", emissionColor);

            // Démarre la transition de fondu enchaîné (crossfade)
            yield return StartCoroutine(CrossfadeImages());

            // Passe à l'image suivante
            currentImageIndex = nextImageIndex;

            // Attend avant de recommencer
            yield return new WaitForSeconds(displayTime);
        }
    }

    //// Fonction de fondu enchaîné entre deux images
    //private IEnumerator CrossfadeImages()
    //{
    //    isFading = true;
    //    float elapsedTime = 0f;

    //    while (elapsedTime < fadeDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

    //        // Réduit l'alpha de l'image actuelle et augmente celui de la suivante
    //        quadMaterial.color = new Color(quadMaterial.color.r, quadMaterial.color.g, quadMaterial.color.b, 1 - alpha);
    //        secondQuadMaterial.color = new Color(secondQuadMaterial.color.r, secondQuadMaterial.color.g, secondQuadMaterial.color.b, alpha);

    //        // Affecte les matériaux au Renderer
    //        quadRenderer.materials = new Material[] { quadMaterial, secondQuadMaterial };

    //        yield return null;
    //    }

    //    // Fin de la transition, on garde uniquement le nouveau matériau
    //    quadRenderer.material = secondQuadMaterial;

    //    isFading = false;
    //}

    private IEnumerator CrossfadeImages()
{
    isFading = true;
    float elapsedTime = 0f;

    // On garde la puissance de l'auto-illumination constante
    Color originalEmissionColor = emissionColor;

    while (elapsedTime < fadeDuration)
    {
        elapsedTime += Time.deltaTime;
        float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

        // Réduit l'alpha de l'image actuelle et augmente celui de la suivante
        quadMaterial.color = new Color(quadMaterial.color.r, quadMaterial.color.g, quadMaterial.color.b, 1 - alpha);
        secondQuadMaterial.color = new Color(secondQuadMaterial.color.r, secondQuadMaterial.color.g, secondQuadMaterial.color.b, alpha);

        // Affecte les matériaux au Renderer
        quadRenderer.materials = new Material[] { quadMaterial, secondQuadMaterial };

        // Garder la couleur d'émission constante
        quadMaterial.SetColor("_EmissionColor", originalEmissionColor);
        secondQuadMaterial.SetColor("_EmissionColor", originalEmissionColor);

        yield return null;
    }

    // Fin de la transition, on garde uniquement le nouveau matériau
    quadRenderer.material = secondQuadMaterial;

    isFading = false;
}
}