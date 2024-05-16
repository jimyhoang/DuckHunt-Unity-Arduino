using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    public TMP_Text lbText;
    private Canvas cv;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowWithPosition(Vector3 pos , int score)
    {
        lbText = gameObject.transform.Find("Canvas/lb").GetComponent<TMP_Text>();
        SetScore(score);
        gameObject.transform.localPosition = pos;      
        SetTimeOut(1, () => {           
            Destroy(gameObject);
        });

    }
    public void SetScore(int score)
    {
        if (lbText != null)
        {
            lbText.text = "+" + score;
            float scale_ = score/100.0f;
            gameObject.transform.localScale = new Vector3(scale_, scale_, scale_);
        }
       
    }
    // hen thoi gian sau do call 1 action
    public Coroutine SetTimeOut(float delay, System.Action action)
    {
        return StartCoroutine(WaitAndDo(delay, action));
    }

    // doi 1 thoi gian chay ham.
    private IEnumerator WaitAndDo(float time, System.Action action = null)
    {
        yield return new WaitForSeconds(time);
        if (action != null)
            action();
    }
}
