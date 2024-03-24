using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    // utility
    System.Random random = new System.Random();

    [Header("Movement Set Value")]
    public float moveSpeed;
    public float rotationSpeed;
    
    [Header("References")]
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public Transform orientation;
    
    [Header("Cam/Sound")]
    [SerializeField] private CinemachineFreeLook vc;
    [SerializeField] private AudioListener audio;

    public override void OnNetworkSpawn () {

        // if owner 
        if (IsOwner) {
            // set all the things
            rb = GetComponent<Rigidbody>();
            // this is here so that you only hear from your pos and not from another client
            audio.enabled = true;
            // enables correct camera 
            vc.Priority = 1;
            // spawnpoint function 
            spawning();
        } else {
            // if not owner disable camera for this client
            vc.Priority = 0;
        }
        
    }

    private void FixedUpdate() {
        // if not owner, exit
        if (!IsOwner) return; 

        // get input 
        float horizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical"); 
        Vector3 inputDir = orientation.forward * VerticalInput + orientation.right * horizontalInput;
        
        // if input call rpc
        if (inputDir != Vector3.zero) {
            updateServerRpc(inputDir);
        }
    }

    [ServerRpc]
    private void updateServerRpc(Vector3 inputDir) {
        // update force on server
        rb.AddForce(inputDir * moveSpeed * 10f, ForceMode.Force);

        // todo 
        // add max speed
        // add friction
        // add jump
    }

    

    private void spawning() {
        // creates array of all objects with spawn point tag 
        // then randomly selects one and sets the transform to the spawnpoints pos
        string spawnpointTag = "SpawnPoint";

        GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag(spawnpointTag);

        int randomNumber = random.Next(0, spawnpoints.Length);

        transform.position = spawnpoints[randomNumber].transform.position;
    }

}
