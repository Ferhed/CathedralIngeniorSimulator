using UnityEngine;
using System.Collections;
using UnityEditor;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

        Collider2D[] col = Physics2D.OverlapPointAll(v);

        if (col.Length > 0)
        {
            foreach (Collider2D c in col)
            {
                if (c.transform.tag == "Bloc")
                {
                    StartCoroutine(c.transform.GetComponent<Bloc>().DestroyMe());
                }
                else if (c.transform.tag == "DaddyBloc")
                {
                    var daddy = c.transform.GetComponent<DaddyBloc>();
                    daddy.stopBloc();
                    daddyBloc = daddy;
                    break;
                }
            }
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
    [SerializeField]
    private float overlapRange = 0.3f;

    private DaddyBloc daddyBloc;
}