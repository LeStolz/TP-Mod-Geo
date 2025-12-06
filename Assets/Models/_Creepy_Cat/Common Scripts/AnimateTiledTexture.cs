using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimateTiledTexture : MonoBehaviour
{
    public List<Renderer> _renderers;                // Liste de renderers à animer
    public int _columns = 2;                          // Nombre de colonnes de la texture
    public int _rows = 2;                             // Nombre de lignes de la texture
    public Vector2 _scale = new Vector2(1f, 1f);     // Échelle de la texture
    public Vector2 _offset = Vector2.zero;            // Décalage de la texture
    public Vector2 _buffer = Vector2.zero;            // Tampon pour les images
    public float _framesPerSecond = 10f;              // Images par seconde
    public bool _playOnce = false;                    // Joue l'animation une seule fois
    public bool _disableUponCompletion = false;       // Désactive le renderer à la fin
    public bool _enableEvents = false;                // Événements à la fin de l'animation
    public bool _playOnEnable = true;                 // Joue l'animation au démarrage
    public bool _newMaterialInstance = false;         // Crée une nouvelle instance de matériau

    private int _index = 0;                           // Index de l'image actuelle
    private Vector2 _textureSize = Vector2.zero;      // Taille de la texture
    private List<Material> _materialInstances;        // Instances de matériau pour chaque renderer
    private bool _isPlaying = false;                  // Indicateur de lecture

    public delegate void VoidEvent();                  // Délégué d'événement
    private List<VoidEvent> _voidEventCallbackList;   // Liste de callbacks

    private void Awake()
    {
        if (_enableEvents)
            _voidEventCallbackList = new List<VoidEvent>();

        _materialInstances = new List<Material>(_renderers.Count);

        foreach (var renderer in _renderers)
        {
            if (renderer != null)
                ChangeMaterial(renderer, renderer.sharedMaterial, _newMaterialInstance);
        }
    }

    private void OnDestroy()
    {
        if (_newMaterialInstance)
        {
            foreach (var material in _materialInstances)
            {
                if (material != null)
                    Object.Destroy(material);
            }
        }
    }

    public void Play()
    {
        if (_isPlaying)
        {
            StopCoroutine("updateTiling");
            _isPlaying = false;
        }

        foreach (var renderer in _renderers)
        {
            if (renderer != null)
                renderer.enabled = true;
        }

        _index = 0; // Réinitialiser l'index pour commencer l'animation
        StartCoroutine(updateTiling());
    }

    public void ChangeMaterial(Renderer renderer, Material newMaterial, bool newInstance = false)
    {
        Material materialInstance = null;

        if (newInstance)
        {
            materialInstance = new Material(newMaterial);
            _materialInstances.Add(materialInstance);
        }
        else
        {
            materialInstance = newMaterial;
        }

        renderer.material = materialInstance;
        CalcTextureSize(renderer);
    }

    private void CalcTextureSize(Renderer renderer)
    {
        _textureSize = new Vector2(1f / _columns, 1f / _rows);
        _textureSize.x /= _scale.x;
        _textureSize.y /= _scale.y;
        _textureSize -= _buffer;

        // Remplacement de _MainTex par _BaseMap pour URP
        renderer.material.SetTextureScale("_BaseMap", _textureSize);
    }

    private void OnEnable()
    {
        if (_playOnEnable)
            Play();
    }

    private IEnumerator updateTiling()
    {
        _isPlaying = true;
        int checkAgainst = (_rows * _columns);

        while (true)
        {
            if (_index >= checkAgainst)
            {
                _index = 0;

                if (_playOnce)
                {
                    if (_disableUponCompletion)
                    {
                        foreach (var renderer in _renderers)
                        {
                            if (renderer != null)
                                renderer.enabled = false;
                        }
                    }

                    if (_enableEvents)
                        HandleCallbacks(_voidEventCallbackList);

                    _isPlaying = false;
                    yield break;
                }
            }

            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                    ApplyOffset(renderer);
            }

            _index++;
            yield return new WaitForSeconds(1f / _framesPerSecond);
        }
    }

    private void ApplyOffset(Renderer renderer)
    {
        Vector2 offset = new Vector2((float)_index / _columns - (_index / _columns),
                                      1 - ((_index / _columns) / (float)_rows));

        if (offset.y == 1)
            offset.y = 0.0f;

        offset.x += ((1f / _columns) - _textureSize.x) / 2.0f;
        offset.y += ((1f / _rows) - _textureSize.y) / 2.0f;

        offset.x += _offset.x;
        offset.y += _offset.y;

        // Remplacement de _MainTex par _BaseMap pour URP
        renderer.material.SetTextureOffset("_BaseMap", offset);
    }

    private void HandleCallbacks(List<VoidEvent> cbList)
    {
        foreach (var callback in cbList)
            callback();
    }
}