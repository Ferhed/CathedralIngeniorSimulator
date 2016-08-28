using UnityEngine;
using System.Collections;

public class DaddyBloc : MonoBehaviour {

    public bool IsMovableByMouse { get; private set; }

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

	void Start () {
        collider.isTrigger = true;
        foreach(Collider2D col in GetComponentsInChildren<Collider2D>())
            col.isTrigger = true;
        launchLenght = Mathf.Abs(transform.position.x) * 2f;
        launchHeight = (Random.Range(0f, heightVariance)+1)*launchHeight;
	}

    void FixedUpdate()
    {
        if(IsMovableByMouse)
        {
            position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.y = Mathf.Max(position.y, Cathedral.Instance.MaxHeight + gapAboveTheMaxHeight);
            transform.position = Vector2.Lerp(transform.position, position, speedToFollowMouse);
        }
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
        Destroy(collider);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        foreach (Bloc bloc in transform.GetComponentsInChildren<Bloc>())
        {
            bloc.DetachBloc();
        }
        Destroy(gameObject);
    }

    public void stopBloc()
    {
        StopCoroutine("launch");
        collider.isTrigger = false;
        IsMovableByMouse = true;
    }

    public void GoPosition()
    {
        IsMovableByMouse = false;
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

    private BoxCollider2D collider;
    private bool isLaunched = false;
    private Vector2 position;
}
