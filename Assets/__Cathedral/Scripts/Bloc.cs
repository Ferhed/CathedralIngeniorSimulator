using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bloc : MonoBehaviour
{
    public bool IsActivated { get; private set; }

    void Awake()
    {
        IsActivated = false;

        foreach(Transform tr in transform.parent.GetComponentsInChildren<Transform>())
        {
            if(tr != transform)
            {
                brothers.Add(tr);
            }
        }
    }

    private void FixedUpdate()
    {
        if( rigidBody!= null
            && rigidBody.velocity.y > velocityForDestruct)
        {
            CheckForDestroyOnBot();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsActivated)
        {
            if (collision.transform.tag.Equals("Ground"))
            {
               StartCoroutine(DestroyMe());
            }
        }
    }

    public void DetachBloc(bool destroy_now)
    {
        transform.parent = Cathedral.Instance.transform;
        rigidBody = gameObject.AddComponent<Rigidbody2D>();
        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 19.0f;

        CheckForDestroyOnBot();

        if(transform.position.y >Cathedral.Instance.MaxBuildHeight)
        {
            StartCoroutine(ActivePhysics());
            return;
        }

        if(destroy_now)
        {
            StartCoroutine(ActivePhysics());
            return;
        }

        var gap = collider.size.x / 2.0f;
        var ray_left = transform.position + Vector3.left * gap * 0.9f - Vector3.up * gap * 0.9f;
        var ray_right = transform.position + Vector3.right * gap * 0.9f - Vector3.up * gap * 0.9f;
        
        RaycastHit2D[] hit_left = Physics2D.RaycastAll(ray_left, Vector3.down, 10.0f);
        RaycastHit2D[] hit_right = Physics2D.RaycastAll(ray_right, Vector3.down, 10.0f);
        
        if (hit_left.Length > 0 ||hit_right.Length > 0 )
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            Invoke("SendMyHeight", 1.0f);
        }
        else
        {
            StartCoroutine(ActivePhysics());
        }
    }

    public void LaunchActivePhysics()
    {
        StartCoroutine(ActivePhysics());
    }

    private IEnumerator ActivePhysics()
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody2D>();
            collider = GetComponent<BoxCollider2D>();
        }

        IsActivated = true;
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.color = brokenColor;
        }
        yield return new WaitForEndOfFrame();

        rigidBody.constraints = RigidbodyConstraints2D.None;
        AllWeakness();
        
        var gap = collider.size.x / 2.0f;
        var ray_left = transform.position + Vector3.left * gap * 0.9f + Vector3.up * gap * 0.9f;
        var ray_right = transform.position + Vector3.right * gap * 0.9f + Vector3.up * gap * 0.9f;

        RaycastHit2D[] hit_left = Physics2D.RaycastAll(ray_left, Vector3.up, 10.0f);
        RaycastHit2D[] hit_right = Physics2D.RaycastAll(ray_right, Vector3.up, 10.0f);
        
        if(hit_left.Length > 0)
        {
            foreach(RaycastHit2D hit in hit_left)
            {
                if(hit.transform != transform)
                {
                    var bloc = hit.transform.GetComponent<Bloc>();
                    if (bloc != null
                        &&!bloc.IsActivated
                        )
                    {
                        bloc.LaunchActivePhysics();
                    }
                }
            }
        }
        if (hit_right.Length > 0)
        {
            foreach (RaycastHit2D hit in hit_right)
            {
                if (hit.transform != transform)
                {
                    var bloc = hit.transform.GetComponent<Bloc>();
                    if (bloc != null
                        && !bloc.IsActivated
                        )
                    {
                        bloc.LaunchActivePhysics();
                    }
                }
            }
        }
    }

    private IEnumerator DestroyMe()
    {        
        Destroy(GetComponent<SpriteRenderer>());
        yield return new WaitForSeconds(timeToDestruct);
        
        StartCoroutine(ActivePhysics());
        
        yield return new WaitForSeconds(0.1f);

        DestroyGameObject();
    }

    public void LaunchDestroyMe()
    {
        StartCoroutine(DestroyMe());
    }

    private void DestroyGameObject()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public bool IsWeakOnTop()
    {
        if(Vector3.Dot(Vector3.up,transform.up)> 0.5f)
        {
            return topWeakness;
        }
        else if(Vector3.Dot(Vector3.up, -transform.up) > 0.5f)
        {
            return botWeakness;
        }
        else if (Vector3.Dot(Vector3.up, transform.right) > 0.5f)
        {
            return rightWeakness;
        }
        else
        {
            return leftWeakness;
        }
    }

    private void AllWeakness()
    {
        botWeakness = true;
        leftWeakness = true;
        rightWeakness = true;
        topWeakness = true;
    }

    private void SendMyHeight()
    {
        Cathedral.Instance.SubmitMyHeight(transform.position.y + collider.size.y / 2.0f );
    }

    private void CheckForDestroyOnBot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.down, 20.0f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform != transform
                && hit.collider.tag == "Bloc"
                && !brothers.Contains(hit.transform)
                )
            {
                var bloc = hit.transform.GetComponent<Bloc>();
                if (bloc.IsWeakOnTop())
                {
                    Destroy(bloc.gameObject);
                }
            }
        }
    }

    [SerializeField]
    private float timeToDestruct = 0.5f;
    [SerializeField]
    private bool topWeakness = false;
    [SerializeField]
    private bool botWeakness = false;
    [SerializeField]
    private bool leftWeakness = false;
    [SerializeField]
    private bool rightWeakness = false;
    [SerializeField]
    private Color brokenColor  = Color.red;

    private List<Transform> brothers = new List<Transform>();
    private float velocityForDestruct = 5.0f;
    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
}
