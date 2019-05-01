﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerROVMovement : MonoBehaviour
{
    [Header("Movement Variables")]

    [SerializeField] private string playerControlling; //Syntax "Player0, Player1, Player2, ect"

    [SerializeField] private float playerDirectionalSpeed;

    [SerializeField] private float playerRiseDesendSpeed;

    [SerializeField] private float playerVerticalRotateSpeed;

    [SerializeField] private float playerHorizontalRotateSpeed;

    private bool canMove = true;

    private Player rewiredPlayer;

    private Transform playerTransform;

    private Rigidbody playerRidgedBody;

    public GameObject diver;

    private void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerControlling); //Gets the rewire player
        playerRidgedBody = gameObject.GetComponent<Rigidbody>();
        playerTransform = gameObject.transform;
        diver = GameObject.FindGameObjectWithTag("Diver");
    }

    private void Update()
    {
        ReadInput();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DiverFeet")
        {
            canMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "DiverFeet")
        {
            canMove = true;
        }
    }

    void ReadInput() //Interperates the input of the player
    {
        if (canMove)
        {
            //----------------------------------------MOVE DOWN----------------------------------------------

            if (rewiredPlayer.GetButtonLongPress("ElevateDOWN"))
            {
                MoveDown();
            }

            //----------------------------------------MOVE UP----------------------------------------------

            if (rewiredPlayer.GetButtonLongPress("ElevateUP"))
            {
                MoveUp();
            }

            //----------------------------------------MOVE AXIS----------------------------------------------
            float yAxisMove = rewiredPlayer.GetAxis("UpDownMovment");
            float xAxisMove = rewiredPlayer.GetAxis("LeftRightMovement");

            MoveDirectional(xAxisMove, yAxisMove);
        }

        //----------------------------------------ROTATE AXIS----------------------------------------------
        float xAxisAim = rewiredPlayer.GetAxis("UpDownAim");
        float yAxisAim = rewiredPlayer.GetAxis("LeftRightAim");

        RotatePlayer(xAxisAim, yAxisAim);
    }

    void MoveUp() //Moves them directly up (based on singular button input)
    {
        Vector3 direction = new Vector3(0,1 * playerRiseDesendSpeed,0);

        playerRidgedBody.AddForce(direction);
    }

    void MoveDown() //Moves them directly down (based on singular button input)
    {
        Vector3 direction = new Vector3(0,-1 * playerRiseDesendSpeed,0);

        playerRidgedBody.AddForce(direction);
    }

    void MoveDirectional(float xAxis, float yAxis) //Moves them in a direction based on the axis input
    {
        Vector3 right = new Vector3(playerTransform.right.x, 0, playerTransform.right.z) * xAxis;
        Vector3 forward = new Vector3(playerTransform.forward.x, 0, playerTransform.forward.z) * yAxis;
        Vector3 direction = right + forward;

        playerRidgedBody.AddForce(direction * playerDirectionalSpeed);
    }

    void RotatePlayer(float xAxis, float yAxis) //Rotates them in a direction based on the axis input
    {
        Quaternion direction = playerTransform.rotation;

        float dx = xAxis * playerHorizontalRotateSpeed * -1;
        float dy = yAxis * playerVerticalRotateSpeed;

        float x = direction.eulerAngles.x;
        float y = direction.eulerAngles.y;

        x += dx;
        y += dy;

        if (x > 180)
            x = Mathf.Clamp(x, 280, 360);
        else
            x = Mathf.Clamp(x, -1, 80);

        direction = Quaternion.Euler(new Vector3(x, y, 0.0f)); //* Time.time;

        playerTransform.rotation = direction;
    }

    public IEnumerator StartCutscene(){
        Start();
        Transform diverTransform = diver.transform;
        diver.GetComponent<PlayerDiverMovement>().canMove = false;
        Vector3 currentPos = playerTransform.position;
        float elapsedTime = 0.0f;
        while (elapsedTime < 2){
            playerTransform.position = Vector3.Lerp(currentPos, new Vector3(currentPos.x + 2, currentPos.y, diverTransform.position.z), (elapsedTime / 2));
            playerTransform.LookAt(new Vector3(diverTransform.position.x, diverTransform.position.y + 2, diverTransform.position.z));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currentPos = playerTransform.position;
        elapsedTime = 0.0f;
        while (elapsedTime < 2){
            playerTransform.position = Vector3.Lerp(currentPos, new Vector3(diverTransform.position.x, currentPos.y, diverTransform.position.z - 2), (elapsedTime / 2));
            playerTransform.LookAt(new Vector3(diverTransform.position.x, diverTransform.position.y + 2, diverTransform.position.z));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        diver.GetComponent<PlayerDiverMovement>().canMove = true;
    }
}