using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {

	// Use this for initialization
	void Start () {
        for(int i = 0; i<transform.GetChildCount(); i++)
        {
            Color tmp = transform.GetChild(i).GetComponent<SpriteRenderer>().color;
            tmp.a = ghostAlpha;
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = tmp;
        }
	
	}
	

    [SerializeField]
    private float ghostAlpha;
}
