using UnityEngine;
using System.Collections;

public class DaddyBloc : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collision)
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
