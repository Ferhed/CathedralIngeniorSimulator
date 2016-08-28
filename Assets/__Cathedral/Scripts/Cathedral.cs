using UnityEngine;
using System.Collections;

public class Cathedral : MonoBehaviour
{
    public static Cathedral Instance { get; private set; }
    public float MaxHeight { get; private set; }

    void Awake()
    {
        Instance = this;
        MaxHeight = 0.0f;
    }
    // Use this for initialization
    void Start()
    {
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2) + (((cathedraleHeight * blocSize) / 2f) * blocSize);
        GameObject guideInstance = Instantiate(guide, new Vector2(0f, posY), Quaternion.identity) as GameObject;
        guideInstance.transform.localScale = new Vector2(cathedraleWidth * blocSize, cathedraleHeight * blocSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(daddyPrefab, new Vector3(mouse_position.x, mouse_position.y, 0.0f), Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Submit();
        }
    }

    public void Submit()
    {
        float score = EvaluateCathedrale();
        Debug.Log("Completion at : " + (int)(score * 100) + "%");
        if (score > scoreAcceptance)
            Debug.Log("You win");
        else
            Debug.Log("You lose, noob ;)");
    }

    public void SubmitMyHeight(float height)
    {
        if (height > MaxHeight)
        {
            MaxHeight = height;
            Debug.Log(MaxHeight);
        }
    }

    public float EvaluateCathedrale()
    {
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2) + (((cathedraleHeight * blocSize) / 2f) * blocSize);
        float surface = cathedraleWidth * cathedraleHeight;
        RaycastHit2D[] cols = Physics2D.BoxCastAll(new Vector2(0f, posY), new Vector2((cathedraleWidth * blocSize)/2f, (cathedraleHeight * blocSize)/2f) , 0.0f, Vector2.zero);
        int count = 0;
        foreach (RaycastHit2D hit in cols)
        {
            if (hit.transform.tag == "Bloc")
                ++count;
        }
        return count / surface;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2) + (((cathedraleHeight * blocSize) / 2f) * blocSize);
        Gizmos.DrawCube(new Vector3(0f, posY, 0f), new Vector3((cathedraleWidth * blocSize)/2f, (cathedraleHeight * blocSize) / 2f, 0f));
    }

    [SerializeField]
    private DaddyBloc daddyPrefab;
    [SerializeField]
    private float cathedraleWidth;
    [SerializeField]
    private float cathedraleHeight;
    [SerializeField]
    private float scoreAcceptance;
    [SerializeField]
    private GameObject guide;
    [SerializeField]
    private float blocSize;


}
