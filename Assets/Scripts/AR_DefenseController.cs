using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR_DefenseController : MonoBehaviour
{
    public GameObject Cross;
    public GameObject Arrow;
    public GameObject WinText;
    public AudioSource AS;
    public AudioClip loopClip;

    private bool startedLoopSound;

    private PlacementController PC;
    private bool gameStarted;
    private float speed = 20;
    private Transform mainCam;

    private float projectileRespawnTime;

    private void Start() {
        mainCam = Camera.main.transform;
        PC = GetComponent<PlacementController>();
        Cross = GameObject.Find("Cross");
        Cross.SetActive(false);
        WinText.SetActive(false);


    }

    public void StartGame() {
        PC.StartButton.SetActive(false);
        PC.RemovePlanes();
        PC.gameStarted = true;
        Cross.SetActive(true);
        AS.Play();
        gameStarted = true;
    }

    private void Update() {
        if (gameStarted) {
            foreach (Transform enemy in PC.allPlacedObjects) {
                enemy.position = Vector3.MoveTowards(enemy.position, mainCam.position, Time.deltaTime / speed);
                enemy.LookAt(mainCam);
            }

            projectileRespawnTime += Time.deltaTime;

            if (projectileRespawnTime >= 2) {
                Transform projectile = Instantiate(Arrow, mainCam.position, mainCam.rotation).transform;

                //projectile.rotation = mainCam.rotation;
                projectile.localScale /= 10;
                projectile.gameObject.AddComponent<ProjectileController>();
                Rigidbody rg = projectile.gameObject.AddComponent<Rigidbody>();
                rg.useGravity = false;
                rg.AddForce(mainCam.forward * 50);

                projectileRespawnTime = 0;
            }

            if (!AS.isPlaying && !startedLoopSound) {
                AS.clip = loopClip;
                AS.loop = true;
                AS.Play();
                startedLoopSound = true;
            }

        }
    }

    public void win() {
        AS.Stop();
        Cross.SetActive(false);
        WinText.SetActive(true);
        gameStarted = false;
    }


}


public class ProjectileController : MonoBehaviour
{
    private float t;

    private void Update() {
        t += Time.deltaTime;

        if (t >= 10) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("AR_Obj")) {
            List<Transform> allEnemies = FindObjectOfType<PlacementController>().allPlacedObjects;
            allEnemies.RemoveAt(allEnemies.FindIndex(e => e.Equals(collision.transform)));
            
            if(allEnemies.Count == 0) {
                FindObjectOfType<AR_DefenseController>().win();
            }

            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }


}