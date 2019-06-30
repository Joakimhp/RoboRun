using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeSpawner : MonoBehaviour {

    public int initialEnvironmentPrefabs;
    public int nextLevelTarget;
    public int nextLevelTracker;
    public int newChallengeMaxEntry; //Should be equal to amount of challenges that should be candidates for the "swap" function, but also for initializing the challenges.


    public GameObject player;
    public List<GameObject> challengeCandidates;
    public List<GameObject> activeChalleges;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        if (initialEnvironmentPrefabs == 0) initialEnvironmentPrefabs = 3;
        SetupInitialEnvironment();
        nextLevelTracker = 0;
    }

    private void LateUpdate() {
        transform.position = new Vector3(0, 0, player.transform.position.z + 13);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Environment") {
            UpdateEnvironment();
        }
    }

    private void SetupInitialEnvironment() {
        if (challengeCandidates != null) {
            int _newChallengeMaxEntry = newChallengeMaxEntry;
            for (int i = 0; i < initialEnvironmentPrefabs; i++) {
                int randomEntry = Random.Range(0, _newChallengeMaxEntry);

                activeChalleges.Add(challengeCandidates[randomEntry]);
                challengeCandidates.RemoveAt(randomEntry);
                _newChallengeMaxEntry -= 1;
            }
            for (int i = 0; i < activeChalleges.Count; i++) {
                Vector3 spawnPosition = new Vector3(0, 0, (26 * 3) + ((26 * i)));
                GameObject spawn = Instantiate(activeChalleges[i], spawnPosition, Quaternion.identity);
                }
        }
        else {
            Debug.LogError("There are no challenges to spawn");
        }
    }

    private void UpdateEnvironment() {
        InstantiateEnvironment();
        if (challengeCandidates.Count != 0) {
            if (challengeCandidates.Count < newChallengeMaxEntry)
                newChallengeMaxEntry = challengeCandidates.Count;
            
            nextLevelTracker += 1;

            if (nextLevelTracker == nextLevelTarget) {
                nextLevelTracker = 0;

                SwapEnvironment();
            }
        }
    }

    private void InstantiateEnvironment() {
        GameObject environmentPrefabToSpawn = activeChalleges[Random.Range(0, activeChalleges.Count)];
        GameObject newSpawn = Instantiate(environmentPrefabToSpawn, new Vector3(0, 0, Mathf.Ceil(player.transform.position.z + ((initialEnvironmentPrefabs+1) * 26))), Quaternion.identity) as GameObject;
    }

    private void SwapEnvironment() {
        activeChalleges.RemoveAt(0);

        int _randomEntry = Random.Range(0, newChallengeMaxEntry);
        activeChalleges.Add(challengeCandidates[_randomEntry]);
        challengeCandidates.RemoveAt(_randomEntry);
    }
}
