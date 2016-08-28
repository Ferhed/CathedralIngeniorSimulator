﻿using UnityEngine;
using System.Collections;

public class Kamikaze : MonoBehaviour {

    public bool GoToTheRight = false;

	void Awake()
    {
        deplacementActive = true;
        if (transform.position.x < 0)
        {
            Vector3 tmp = transform.localScale;
            tmp.x*=-1;
            transform.localScale = tmp;
            GoToTheRight = true;
        }
    }

    void FixedUpdate()
    {
        if(deplacementActive)
        {
            int random = Random.Range(0,4);
            switch(random)
            {
                case 0:
                case 3:
                    StartCoroutine("DeplacementYolloh");
                    break;
                case 1:
                    StartCoroutine("LoopingYolloh");
                    break;
                case 2:
                    StartCoroutine("SinusYolloh");
                    break;
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(LoopingYolloh());
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(SinusYolloh());
        }
    }

    private IEnumerator LoopingYolloh()
    {
        deplacementActive = false;

        var go = new GameObject();
        go.transform.position = transform.position + Vector3.up * loopingRadius;
        transform.parent = go.transform;
        var rotation = 0.0f;
        var time = 0.0f;
        while(time < 1.0f)
        {
            go.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
            if(GoToTheRight)
            {
                rotation += Time.deltaTime*360.0f;
            }
            else
            {
                rotation -= Time.deltaTime * 360.0f;
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        go.transform.rotation = Quaternion.identity;
        transform.parent = null;
        Destroy(go);

        deplacementActive = true;    
    }

    private IEnumerator DeplacementYolloh()
    {
        deplacementActive = false;
        float time = 0.0f;
        while (time < 1.0f)
        {
            if (GoToTheRight)
                transform.position = transform.position + Vector3.right * Time.deltaTime * speed;
            else
                transform.position = transform.position + Vector3.left * Time.deltaTime * speed;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        deplacementActive = true;
    }

    private IEnumerator SinusYolloh()
    {
        deplacementActive = false;
        float startY = transform.position.y;
        var time = 0.0f;
        float posY = startY;
        while (time <1.0f)
        {
            posY = startY + Mathf.Sin(time * (2 * Mathf.PI)) * sinusAmplitude;
            float posX;
            if (GoToTheRight)

                posX = transform.position.x + Time.deltaTime * speed;
            else
                posX = transform.position.x - Time.deltaTime * speed;
            transform.position = new Vector2(posX, posY);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame(); 
        }
        deplacementActive = true;

    }

    [SerializeField]
    private float speed = 50.0f;
    [SerializeField]
    private float sinusAmplitude = 50.0f;
    [SerializeField]
    private float loopingRadius = 50.0f;

    private bool deplacementActive = true;
}
