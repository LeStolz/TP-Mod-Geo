using UnityEngine;

public class ArtworksManager : MonoBehaviour
{
    public static ArtworksManager Instance { get; private set; }

    [SerializeField] Artwork[] artworks;
    public int Current { get; private set; } = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (artworks.Length == 0)
        {
            artworks = FindObjectsByType<Artwork>(FindObjectsSortMode.None);
        }

        for (int k = 0; k < artworks.Length; k++)
            artworks[k].ToggleHighlight(k == Current);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            HighlightNextArtwork();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            HighlightPreviousArtwork();
        }
    }

    Artwork HighlightArtwork(int id)
    {
        Current = id % artworks.Length;

        for (int k = 0; k < artworks.Length; k++)
            artworks[k].ToggleHighlight(k == Current);

        return artworks[Current];
    }

    Artwork GetArtwork(int id)
    {
        return artworks[id % artworks.Length];
    }

    public Artwork GetNextArtwork()
    {
        return GetArtwork(Current + 1);
    }

    public Artwork GetPreviousArtwork()
    {
        return GetArtwork(Current - 1);
    }

    public Artwork HighlightNextArtwork()
    {
        return HighlightArtwork(Current + 1);
    }

    public Artwork HighlightPreviousArtwork()
    {
        return HighlightArtwork(Current - 1);
    }
}
