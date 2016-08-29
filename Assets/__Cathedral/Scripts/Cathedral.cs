using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Cathedral : MonoBehaviour
{
    public static Cathedral Instance { get; private set; }
    public float MaxHeight { get; private set; }
    public float MaxBuildHeight { get { return maxBuildHeight; } }
    public BoxCollider2D GuideCollider { get; private set; }
    public float MaxTime = 45f;

    void Awake()
    {
        Instance = this;
        MaxHeight = 0.0f;
    }

    public void Init()
    {
        Invoke("spawnKamikaze", Random.RandomRange(spawnEnemySpeedMin, spawnEnemySpeedMax));
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
        Invoke("spawnKamikaze", Random.Range(spawnEnemySpeedMin, spawnEnemySpeedMax));
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
        if(pilgrimNumber > pelerinForHouse)
        {
            pelerinForHouse += 10;
            float posX;
            int side = Random.Range(0, 100);
            if (side > 50)
                posX = Random.Range(cathedraleWidth / 2f * blocSize, maxPelerinX);
            else
                posX = Random.Range(minPelerinX, -cathedraleWidth / 2f * blocSize);
            GameObject tmp = Instantiate(house, new Vector2(posX, 24.0f), Quaternion.identity) as GameObject;
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
        for (int i = 0; i < CameraScript.Instance.transform.childCount; i++)
        {
            Destroy(CameraScript.Instance.transform.GetChild(0).gameObject);
        }
        
        var height = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, Screen.height)).y;
        GameObject go = Instantiate(blink, new Vector3(0.0f,height, 0.0f), Quaternion.identity)as GameObject;
        go.transform.localScale = new Vector3(345f, height,0.0f);

        if (Submit()            
            && MaxHeight > heightMinimum            
            && pilgrimNumber > pilgrimNeeded
            )
        {

            MusicManager.instance.GetComponent<AudioSource>().volume /= 2f;
            InputManager.Instance.GetComponent<AudioSource>().PlayOneShot(winSound);
            Invoke( "Win",4.0f);
        }
        else
        {
            go.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            MusicManager.instance.GetComponent<AudioSource>().volume /= 2f;
            InputManager.Instance.GetComponent<AudioSource>().PlayOneShot(loseSounds[Random.Range(0, loseSounds.Count)]);
            Invoke("Reset", 4.0f);
            Invoke("RelaunchScene", 9.0f);
        }
    }

    private void RelaunchScene()
    {
        SceneManager.LoadScene(0);
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
    [SerializeField]
    private GameObject house;
    [SerializeField]
    private GameObject blink;
    [Header("VictoryValues")]
    [SerializeField]
    private float scoreAcceptance = 65.0f;
    [SerializeField]
    private float heightMinimum = 500.0f;
    [SerializeField]
    private int pilgrimNeeded= 10;
    [Header("Value")]
    [SerializeField]
    private float spawnEnemySpeedMin = 3.0f;
    [SerializeField]
    private float spawnEnemySpeedMax = 5.0f;
    [SerializeField]
    private AudioClip winSound;
    [SerializeField]
    private List<AudioClip> loseSounds = new List<AudioClip>();

    public int totalPelerin = 0;
    public int kamikazeKilled = 0;
    public int pelerinKilled = 0;

    private List<Pelerin> pilgrims = new List<Pelerin>();
    private int pilgrimNumber = 0;
    private float completion;
    private float minPelerinX;
    private float maxPelerinX;
    private int pelerinForHouse = 10;
}
