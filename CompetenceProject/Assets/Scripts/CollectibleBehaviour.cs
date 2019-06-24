using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehaviour : MonoBehaviour {

    //private Animator animator;
    private Rigidbody rb;
    public Vector3 newPosition;
    public float runSpeed;
    public bool collected;

    private void Start() {
        //animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        newPosition = new Vector3(0, 0, runSpeed);
        collected = false;
    }

    private void Update() {
        if (!collected && GameManager.instance.charactersCanMove) {
            rb.velocity = newPosition;
        }
        else {
            rb.velocity = Vector3.zero;
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            collected = true;
            GameManager.instance.GatherCollectible(gameObject, GetComponent<Animator>());
        }
    }
    
}
