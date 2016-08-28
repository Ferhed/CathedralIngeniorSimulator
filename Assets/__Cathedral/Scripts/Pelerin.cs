using UnityEngine;
using System.Collections;

public class Pelerin : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.tag == "Bloc")
        {
            Death();
        }
    }

    private void Death()
    {
        Instantiate(deathMark, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    [SerializeField]
    private GameObject deathMark;
}
