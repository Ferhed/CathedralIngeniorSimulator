using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cathedral : MonoBehaviour
{
    public static Cathedral Instance { get; private set; }
    public float MaxHeight { get; private set; }
    public float MaxBuildHeight { get { return maxBuildHeight; } }
    public BoxCollider2D GuideCollider { get; private set; }

    void Awake()
    {
        Instance = this;
        MaxHeight = 0.0f;
        Invoke("spawnKamikaze", Random.RandomRange(2f, 5f));
    }
    // Use this for initialization
    void Start()
    {
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2f) + (((cathedraleHeight) / 2f)*blocSize);
        GameObject guideInstance = Instantiate(guide, new Vector2(0f, posY), Quaternion.identity) as GameObject;
        guideInstance.transform.localScale = new Vector2(cathedraleWidth, cathedraleHeight);
        GameObject guideInstance2 = Instantiate(guide, new Vector2(0f, posY), Quaternion.identity) as GameObject;
        guideInstance2.transform.localScale = new Vector2(cathedraleWidth, maxBuildHeight);
        Destroy(guideInstance2.GetComponent<SpriteRenderer>());
        GuideCollider = guideInstance2.GetComponent<BoxCollider2D>();
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
    
    void spawnKamikaze()
    {

        int side = Random.Range(0, 2);
        if (side == 0)
        {
            var y_spawn = Mathf.Max(50f, Random.Range(50f, MaxHeight)-100.0f);
            GameObject kami = Instantiate(kamikazePrefab, new Vector2(minPelerinX-50f,y_spawn), Quaternion.identity)as GameObject;
            Kamikaze kamikaze = kami.GetComponent<Kamikaze>();
            var y_target = Mathf.Max(50f, Random.Range(50f, MaxHeight));
            kamikaze.Direction = (new Vector3(0.0f,y_target)- kami.transform.position).normalized;
        }
        else
        {
            var y_spawn = Mathf.Max(50f, Random.Range(50f, MaxHeight)-100.0f);
            GameObject kami = Instantiate(kamikazePrefab, new Vector2(maxPelerinX + 50f, y_spawn), Quaternion.identity) as GameObject;
            Kamikaze kamikaze = kami.GetComponent<Kamikaze>();
            var y_target = Mathf.Max(50f, Random.Range(50f, MaxHeight));
            kamikaze.Direction = (new Vector3(0.0f, y_target) - kami.transform.position).normalized;
        }
        Invoke("spawnKamikaze", Random.RandomRange(4f, 5f));
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
            int side = Random.Range(0, 100);
            if (side > 50)
                posX = Random.Range(cathedraleWidth/2f * blocSize, maxPelerinX);
            else
                posX = Random.Range(minPelerinX, -cathedraleWidth/2f * blocSize);
            // spawn with a deltaX and then move toward his destination
            GameObject pel = pelerinPrefabs[Random.Range(0, pelerinPrefabs.Count)];
            GameObject tmp = Instantiate(pel, new Vector2(posX, pel.transform.position.y), Quaternion.identity) as GameObject;
            if (side > 50)
                tmp.transform.localScale = new Vector3(-tmp.transform.localScale.x, tmp.transform.localScale.y, tmp.transform.localScale.z);
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
            CameraScript.Instance.ChangeDistance();
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
    private List<GameObject> pelerinPrefabs = new List<GameObject>();
    [SerializeField]
    private float maxBuildHeight = 600.0f;
    [SerializeField]
    private GameObject kamikazePrefab;

    private float completion;
    private float minPelerinX;
    private float maxPelerinX;
}
