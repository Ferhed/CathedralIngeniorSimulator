using UnityEngine;
using System.Collections;

public class DaddyBloc : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Collider2D>().isTrigger = true;
        foreach(Collider2D col in GetComponentsInChildren<Collider2D>())
            col.isTrigger = true;
        launchLenght = Mathf.Abs(transform.position.x) * 2f;
        launchHeight = Random.Range(1f, 1.3f)*launchHeight;
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            if(spawnOnGround)
                spawnPrefabGround();
            else 
            {
                // FX
                Destroy(gameObject);
            }
        }
        else 
        {
            Destroy(gameObject.GetComponent<Collider2D>());
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            foreach(Bloc bloc in transform.GetComponentsInChildren<Bloc>())
            {
                bloc.DetachBloc();
            }
            Destroy(gameObject);
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
    private bool spawnOnGround = false;

}
