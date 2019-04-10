﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuckInGap : MonoBehaviour
{
    public GameObject lamprey;

    bool playerIsStuck;

    // Taken from LampreyAI script so that it doesn't need a door to work and will attack the player when they get stuck in a gap

    [SerializeField] float moveTime;

    public GameObject drone;

    bool isLunging;

    bool findingPlayer;

    Vector3 currentPos;

    List<GameObject> waypoints;

    GameObject startWaypoint;

    void Start()
    {
        drone = GameObject.FindGameObjectWithTag("Drone");
        currentPos = transform.position;
        waypoints = new List<GameObject>();

        playerIsStuck = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Diver")
        {
            // Spawn lamprey to kill them and then restarts at checkpoint
            lamprey.SetActive(true);
            playerIsStuck = true;
        }
    }

    void Update()
    {
        //Only activate when the lamprey
        if (playerIsStuck)
        {

            AudioManager.instance.PlayOneShot("Monster1e", 1f);

            //If you can see the player, move to the player
            if (CanSeePlayer())
            {
                if (!isLunging)
                {
                    StartCoroutine(JumpAtPlayer());
                }
            }
            //Else find a waypoint that can see the player
            else if (!findingPlayer)
            {
                findingPlayer = true;
                waypoints.Clear();
                FindWaypointMap();
                StartCoroutine(MoveBetweenPoints());
            }
        }
    }

    IEnumerator MoveBetweenPoints()
    {
        waypoints.Reverse();
        waypoints.Add(drone);
        for (int i = 0; i < waypoints.Count; i++)
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(currentPos, waypoints[i].transform.position, (elapsedTime / moveTime));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            currentPos = transform.position;
        }
    }

    void FindWaypointMap()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Waypoint");

        FindStartingPoint();

        foreach (GameObject go in gos)
        {
            Vector3 dir = go.transform.position - (drone.transform.position + transform.up);
            RaycastHit hit;

            int layerMask = 1 << 12;

            if (Physics.Raycast(drone.transform.position + transform.up, dir, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.gameObject.tag == "Waypoint")
                {
                    //Find the waypoint that can see the player then work backwards. What waypoint can see this waypoint? What about that one?
                    waypoints.Add(hit.transform.gameObject);
                    if (FindWaypoint(0))
                    {
                        return;
                    }
                    waypoints.Remove(hit.transform.gameObject);
                    //If that was not the correct waypoint check other waypoints
                }
            }
        }

        Debug.Log("ERROR! UNABLE TO FIND PATH");
    }

    void FindStartingPoint()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Waypoint");

        int layerMask = 1 << 12;

        //First find the starting point
        foreach (GameObject go in gos)
        {
            Vector3 dir = go.transform.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.gameObject.tag == "Waypoint")
                {
                    startWaypoint = hit.transform.gameObject;
                    return;
                }
            }
        }
    }

    bool FindWaypoint(int index)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (GameObject go in gos)
        {
            Vector3 dir = go.transform.position - waypoints[index].transform.position;
            RaycastHit hit;

            int layerMask = 1 << 12;

            if (Physics.Raycast(waypoints[index].transform.position, dir, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.gameObject.tag == "Waypoint")
                {
                    if (hit.transform.gameObject == startWaypoint)
                    {
                        waypoints.Add(hit.transform.gameObject);
                        //Found the start, return all the way up
                        return true;
                    }
                    if (!waypoints.Contains(hit.transform.gameObject))
                    {
                        waypoints.Add(hit.transform.gameObject);
                        if (FindWaypoint(index + 1))
                        {
                            return true;
                        }
                        waypoints.Remove(hit.transform.gameObject);
                        //If that was not the correct waypoint check other waypoints
                    }
                }
            }
        }
        return false;
    }

    IEnumerator JumpAtPlayer()
    {
        AudioManager.instance.PlayOneShot("Monster2e", 1f);
        isLunging = true;
        float elapsedTime = 0.0f;
        while (elapsedTime < moveTime)
        {
            gameObject.transform.position = Vector3.Lerp(currentPos, drone.transform.position, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SceneLoader.instance.LoseGame();
    }

    bool CanSeePlayer()
    {
        Vector3 dir = drone.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Drone")
            {
                return true;
            }
        }
        return false;
    }
}