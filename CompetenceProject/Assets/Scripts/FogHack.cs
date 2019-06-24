using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHack : MonoBehaviour {

    public GameObject player;
    public Vector3 newPosition;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void UpdateRealtivePositionToPlayer() {
        transform.position = player.transform.position + newPosition;
    }
}
