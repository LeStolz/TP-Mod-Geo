using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class GuideManager : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds4_0 = new(4.0f);

    enum State
    {
        Idle,
        MovingToArtwork,
        AdmiringArtwork
    }

    [Serializable]
    class Dialog
    {
        public string message;
        public AudioClip clip;
        public TimeSpan duration;

        public Dialog(string message, AudioClip clip = null, TimeSpan duration = default)
        {
            this.message = message;
            this.clip = clip;
            this.duration = duration;
        }
    }

    [SerializeField] float minDistanceToPlayer = 5.8f;
    [SerializeField] float maxDistanceToPlayer = 6.2f;
    [SerializeField] Transform anchorPoint;
    [SerializeField] TextMeshProUGUI guideText;
    [SerializeField] List<Dialog> dialogs;

    AudioSource audioSource;
    bool isGuiding = true;
    Animator animator;
    NavMeshAgent agent;
    GameObject player;

    State currentState = State.Idle;
    Vector3 currentDestination;

    void Start()
    {
        isGuiding = PlayerPrefs.GetInt("WithGuide", 1) == 1;
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (anchorPoint == null)
        {
            anchorPoint = new GameObject("AnchorPoint").transform;
            anchorPoint.position = transform.position;
        }

        currentDestination = ArtworksManager.Instance.GetNextArtwork().transform.position;
    }

    void Update()
    {
        animator.SetFloat("speed", agent.velocity.magnitude);

        if (player == null || !isGuiding)
        {
            agent.SetDestination(anchorPoint.position);
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (currentState == State.AdmiringArtwork)
        {
            var directionToArtwork = (currentDestination - transform.position).normalized;

            if (Math.Abs(Vector3.Dot(transform.forward, directionToArtwork)) < 0.9f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(directionToArtwork, Vector3.up),
                    Time.deltaTime * 2.0f
                );
            }

            return;
        }

        if (currentState == State.Idle)
        {
            if (distanceToPlayer <= minDistanceToPlayer)
            {
                currentState = State.MovingToArtwork;
            }
        }

        if (currentState == State.MovingToArtwork)
        {
            agent.SetDestination(currentDestination);

            CheckTooFarFromPlayer(distanceToPlayer);

            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                currentState = State.AdmiringArtwork;
                animator.SetBool("admiring", true);
                ArtworksManager.Instance.HighlightNextArtwork();

                Speak(dialogs.Find(d => d.message.Contains("piece")));

                IEnumerator ResumeMoving()
                {
                    yield return _waitForSeconds4_0;

                    currentDestination = ArtworksManager.Instance.GetNextArtwork().transform.position;
                    currentState = State.Idle;
                    animator.SetBool("admiring", false);

                    CheckTooFarFromPlayer(distanceToPlayer);
                }

                StartCoroutine(ResumeMoving());
            }
        }
    }

    void CheckTooFarFromPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > maxDistanceToPlayer)
        {
            currentState = State.Idle;
            agent.SetDestination(transform.position);

            Speak(dialogs.Find(d => d.message.Contains("wait")));

            return;
        }
    }

    void Speak(Dialog dialog)
    {
        var duration = dialog.duration;
        var message = dialog.message;
        var clip = dialog.clip;

        if (duration == default) duration = TimeSpan.FromSeconds(Math.Max(4.0, message.Length * 0.05));

        StopAllCoroutines();

        IEnumerator SpeakingRoutine(string msg, TimeSpan dur)
        {
            guideText.text = msg;
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
            yield return new WaitForSeconds((float)dur.TotalSeconds);
            guideText.text = "";
        }

        StartCoroutine(SpeakingRoutine(message, duration));
    }

    public void SetFollowing(int follow)
    {
        isGuiding = follow == 1;
        PlayerPrefs.SetInt("WithGuide", follow);
        PlayerPrefs.Save();
    }
}
