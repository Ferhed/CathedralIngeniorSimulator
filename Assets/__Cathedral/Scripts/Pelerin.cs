using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pelerin : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("AIE trigger");
        if(col.transform.tag == "Bloc" || (col.transform.tag == "DaddyBloc" && col.GetComponent<DaddyBloc>().released))
        {
            Death();
        }
    }

    private void Death()
    {
        // blood FX
        GameObject tmp = deathMarks[Random.Range(0, deathMarks.Count)];
        Instantiate(tmp, new Vector2(transform.position.x, tmp.transform.position.y), Quaternion.identity);
        Destroy(gameObject);
    }

    [SerializeField]
    private List<GameObject> deathMarks = new List<GameObject>();
}
