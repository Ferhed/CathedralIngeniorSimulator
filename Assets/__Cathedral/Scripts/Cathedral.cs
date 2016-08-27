using UnityEngine;
using System.Collections;

public class Cathedral : MonoBehaviour
{
    public static Cathedral Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.A))
        {
            var mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(daddyPrefab, new Vector3(mouse_position.x, mouse_position.y, 0.0f) , Quaternion.identity);
        }
	}

    [SerializeField]
    private DaddyBloc daddyPrefab;
}
