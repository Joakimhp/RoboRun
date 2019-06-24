using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseActivator : MonoBehaviour {

    public EnemyBehavour enemyBehavour;

    private void Start() {
        enemyBehavour = transform.GetComponentInParent<EnemyBehavour>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            enemyBehavour.canChase = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            enemyBehavour.canChase = false;
        }
    }
}
