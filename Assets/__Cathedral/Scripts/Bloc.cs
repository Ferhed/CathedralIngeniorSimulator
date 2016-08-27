using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bloc : MonoBehaviour
{
    void Awake()
    {

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

    public void DetachBloc()
    {
        transform.parent = Cathedral.Instance.transform;
        rigidBody = gameObject.AddComponent<Rigidbody2D>();
        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 0.19f;

        CheckForDestroyOnBot();

        var gap = collider.size.x / 2.0f;
        var ray_left = transform.position + Vector3.left * gap * 0.9f - Vector3.up * gap * 1.1f;
        var ray_right = transform.position + Vector3.right * gap * 0.9f - Vector3.up * gap * 1.1f;
        
        RaycastHit2D[] hit_left = Physics2D.RaycastAll(ray_left, Vector3.down, 0.1f);
        RaycastHit2D[] hit_right = Physics2D.RaycastAll(ray_right, Vector3.down, 0.1f);
        
        if (hit_left.Length > 0 ||hit_right.Length > 0 )
        {
           rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            StartCoroutine(ActivePhysics());
        }
    }

    public IEnumerator ActivePhysics()
    {
        yield return new WaitForEndOfFrame();

        rigidBody.constraints = RigidbodyConstraints2D.None;
        
        var gap = collider.size.x / 2.0f;
        var ray_left = transform.position + Vector3.left * gap * 0.9f + Vector3.up * gap * 1.1f;
        var ray_right = transform.position + Vector3.right * gap * 0.9f + Vector3.up * gap * 1.1f;

        RaycastHit2D hit_left = Physics2D.Raycast(ray_left, Vector3.up, 0.1f);
        RaycastHit2D hit_right = Physics2D.Raycast(ray_right, Vector3.up, 0.1f);

        if(hit_left.collider != null)
        {
            StartCoroutine(hit_left.transform.GetComponent<Bloc>().ActivePhysics());
        }
        if (hit_right.collider != null)
        {
            StartCoroutine(hit_right.transform.GetComponent<Bloc>().ActivePhysics());
        }

        /*RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.up, 0.2f);
        if(hits.Length > 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider.tag == "Bloc"
                    && hit.transform != transform
                    )
                {
                    StartCoroutine(hit.transform.GetComponent<Bloc>().ActivePhysics());
                }
            }
        }*/
    }

    public IEnumerator DestroyMe()
    {
        IsWeakOnTop();
        Destroy(GetComponent<SpriteRenderer>());
        yield return new WaitForSeconds(timeToDestruct);

        StartCoroutine(ActivePhysics());
        
        yield return new WaitForSeconds(0.1f);

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

    private void CheckForDestroyOnBot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.down, 0.2f);

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
    private float velocityForDestruct = 0.2f;
    [SerializeField]
    private bool topWeakness = false;
    [SerializeField]
    private bool botWeakness = false;
    [SerializeField]
    private bool leftWeakness = false;
    [SerializeField]
    private bool rightWeakness = false;

    private List<Transform> brothers = new List<Transform>();
    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
}
