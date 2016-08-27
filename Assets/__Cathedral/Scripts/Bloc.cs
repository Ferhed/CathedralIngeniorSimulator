using UnityEngine;
using System.Collections;

public class Bloc : MonoBehaviour
{
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public void DetachBloc()
    {
        transform.parent = Cathedral.Instance.transform;
        rigidBody = gameObject.AddComponent<Rigidbody2D>();

        var gap = collider.size.x / 4.0f;
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
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.up, 0.25f);
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
        }
    }

    public IEnumerator DestroyMe()
    {
        Destroy(GetComponent<SpriteRenderer>());
        yield return new WaitForSeconds(timeToDestruct);

        StartCoroutine(ActivePhysics());
        
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    [SerializeField]
    private float timeToDestruct = 0.5f;

    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
}
