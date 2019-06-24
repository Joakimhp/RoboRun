using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamHit : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            GameManager.instance.KillPlayer(other.gameObject);
        }
    }
}
