using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Canva : MonoBehaviour {

    public static Canva Instance { get; private set; }
    public bool GameIsLaunched = false;

    void Awake()
    {
        Instance = this;
    }

    public void LaunchGame()
    {
        panelMenu.gameObject.SetActive(false);
        panelGame.gameObject.SetActive(true);
        Invoke("ActiveBoolean", 1.0f);
    }

    private void ActiveBoolean()
    {
        GameIsLaunched = true;
    }

    [SerializeField]
    private RectTransform panelGame;
    [SerializeField]
    private RectTransform panelMenu;
}
