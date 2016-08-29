using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {
    public static EffectManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void InstantiateFx(GameObject fx, Vector3 position)
    {
        GameObject go = Instantiate(fx, position, Quaternion.identity) as GameObject;
        Destroy(go, 1.0f);
    }

    public GameObject blood;
    public GameObject kamikaze;
    public GameObject fallOff;
}
