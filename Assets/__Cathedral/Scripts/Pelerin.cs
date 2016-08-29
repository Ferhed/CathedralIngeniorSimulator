using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pelerin : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.tag == "Bloc" || (col.transform.tag == "DaddyBloc" && col.GetComponent<DaddyBloc>().released))
        {
            Death();
        }
    }

    public void Death()
    {
        // blood FX
        GameObject tmp = deathMarks[Random.Range(0, deathMarks.Count)];
        Instantiate(tmp, new Vector2(transform.position.x, tmp.transform.position.y), Quaternion.identity);
        EffectManager.Instance.InstantiateFx(EffectManager.Instance.blood, transform.position);
        Destroy(gameObject,0.1f);
        Cathedral.Instance.pelerinKilled++;
    }

    [SerializeField]
    private List<GameObject> deathMarks = new List<GameObject>();
}
