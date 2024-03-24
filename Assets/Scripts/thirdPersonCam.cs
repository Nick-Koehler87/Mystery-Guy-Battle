using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;


public class thirdPersonCam : NetworkBehaviour
{
    
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;

    [Header("Set Value")]
    public float rotationSpeed;

    // Start is called before the first frame update
    public override void OnNetworkSpawn () {
        // lock and hid cursor
        Cursor.lockState =  CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    private void FixedUpdate() { 
        // if not owner, exit
        if (!IsOwner) return; 
        // this script is attached to the camera so transform is the cam pos
        // sets orientation to face the direction the camera is pointing
        orientation.forward = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        
        // use unity built in input.  this is good also if I ever add controler suport
        float horizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical"); 

        // this takes the input and applies it so that it lines up with the curent orientation
        // this means when I press forward, I will go in the direction the camera is facing
        Vector3 inputDir = orientation.forward * VerticalInput + orientation.right * horizontalInput;

        // if there is input, call the rpc
        // done to save resources
        if (inputDir != Vector3.zero) {
            updateServerRpc(inputDir);
        } 

        
    } 
    [ServerRpc]
    private void updateServerRpc(Vector3 inputDir) {  
        // target rotation is the direction you want to be facing
        Quaternion targetRotation = Quaternion.LookRotation(inputDir.normalized, Vector3.up);
        // slerp is a function that smoothly rotates to a specific direction 
        playerObj.rotation = Quaternion.Slerp(playerObj.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // update on client.  Without this you wont see the changes made here on the client but you will see them on the host
        updateClientRpc(playerObj.rotation);
    }

    [ClientRpc]
    private void updateClientRpc(Quaternion rotation) {
        // see updateServerRpc()
        playerObj.rotation = rotation;
    }
}
