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
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2f) + (((cathedraleHeight) / 2f)*blocSize);
        GameObject guideInstance = Instantiate(guide, new Vector2(0f, posY), Quaternion.identity) as GameObject;
        guideInstance.transform.localScale = new Vector2(cathedraleWidth, cathedraleHeight);
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPelerin(1);
        }
        minPelerinX = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        maxPelerinX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x;
    }

    public float getCompletion()
    {
        return completion;
    }

    public void SpawnPelerin(int number)
    {
        for(int i = number;  number>0; number--)
        {
            float posX;
            float side = Random.Range(-1, 1);
            if (side > 0)
                posX = Random.Range(cathedraleWidth * blocSize, maxPelerinX);
            else
                posX = Random.Range(minPelerinX, -cathedraleWidth * blocSize);
            // spawn with a deltaX and then move toward his destination
            Instantiate(pelerinPrefab, new Vector2(posX, pelerinPrefab.transform.position.y), Quaternion.identity);
            // probably launch a coroutine with fade and movement
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
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2f) + ((cathedraleHeight * blocSize) / 2f);
        RaycastHit2D[] cols = Physics2D.BoxCastAll(new Vector2(0f, posY), new Vector2(cathedraleWidth * blocSize, cathedraleHeight * blocSize) , 0.0f, Vector2.zero);
        float surface = cathedraleWidth * cathedraleHeight;
        int count = 0;
        foreach (RaycastHit2D hit in cols)
        {
            if (hit.transform.tag == "Bloc")
            {
                ++count;
            }
        }
        return completion = (count / surface);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2f) + ((cathedraleHeight * blocSize) / 2f);
        Gizmos.DrawCube(new Vector3(0f, posY, 0f), new Vector3(cathedraleWidth * blocSize, cathedraleHeight * blocSize, 0f));
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
    [SerializeField]
    private GameObject pelerinPrefab;

    private float completion;
    private float minPelerinX;
    private float maxPelerinX;


}
