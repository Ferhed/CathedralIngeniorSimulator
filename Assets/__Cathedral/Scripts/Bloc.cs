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

        var ray_left = transform.position + Vector3.left * collider.size.x / 2.0f - Vector3.up * collider.size.y / 2.0f;
        var ray_right = transform.position + Vector3.right * collider.size.x / 2.0f - Vector3.up * collider.size.y / 2.0f;

        RaycastHit2D hit_left = Physics2D.Raycast(ray_left, Vector3.down, 0.3f);
        RaycastHit2D hit_right = Physics2D.Raycast(ray_right, Vector3.down, 0.3f);
        
        if (hit_left.collider != null ||hit_right.collider != null )
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.up, 0.5f);
        if(hit.collider != null
            && hit.transform.tag == "Bloc"
            )
        {
            var bloc = hit.transform.GetComponent<Bloc>();
            bloc.ActivePhysics();
        }
    }

    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
}
