using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController_PileDriverMinigame1 : MonoBehaviour
{
    public static GameController_PileDriverMinigame1 instance;
    public Camera mainCamera;
    public PileDriver_PileDriverMinigame1 pileDriverObj;
    public List<GameObject> listPit = new List<GameObject>();
    public int level = 1;
    public List<int> listIndexPitActive = new List<int>();
    public Button btnPile;
    public ProgressBar_PileDriverMinigame1 progressBar;
    public GameObject thisMole;
    public bool isLose, isWin, isSetUping, isProgressUping;
    public int CountNextLevel;
    private List<GameObject> listPitTmp = new List<GameObject>();
    public Canvas canvas;
    public GameObject tutorial1, tutorial2;
    public bool isTutorial1, isTutorial2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        isLose = false;
        isWin = false;
        isSetUping = false;
        isProgressUping = false;
        isTutorial1 = true;
        isTutorial2 = true;
    }

    private void Start()
    {
        SetSizeCamera();
        for (int v = 2; v < 6; v++)
        {
            canvas.transform.GetChild(2).GetChild(v).GetChild(0).gameObject.SetActive(false);
        }
        SetUpLevel(level);
        btnPile.onClick.AddListener(Pile);
        tutorial1.SetActive(false);
        tutorial2.SetActive(false);

        progressBar.current_progress = 0;
        progressBar.max_progress = 100;
        progressBar.Bar.fillAmount = progressBar.current_progress / progressBar.max_progress;
    }

    void SetSizeCamera()
    {
        float f1 = 16.0f / 9;
        float f2 = Screen.width * 1.0f / Screen.height;

        mainCamera.orthographicSize *= f1 / f2;
    }

    void SetUpLevel(int levelIndex)
    {
        CountNextLevel = 0;
        isSetUping = true;
        List<int> listCheckSame = new List<int>();
        listIndexPitActive.Clear();
        int ran = -1;
        for (int n = 0; n < listPit.Count; n++)
        {
            listPit[n].transform.GetChild(2).gameObject.SetActive(false);
            listCheckSame.Add(n);
        }


        int iWithoutLoop = 0;
        for (int i = 0; i < levelIndex + 2; i++)
        {
            ran = Random.Range(0, listCheckSame.Count);
            listIndexPitActive.Add(listCheckSame[ran]);
            listCheckSame.RemoveAt(ran);

            listPit[listIndexPitActive[i]].transform.GetChild(2).transform.localPosition = new Vector2(0, -1);
            listPit[listIndexPitActive[i]].transform.GetChild(2).gameObject.SetActive(true);
            listPit[listIndexPitActive[i]].transform.GetChild(2).transform.DOLocalMoveY(0.5f, 2).OnComplete(() =>
            {
                listPit[listIndexPitActive[iWithoutLoop]].transform.GetChild(2).transform.DOLocalMoveY(-1, 2).OnComplete(() =>
                {
                    if (isSetUping)
                    {
                        isSetUping = false;
                    }
                    if (isTutorial1)
                    {
                        isTutorial1 = false;
                        tutorial1.transform.position = new Vector2(-7.06f, -4.05f);
                        tutorial1.SetActive(true);
                        tutorial1.transform.DOMove(new Vector2(-8.66f, -4.35f), 1).SetLoops(-1);
                    }
                });
                iWithoutLoop++;

            });
        }
    }

    void Pile()
    {
        if (pileDriverObj.isReadyPile && !isLose && !isWin)
        {
            if (tutorial2.activeSelf)
            {
                tutorial2.SetActive(false);
                tutorial2.transform.DOKill();
                pileDriverObj.speed = 10;
            }
            if (pileDriverObj.isPitHaveMole)
            {
                GameObject tmpMole = thisMole;
                listPitTmp.Add(thisMole);
                thisMole = null;
                tmpMole.transform.DOLocalMoveY(0.5f, 1);
                tmpMole.transform.parent.GetComponent<CapsuleCollider2D>().enabled = false;
                CountNextLevel++;
                if (CountNextLevel == listIndexPitActive.Count)
                {

                    if (level == 4)
                    {
                        Debug.Log("Win");
                        isWin = true;
                        pileDriverObj.transform.DOMoveX(pileDriverObj.transform.position.x + 20, 2).OnComplete(() =>
                        {
                            pileDriverObj.StopAllCoroutines();
                        });
                    }
                    if (level < 4)
                    {
                        pileDriverObj.transform.DOMove(new Vector2(-8.48f, -0.05f), 1).OnComplete(() =>
                        {
                            foreach (GameObject x in listPitTmp)
                            {
                                x.transform.parent.GetComponent<CapsuleCollider2D>().enabled = true;
                            }
                            level++;
                            SetUpLevel(level);
                        });
                    }
                    isProgressUping = true;
                }
            }
            else
            {
                Debug.Log("Lose");
                isLose = true;
                btnPile.enabled = false;
                pileDriverObj.variableJoystick.enabled = false;
                for (int i = 0; i < listIndexPitActive.Count; i++)
                {
                    if ((Vector2)listPit[listIndexPitActive[i]].transform.GetChild(2).transform.localPosition == new Vector2(0, -1))
                    {
                        listPit[listIndexPitActive[i]].transform.GetChild(2).DOLocalMoveY(0.5f, 1);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (isProgressUping)
        {
            progressBar.current_progress += Time.deltaTime * 50;
            progressBar.Bar.fillAmount = progressBar.current_progress / progressBar.max_progress;
            if (progressBar.current_progress > (100f * level) / 4)
            {
                isProgressUping = false;
                progressBar.current_progress = (100f * level) / 4;
                GameObject tmpStar = canvas.transform.GetChild(2).GetChild(level + 1).GetChild(0).gameObject;
                tmpStar.SetActive(true);
                tmpStar.transform.DOScale(1.5f, 0.5f).OnComplete(() =>
                {
                    tmpStar.transform.DOScale(1, 0.5f);
                });

            }
        }
    }
}


