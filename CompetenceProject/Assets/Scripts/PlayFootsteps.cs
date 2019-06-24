using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFootsteps : MonoBehaviour {

	public AudioSource audioSource;
    public int randomRangeIndex;

    public float[] initialCounter;
    public float currentCounter;
    public int currentCounterIndex;


    private void Start() {
        if (gameObject.tag == "Player") {
            randomRangeIndex = GameManager.GetAudioBank.playerFootsteps.Length - 1;
        }
        audioSource = GetComponent<AudioSource>();
        currentCounter = initialCounter[currentCounterIndex];
    }
    

    public void Play() {
        currentCounter -= Time.deltaTime;

        if (currentCounter < 0) {
            
            int rnd = Random.Range(0, randomRangeIndex);

            audioSource.clip = GameManager.GetAudioBank.playerFootsteps[rnd];
            audioSource.Play();

            currentCounter = initialCounter[currentCounterIndex];
        }
    }
}
