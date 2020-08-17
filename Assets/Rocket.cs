using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] AudioClip SuccessSound;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem DeathParticles;
    [SerializeField] ParticleSystem SuccessParticles;
    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionsEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive) ProcessInput();
        if (Debug.isDebugBuild) RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) LoadNextScene();
        else if (Input.GetKeyDown(KeyCode.C)) collisionsEnabled = !collisionsEnabled;
    }

    private void ProcessInput()
    {
        RespondToThrustInput();
        RespondToRotateInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsEnabled) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //print("OK");
                break;
            case "Finish":
                // print("Hit finish");
                StartSuccessSequence();
                break;
            default:
                //print("Collided");
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(SuccessSound);
        SuccessParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(DeathSound);
        DeathParticles.Play();
        Invoke("LoadFirstScene", levelLoadDelay);
    }


    private void LoadFirstScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene((currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) audioSource.PlayOneShot(mainEngine);
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // Take control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {

            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }


}
