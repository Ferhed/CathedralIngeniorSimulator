﻿using UnityEngine;
using System.Collections;

public class DaddyBloc : MonoBehaviour {

    public bool IsMovableByMouse { get; private set; }

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public int pelerinToSpawn;
    public bool released = false;

    void Start () {
        collider.isTrigger = true;
        foreach(Collider2D col in GetComponentsInChildren<Collider2D>())
            col.isTrigger = true;
        launchLenght = Mathf.Abs(transform.position.x) * 2f;
        launchHeight = Mathf.Max( Cathedral.Instance.MaxHeight, 250.0f) * 1.5f;
        launchHeight += (Random.Range(-heightVariance, heightVariance))*launchHeight;
        launchDuration += (Random.Range(-0.5f, 0.5f)) * launchDuration;
        rotationSpeed += (Random.Range(-0.2f, 0.2f)) * rotationSpeed;
    }

    void FixedUpdate()
    {
        if(IsMovableByMouse)
        {
            position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.y = Mathf.Max(position.y, Cathedral.Instance.MaxHeight + gapAboveTheMaxHeight);
            transform.position = Vector2.Lerp(transform.position, position, speedToFollowMouse);
            Vector3 tmp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tmp.z = 0;
            blinkInstance.transform.position = transform.position;
            ghost.transform.position = tmp;
            blinkInstance.transform.localScale = new Vector3(blinkInstance.transform.localScale.x, Vector3.Distance(transform.position, tmp), 0.0f);
        }
        transform.Rotate(transform.forward, rotationSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            if(spawnOnGround)
                spawnPrefabGround();
            else if(isLaunched)
            {
                // FX
                Destroy(gameObject);
            }
        }
        if (collision.transform.tag.Equals("Zone") && released)
        {
            Cathedral.Instance.SpawnPelerin(pelerinToSpawn);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            if (!isLaunched)
                isLaunched = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var y = new Vector3(0.0f,collider.size.y / 2.0f);
        var x = new Vector3( collider.size.x / 2.0f,0.0f);
        bool CanSurvive= false;

        Vector2[] coins =
        {
            transform.position + x + y ,
            transform.position + x - y ,
            transform.position - x + y ,
            transform.position - x - y ,
        };
        foreach(Vector2 coin in coins)
        {
            if(Cathedral.Instance.GuideCollider.bounds.Contains(coin))
            {
                CanSurvive = true;
            }
        }

        if (!CanSurvive)
        {
            Destroy(collider);
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            if (collision.transform.tag == "Zone")
                Debug.Log("I'm in !");
            foreach (Bloc bloc in transform.GetComponentsInChildren<Bloc>())
            {
                bloc.DetachBloc(true);
            }
            Destroy(gameObject);

            return;
        }
        

        Destroy(collider);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        if (collision.transform.tag == "Zone")
            Debug.Log("I'm in !");
        foreach (Bloc bloc in transform.GetComponentsInChildren<Bloc>())
        {
            bloc.DetachBloc(false);
        }
        Destroy(gameObject);
    }

    public void stopBloc()
    {
        InputManager.Instance.GetComponent<AudioSource>().PlayOneShot(grab, 1f);
        StopCoroutine("launch");
        collider.isTrigger = false;
        EffectManager.Instance.InstantiateFx(EffectManager.Instance.picking, transform.position);
        RotateBlock();
        ghost = Instantiate(ghostPrefab) as GameObject;
        blinkInstance = Instantiate(blinkPrefab) as GameObject;
        blinkInstance.transform.localScale = new Vector3(collider.size.x, 0.0f, 0.0f);
        IsMovableByMouse = true;
    }

    public void GoPosition()
    {
        released = true;
        Destroy(blinkInstance);
        IsMovableByMouse = false;
        Destroy(ghost);
        //RotateBlock();
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, collider.size, 0, Vector2.down);
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.transform.GetComponent<DaddyBloc>())
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + collider.size.y / 2.0f + teleportHeight, 0.0f);
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                break;
            }
        }
        if (Cathedral.Instance.GuideCollider.bounds.Contains(transform.position))
        {
            Cathedral.Instance.SpawnPelerin(pelerinToSpawn);
            released = false;
        }
        EffectManager.Instance.InstantiateFx(EffectManager.Instance.fallOff, transform.position);
    }

    private void spawnPrefabGround()
    {
        // spawn a prefab on ground Collision (house, tree)
    }

    IEnumerator launch(AnimationCurve curve)
    {
        
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        launchLenght = Mathf.Abs(transform.position.x) * 2f;
        float ratio;
        bool right = transform.position.x > 0;
        while((ratio = (Time.time-startTime)/launchDuration) <= 1)
        {
            float posX;
            float posY;
            if (right)
            {
                posX = ratio * -launchLenght;
                posY = curve.Evaluate((1 - ratio)) * launchHeight;
            }
            else
            {
                posX = ratio* launchLenght;
                posY = curve.Evaluate(ratio) * launchHeight;
            } 
            transform.position = startPosition + new Vector3(posX, posY, 0f);
            yield return null;
        }
    }

    private void RotateBlock()
    {
        rotationSpeed = 0.0f;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        /*if (Vector3.Dot(Vector3.up, transform.up) > 0.5f)
        {
            transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f);
        }
        else if (Vector3.Dot(Vector3.up, -transform.up) > 0.5f)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        }
        else if (Vector3.Dot(Vector3.up, transform.right) > 0.5f)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
        }*/
    }

    void OnMouseEnter()
    {
        InputManager.Instance.SetCursor(InputManager.Instance.cursorTextureBloc);
    }

    void OnMouseExit()
    {
        InputManager.Instance.SetCursor(InputManager.Instance.cursorTextureNormal);
    }

    [Header("Tweak")]
    [SerializeField]
    private float launchDuration;
    [SerializeField]
    private float launchHeight;
    [SerializeField]
    private float launchLenght;
    [SerializeField]
    private float heightVariance;
    [SerializeField]
    private bool spawnOnGround = false;
    [SerializeField]
    private float teleportHeight = 1.0f;
    [SerializeField]
    private float speedToFollowMouse = 10.0f;
    [SerializeField]
    private float gapAboveTheMaxHeight = 10.0f;
    [SerializeField]
    private float rotationSpeed = 150.0f;
    [SerializeField]
    private float ghostAlpha = 0.5f;
    [SerializeField]
    private GameObject ghostPrefab;
    [SerializeField]
    private GameObject blinkPrefab;
    [SerializeField]
    private AudioClip grab;
    [SerializeField]
    private AudioClip drop;

    private BoxCollider2D collider;
    private bool isLaunched = false;
    private Vector2 position;
    private GameObject ghost;
    private GameObject blinkInstance;
}
