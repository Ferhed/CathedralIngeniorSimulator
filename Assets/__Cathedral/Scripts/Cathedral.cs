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
    }

    public void Init()
    {
        Invoke("spawnKamikaze", Random.RandomRange(2f, 5f));
    }
    // Use this for initialization
    void Start()
    {
        float tmpSize = blocSize/512f;
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        float posY = ground.transform.position.y + (ground.transform.localScale.y * blocSize / 2f) + ((cathedraleHeight * blocSize*0.9f) / 2f);
        GameObject guideInstance = Instantiate(guide, new Vector2(0f, posY), Quaternion.identity) as GameObject;
        guideInstance.transform.localScale = new Vector2(cathedraleWidth*tmpSize, cathedraleHeight* tmpSize);
        GameObject guideInstance2 = Instantiate(guide, new Vector2(0f, posY), Quaternion.identity) as GameObject;
        guideInstance2.transform.localScale = new Vector2(cathedraleWidth, maxBuildHeight);
        Destroy(guideInstance2.GetComponent<SpriteRenderer>());
        GuideCollider = guideInstance2.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.A))
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
        }*/
        minPelerinX = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        maxPelerinX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x;
    }
    
    void spawnKamikaze()
    {
        if(!Canva.Instance.GameIsLaunched)
        {
            return;
        }

        GameObject kami;
        int side = Random.Range(0, 2);
        if (side == 0)
        {
            var y_spawn = Mathf.Max(50f, Random.Range(50f, MaxHeight))*1.5f;
            kami = Instantiate(kamikazePrefab, new Vector2(minPelerinX-50f,y_spawn), Quaternion.identity)as GameObject;
            Kamikaze kamikaze = kami.GetComponent<Kamikaze>();
        }
        else
        {
            var y_spawn = Mathf.Max(50f, Random.Range(50f, MaxHeight))*1.5f;
            kami = Instantiate(kamikazePrefab, new Vector2(maxPelerinX + 50f, y_spawn), Quaternion.identity) as GameObject;
            Kamikaze kamikaze = kami.GetComponent<Kamikaze>();
        }
        Invoke("spawnKamikaze", Random.Range(4.0f, 6.0f));
    }

    public float getCompletion()
    {
        return completion;
    }

    public void SpawnPelerin(int number)
    {
        pilgrimNumber += number;
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
            pilgrims.Add(tmp.GetComponent<Pelerin>());
            totalPelerin++;
        }
    }

    public bool Submit()
    {
        float score = EvaluateCathedrale();
        if (score > (scoreAcceptance/100f))
            return true;
        else
            return false;
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

    public void SubmitToGod()
    {
        if(Submit()            
            && MaxHeight > heightMinimum            
            && pilgrimNumber > pilgrimNeeded
            )
        {
            Win();
        }
        else
        {
            Reset();
        }
    }

    private void Win()
    {
        Time.timeScale = 0;
        Canva.Instance.transform.GetChild(0).gameObject.SetActive(false);
        Canva.Instance.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void Reset()
    {
        foreach(Bloc bloc in GetComponentsInChildren<Bloc>())
        {
            bloc.LaunchActivePhysics();
        }
        MaxHeight = 0.0f;
        CameraScript.Instance.ChangeDistance();
        
        for(int i = 0; i< CameraScript.Instance.transform.childCount;i++)
        {
            Destroy(CameraScript.Instance.transform.GetChild(0).gameObject);
        }

        while(pilgrims.Count>0)
        {
            var pilgr = pilgrims[0];
            pilgr.Death();
            pilgrims.Remove(pilgr);
        }
        var go = Instantiate(StartBlock);
        go.transform.parent = transform;
        go.transform.position = new Vector3(0.0f, 77.0f, 0.0f);
    }

    [SerializeField]
    private DaddyBloc daddyPrefab;
    [SerializeField]
    private float cathedraleWidth;
    [SerializeField]
    private float cathedraleHeight;
    [SerializeField]
    private GameObject guide;
    [SerializeField]
    private float blocSize;
    [SerializeField]
    private float GuideSize;
    [SerializeField]
    private List<GameObject> pelerinPrefabs = new List<GameObject>();
    [SerializeField]
    private float maxBuildHeight = 600.0f;
    [SerializeField]
    private GameObject kamikazePrefab;
    [SerializeField]
    private GameObject StartBlock;
    [Header("VictoryValues")]
    [SerializeField]
    private float scoreAcceptance = 65.0f;
    [SerializeField]
    private float heightMinimum = 500.0f;
    [SerializeField]
    private int pilgrimNeeded= 10;

    public int totalPelerin = 0;
    public int kamikazeKilled = 0;
    public int pelerinKilled = 0;

    private List<Pelerin> pilgrims = new List<Pelerin>();
    private int pilgrimNumber = 0;
    private float completion;
    private float minPelerinX;
    private float maxPelerinX;
    
}
