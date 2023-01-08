using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera cam;
    public GameObject target;
    private Vector3 velocity;
    public float cameraLerp = 0.1f;

    public bool smooth = true;
    public bool follow = true;
    public int defaultZoom = 10;
    public int zoom;
    public float lerpSpeed = 0.2f;

    private void Start()
    {
        cam.orthographicSize = defaultZoom;
        zoom = defaultZoom;
    }

    void FixedUpdate()
    {
        if (follow)
        {
            if (smooth)
            {
                Vector3 point = Camera.main.WorldToViewportPoint(target.transform.position);
                Vector3 delta = target.transform.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
                Vector3 destination = transform.position + delta;
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, cameraLerp);
            }
            else
            {
                transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), lerpSpeed);
        }
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, lerpSpeed);
    }
}
