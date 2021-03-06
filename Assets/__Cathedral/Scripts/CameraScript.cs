﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraScript : MonoBehaviour {

    public static CameraScript Instance { get; private set; }
	
    void Awake()
    {
        Instance = this;
        camera = GetComponent<Camera>();
    }

	// Update is called once per frame
	void Update () {
        if(!Canva.Instance.GameIsLaunched)
        {
            return;
        }

        var size_lerp = Mathf.Lerp(camera.orthographicSize, size, changeSpeed);
        var position_lerp = Mathf.Lerp(transform.position.y, positionY, changeSpeed);

        transform.position = new Vector3(transform.position.x, position_lerp, transform.position.z);
        camera.orthographicSize = size_lerp;
    }

    public void ChangeDistance()
    {
        var height = Mathf.Max(minSize, Cathedral.Instance.MaxHeight);
        height = Mathf.Min(maxSize, height);
        positionY = height;
        size = height+30.0f;
    }

    public void ScreenShake()
    {
        transform.DOShakePosition(0.5f,screenShake);
    }

    [SerializeField]
    private float changeSpeed = 1.0f;
    [SerializeField]
    private float minSize = 250.0f;
    [SerializeField]
    private float maxSize = 600.0f;
    [SerializeField]
    private float screenShake = 10.0f;

    private Camera camera;
    private float positionY = 220.0f;
    private float size = 250.0f;
}
