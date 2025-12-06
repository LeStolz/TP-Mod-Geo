using System.Collections.Generic;
using UnityEngine;

class AmbianceEnvironmentManager : MonoBehaviour
{
	[SerializeField]
	List<AmbianceEnvironmentSO> ambianceModes;
	[SerializeField]
	private int currentModeIndex = 0;

	private new GameObject camera;
	private List<ChangeEmissionColor> emissionColors;
	public static AmbianceEnvironmentManager Instance { get; private set; }

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

	private void Start()
	{
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		emissionColors = new List<ChangeEmissionColor>(FindObjectsByType<ChangeEmissionColor>(FindObjectsSortMode.None));

		currentModeIndex = PlayerPrefs.GetInt("AmbianceModeIndex", 0);
		SetModeIndex(currentModeIndex);
	}

	public void SetModeIndex(int modeIndex)
	{
		currentModeIndex = modeIndex;
		PlayerPrefs.SetInt("AmbianceModeIndex", currentModeIndex);

		foreach (var emissionColor in emissionColors)
		{
			var currentAmbianceMode = ambianceModes[currentModeIndex];

			emissionColor.timeBetweenColors = currentAmbianceMode.timeBetweenColors;
			emissionColor.fadeDuration = currentAmbianceMode.fadeDuration;
			emissionColor.emissionIntensity = currentAmbianceMode.emissionIntensity;

			camera.GetComponent<AudioSource>().clip = currentAmbianceMode.ambianceClip;
			camera.GetComponent<AudioSource>().Play();
		}
	}
}