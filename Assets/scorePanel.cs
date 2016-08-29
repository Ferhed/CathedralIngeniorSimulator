using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scorePanel : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        pelerinKilled.text = "" + Cathedral.Instance.pelerinKilled;
        pelerinAlive.text = "" + (Cathedral.Instance.totalPelerin-Cathedral.Instance.pelerinKilled);
        cathedralMaxHeight.text = "" + Cathedral.Instance.MaxHeight;
        kamikazeKilled.text = "" + Cathedral.Instance.kamikazeKilled;
    }

    [SerializeField]
    private Text pelerinKilled;
    [SerializeField]
    private Text pelerinAlive;
    [SerializeField]
    private Text cathedralMaxHeight;
    [SerializeField]
    private Text kamikazeKilled;
}
