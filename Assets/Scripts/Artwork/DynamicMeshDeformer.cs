using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class DynamicDeformer : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds2_0 = new(2.0f);

    [SerializeField] BoxCollider deformArea;

    [Header("Sine (respiration)")]
    [SerializeField] float amplitude = 0.005f; // en mètres le long de la normale
    [SerializeField] float frequency = 1.0f; // Hz
    [Header("Sculpt (souris)")]
    [SerializeField] float radius = 0.5f; // m
    [SerializeField] float strength = 0.2f; // m par coupé de pinceau

    Mesh _mesh;
    Vector3[] _originalVerts; // copie des vertices d'origine
    Vector3[] _baseVerts; // copie des vertices d'origine
    Vector3[] _baseNormals; // normales de référence
    Vector3[] _workVerts; // buffer de travail
    bool _meshColliderDirty;

    void Awake()
    {
        // .mesh => instance locale dynamique
        _mesh = GetComponent<MeshFilter>().mesh;
        _mesh.MarkDynamic();
        _baseVerts = _mesh.vertices;
        _workVerts = (Vector3[])_baseVerts.Clone();
        _originalVerts = (Vector3[])_baseVerts.Clone();
        _baseNormals = _mesh.normals;
        if (_baseNormals == null || _baseNormals.Length != _baseVerts.Length)
        {
            _mesh.RecalculateNormals();
            _baseNormals = _mesh.normals;
        }
    }

    void Update()
    {
        DeformSine();
        SculptInput();

        // Mise à jour du mesh (normales recalculées modérément pour limiter le coût)
        _mesh.vertices = _workVerts;
        if (Time.frameCount % 3 == 0) _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        // Si vous avez un MeshCollider, ne le mettez à jour qu'en fin de stroke:
        if (_meshColliderDirty && Input.GetMouseButtonUp(0))
        {
            var mc = GetComponent<MeshCollider>();
            if (mc)
            {
                mc.sharedMesh = null; // d�tacher avant de r�assigner
                mc.sharedMesh = _mesh;
            }
            _meshColliderDirty = false;
        }
    }

    bool isPerforming = false;
    public void Perform()
    {
        if (isPerforming) return;

        var prevAmplitude = amplitude;
        var prevFrequency = frequency;
        amplitude *= 4;
        frequency *= 4;

        isPerforming = true;

        IEnumerator ResetPerform()
        {
            yield return _waitForSeconds2_0;
            amplitude = prevAmplitude;
            frequency = prevFrequency;
            isPerforming = false;
        }

        StartCoroutine(ResetPerform());
    }

    public void Reset()
    {
        _workVerts = (Vector3[])_originalVerts.Clone();
        _baseVerts = (Vector3[])_originalVerts.Clone();
        _meshColliderDirty = true;
    }

    void DeformSine()
    {
        float t = Time.time * Mathf.PI * 2f * frequency;
        for (int i = 0; i < _workVerts.Length; i++)
        {
            // D�placement le long de la normale d'origine (respiration douce)
            float s = Mathf.Sin(t);
            _workVerts[i] = _baseVerts[i] + _baseNormals[i] * (s * amplitude);
        }
    }

    void SculptInput()
    {
        if (!Camera.main || !Input.GetMouseButton(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, 1000f))
            return;

        if (!deformArea.bounds.Contains(hit.point))
            return;

        // Sens : clic gauche = pousser, SHIFT+clic gauche = tirer
        float dir = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -1f : 1f;

        // Centre et normale au point d'impact (espace monde)
        Vector3 center = hit.point;
        Vector3 pushN = hit.normal * dir;

        // Appliquer une bosse/creux gaussienne dans un rayon donn�
        for (int i = 0; i < _baseVerts.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(_baseVerts[i]);
            float d = Vector3.Distance(worldPos, center);
            if (d < radius)
            {
                float w = Mathf.Exp(-(d * d) / (2f * radius * radius)); // gaussienne
                worldPos += pushN * (strength * w);

                _baseVerts[i] = transform.InverseTransformPoint(worldPos);
            }
        }
        _meshColliderDirty = true; // on r�actualisera plus tard
    }
}
