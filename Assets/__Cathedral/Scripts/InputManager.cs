using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        SetCursor(cursorTextureNormal);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)
            && Canva.Instance.GameIsLaunched
            )
        {
            if(daddyBloc == null)
            {
                ScreenMouseRay();
            }
            else
            {
                daddyBloc.GoPosition();
            }
        }
    }

    public void ScreenMouseRay()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0.0f;

        Vector2 v = Camera.main.ScreenToWorldPoint(mousePosition);

        Collider2D col = Physics2D.OverlapPoint(v);

        if (col != null)
        {
            if (col.transform.tag == "Bloc")
            {
                col.transform.GetComponent<Bloc>().LaunchDestroyMe();
            }
            else if (col.transform.tag == "DaddyBloc")
            {
                var daddy = col.transform.GetComponent<DaddyBloc>();
                daddy.stopBloc();
                daddyBloc = daddy;
            }
            else if (col.transform.tag == "Kamikaze")
            {
                var kamikaze = col.transform.GetComponent<Kamikaze>();
                kamikaze.Explosion();
                Cathedral.Instance.kamikazeKilled++;
            }
            ResetCursor();
        }
        else
        {
            Collider2D collider = Physics2D.OverlapCircle(v, overlapRange);
            if (collider != null && collider.tag == "DaddyBloc")
            {
                var daddy = collider.transform.GetComponent<DaddyBloc>();
                daddy.stopBloc();
                daddyBloc = daddy;
            }
        }
    }

    public Texture2D cursorTextureNormal;
    public Texture2D cursorTextureKamikaze;
    public Texture2D cursorTextureBloc;
    public CursorMode cursorMode = CursorMode.Auto;

    public void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, new Vector2(cursor.width/2,cursor.height/2), cursorMode);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(cursorTextureNormal, new Vector2(cursorTextureNormal.width / 2, cursorTextureNormal.height / 2), cursorMode);
    }

    [SerializeField]
    private float overlapRange = 0.3f;

    private DaddyBloc daddyBloc;
}