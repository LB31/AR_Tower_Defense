using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour
{
    public GameObject PrefabToPlace;
    public GameObject StartButton;
    public List<Transform> allPlacedObjects;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARRaycastManager arRaycastManager;
    private GameObject lastSelectedObject;


    private Vector2 touchPosition;
    public bool gameStarted;

    void Awake() {
        arRaycastManager = GetComponent<ARRaycastManager>();

        StartButton.SetActive(false);
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition) {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;

        return false;
    }

    public void RemovePlanes() {
        ARPlaneManager pm = GetComponent<ARPlaneManager>();
        foreach (ARPlane plane in pm.trackables) {
            plane.gameObject.SetActive(false);
        }
        pm.enabled = false;
    }


    void Update() {
        if (gameStarted) return;

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began) {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject)) {
                    lastSelectedObject = hitObject.transform.gameObject.CompareTag("AR_Obj") ? hitObject.transform.gameObject : null;

                }
            }

            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)) {
                Pose hitPose = hits[0].pose;
                Vector3 pos = hitPose.position += new Vector3(0, PrefabToPlace.transform.lossyScale.y / 2, 0);
                if (lastSelectedObject == null) {  
                    lastSelectedObject = Instantiate(PrefabToPlace, pos, hitPose.rotation);
                    lastSelectedObject.transform.rotation = hitPose.rotation;

                    allPlacedObjects.Add(lastSelectedObject.transform);
                    if (allPlacedObjects.Count >= 5) StartButton.SetActive(true);
                } else {

                    lastSelectedObject.transform.position = pos;
                    lastSelectedObject.transform.rotation = hitPose.rotation;

                }
            }

        }
    }



}