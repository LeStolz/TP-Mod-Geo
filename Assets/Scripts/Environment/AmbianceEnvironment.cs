using UnityEngine;


[CreateAssetMenu(fileName = "NewAmbianceEnvironment", menuName = "Data/AmbianceEnvironment")]
class AmbianceEnvironmentSO : ScriptableObject
{
	public string modeName;
	public float timeBetweenColors;
	public float fadeDuration;
	public float emissionIntensity;
	public AudioClip ambianceClip;
}