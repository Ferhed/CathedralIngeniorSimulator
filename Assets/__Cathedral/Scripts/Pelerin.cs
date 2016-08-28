using UnityEngine;
using System.Collections;

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
        //Instantiate(deathMark, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    [SerializeField]
    private GameObject deathMark;
}
