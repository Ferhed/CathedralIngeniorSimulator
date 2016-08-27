using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Launcher : MonoBehaviour {


	// Use this for initialization
	void Start () 
    {
        InvokeRepeating("launch", 0f, lauchRate);
	}
	
    private void launch()
    {
        float posX = Random.Range(-spawnRange, spawnRange);
        if(posX>0)
            posX+=startSpawnRange;
        else
            posX-=startSpawnRange;
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
        GameObject instance = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform.position, Quaternion.identity) as GameObject;
        instance.GetComponent<DaddyBloc>().StartCoroutine("launch", curve);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pointPosition = transform.position;
        Gizmos.DrawSphere(pointPosition + Vector3.right * startSpawnRange, .5f);
        Gizmos.DrawSphere(pointPosition - Vector3.right * startSpawnRange, .5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pointPosition + Vector3.right * (startSpawnRange + spawnRange), .5f);
        Gizmos.DrawSphere(pointPosition - Vector3.right * (startSpawnRange + spawnRange), .5f);
    }

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private float lauchRate;

    [SerializeField]
    private float spawnRange;

    [SerializeField]
    private float startSpawnRange;

    [SerializeField]
    private List<GameObject> prefabs = new List<GameObject>();
}
