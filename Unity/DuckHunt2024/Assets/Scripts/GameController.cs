/*******
 * DUCK HUNT
 * Version : 2.0 - 04.2024
 * Hoang Minh Quan
 * http://khoahocvui.vn
 * *****/
using System.Collections;
using System.Collections.Generic;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject beginLayer;
    public GameObject blackLayer;
    public GameObject gameLayer;
    public GameObject lbScore;
    public GameObject lbTime;
    public GameObject ScoreTextPrefap;
    public GameObject gameOverLayer;
    public GameObject topMenuLayer;
    public List<GameObject> duckInstanceArr;


    List<Duck> duckArr;
   
    public GameObject lbOverScore;
    public List<AudioClip> audioArr;
    private int gameScore;
    private int gameTime;
    public int gameStatus;
    bool isReadSensor = false;
    public int gunDamage = 1;
    ComController comController;

    // Start is called before the first frame update
    void Start()
    {
        duckArr = new List<Duck>();
        HideBlackScreen();
        gameOverLayer.SetActive(false);
        beginLayer.SetActive(true);
        topMenuLayer.SetActive(false);
        gameLayer.SetActive(false);
        gameScore = 0;
        gameStatus = -1;

      

    }

    public void StartComConnect()
    {

        comController = GetComponent<ComController>();
        comController.CreatePortWithCallback(() => {
            StartGame(); // start game
        });
    }

    // start game
    public void StartGame()
    {
        gameStatus = 0;
        gameTime = 90;
        beginLayer.SetActive(false);
        gameLayer.SetActive(true);
        gameOverLayer.SetActive(false);
        topMenuLayer.SetActive(true);
        AddNewBird();
        SetTimeOut(2, () =>
        {
            AddNewBird();
        });
        setScore(0);
        updateTime(gameTime);
    }

    // Add a new bird
    public void AddNewBird()
    {
        //Debug.Log("new bird");
        int k = Random.Range(0, duckInstanceArr.Count - 1);
        Duck duck = Instantiate(duckInstanceArr[k], gameLayer.transform).GetComponent<Duck>();
        duckArr.Add(duck);
        duck.randomFly();
    }

    // show black screen
    public void ShowBlackScreen()
    {
        blackLayer.SetActive(true);
        /*foreach (Duck d in duckArr)
        {
            d.white_circle.SetActive(true);
        }*/
    }
    // show white circle from Duck
    public void ShowWhiteCircle(bool val, int index)
    {
        duckArr[index].white_circle.SetActive(val);
    }
    // hide black screen
    public void HideBlackScreen()
    {
        blackLayer.SetActive(false);
        foreach (Duck d in duckArr)
        {
            d.white_circle.SetActive(false);
        }
    }

    // trigger press begin
    public void ShootBegin()
    {
        ShowBlackScreen();
        GetComponent<AudioSource>().clip = audioArr[0];
        GetComponent<AudioSource>().Play();
    }

    // onDamage to the bird

    public void OnDamage(int duckIndex)
    {
        Debug.Log("on damage");
        if (gameStatus != 0)
        {
            return;
        }
        // this version only support one duck on screen!!!!
        Duck duck = duckArr[duckIndex];
        duck.onDamge(gunDamage);
        if (duck.hp == 0)
        {
            ScoreObject lb = Instantiate(ScoreTextPrefap, gameLayer.transform).GetComponent<ScoreObject>();
            lb.ShowWithPosition(new Vector3( duck.transform.localPosition.x, duck.transform.localPosition.y + 1, duck.transform.localPosition.z), duck.score);         
            setScore(gameScore + duck.score);        
            duckArr.RemoveAt(duckIndex);
            AddNewBird();
        }
        GetComponent<AudioSource>().clip = audioArr[1];
        GetComponent<AudioSource>().Play();

    }
    // Update is called once per frame
    float eTime = 0;
    void Update()
    {
        if (gameStatus < 0)
            return;
        eTime += Time.deltaTime;
        if (eTime > 1)
        {
            eTime = 0;
            updateTime(gameTime - 1);
        }
        if (gameTime == 0)
        {
            GameOver();
        }
        UpdateDuck();
    }

    float delayTime = 0.2f;
    float sensorTime = 0;
    int fpsState = 0;
    int fpsTotal = 0;
    int trigerState = 0;
    int sensorState = 0;

    float delayShoot = 0.0f;
    int duckSelect = 0;
    void UpdateDuck()
    {
        string s;
        if (gameStatus < 0)
            return;
        if (ComController.spCom.IsOpen && gameStatus == 0)
        {
            try
            {
                s = ComController.spCom.ReadLine();
                string[] proArr = s.Split(","); // [x,y] - x: trigger , y : sensor state
                trigerState = int.Parse(proArr[0]);
                sensorState = int.Parse(proArr[1]);

                delayShoot -= Time.deltaTime;
                if (trigerState == 1 && !isReadSensor && delayShoot < 0)
                {
                    isReadSensor = true;
                    duckSelect = 0;
                    fpsTotal = 0;
                    ShootBegin(); // show black screen and hide white circle
                    delayShoot = 0.5f; // delay 0.5s
                }
                if (isReadSensor)
                {
                    sensorTime += Time.deltaTime;
                  
                    if (duckSelect < duckArr.Count && duckArr.Count > 0)
                    {
                        if (fpsTotal == 0)
                        {                         
                            ShowBlackScreen();
                            ShowWhiteCircle(true,duckSelect); // show white circle in bird at index 
                            sensorTime = 0;
                            fpsState = 0;
                        }
                        Duck duck = duckArr[duckSelect];
                        fpsTotal += 1;
                        fpsState += sensorState;
                        //Debug.Log(sensorTime + " / " + fpsTotal);
                        if (sensorTime > 0.2f * delayTime)
                        {
                            HideBlackScreen();
                        }
                        if (sensorTime > 0.3f * delayTime)
                        {
                            ShowWhiteCircle(false, duckSelect);
                        }
                        if (sensorTime > delayTime)
                        {
                            sensorTime = 0;
                            //Debug.Log("receiver : " + s);
                           // Debug.Log("FPS state :" + fpsState + "/" + fpsTotal);
                            if (fpsState > 0 && fpsState < fpsTotal) // last state of sensor is 0 : hit the white circle target.  sensorFps > 0 (ignore other lights)
                            {
                                isReadSensor = false;
                                OnDamage(duckSelect);                               
                             //   Debug.Log("On Damage!");
                            }
                            else
                            {
                                duckSelect++;
                            }                           
                            fpsTotal = 0;
                        }
                      
                    }
                    else
                    {
                        isReadSensor = false;
                    }

                }
            }
            catch
            {
                return;
            }
        }

    }
    public void updateTime(int gameTime_)
    {
        gameTime = gameTime_;
        int m = gameTime / 60;
        int s = gameTime % 60;
        string ss = s.ToString();
        if (s < 10) ss = "0" + s;
        string str = "0" + m + ":" + ss;
        lbTime.GetComponent<Text>().text = str;
    }

    public void setScore(int score_)
    {
        gameScore = score_;
        string str = score_.ToString();
        if (score_ >= 100 && score_ < 1000)
        {
            str = "00" + score_;
        }
        else if (score_ >= 1000 && score_ < 10000)
        {
            str = "0" + score_;
        }
        if (score_ >= 1000 && score_ < 10000)
        {
            str = "0" + score_;
        }
        else if (score_ < 10)
        {
            str = "0000" + score_;
        }
        lbScore.GetComponent<Text>().text = str;
    }

    public void GameOver()
    {
        gameOverLayer.SetActive(true);
        lbOverScore.GetComponent<TMPro.TMP_Text>().text = gameScore.ToString();
        GetComponent<AudioSource>().clip = audioArr[3];
        GetComponent<AudioSource>().Play();
        gameStatus = -1;
        duckArr.Clear();
        foreach (Transform child in gameLayer.transform)
        {
            Destroy(child.gameObject);
        }        
    }

    public void PlayAgain()
    {
        duckArr.Clear();
        StartGame();
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
