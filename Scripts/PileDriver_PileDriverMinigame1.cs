using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PileDriver_PileDriverMinigame1 : MonoBehaviour
{
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody2D rb;
    public Vector2 direction;
    public bool isPlayingCoroutine = false;
    public bool isHoldMouse;
    public Vector2 prePos, currentPos;
    public Camera mainCamera;
    public float screenRatio;
    public bool isReadyPile = false;
    public bool isPitHaveMole = false;

    private void Start()
    {
        screenRatio = Screen.width * 1f / Screen.height;
        speed = 10;
        currentPos = transform.position;
        prePos = currentPos;
        StartCoroutine(UpdatePos());
    }

    IEnumerator UpdatePos()
    {
        while (!isPlayingCoroutine)
        {
            if (isHoldMouse || GameController_PileDriverMinigame1.instance.isSetUping || GameController_PileDriverMinigame1.instance.isWin)
            {
                currentPos = transform.position;
                yield return new WaitForSeconds(0.01f);
                prePos = currentPos;
                yield return new WaitForSeconds(0.01f);
                currentPos = transform.position;
            }
            else
                yield return new WaitForSeconds(0.1f);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHoldMouse = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isHoldMouse = false;
        }

        if (isHoldMouse || GameController_PileDriverMinigame1.instance.isSetUping || GameController_PileDriverMinigame1.instance.isWin)
        {
            if (currentPos.x > prePos.x)
            {
                transform.localScale = new Vector2(-0.5f, 0.5f);
            }
            if (currentPos.x < prePos.x)
            {
                transform.localScale = new Vector2(0.5f, 0.5f);
            }
            if (currentPos.x == prePos.x)
            {
                return;
            }
        }
    }


    public void FixedUpdate()
    {

        if (!GameController_PileDriverMinigame1.instance.isSetUping && !GameController_PileDriverMinigame1.instance.isWin)
        {
            if (variableJoystick.Horizontal != 0 || variableJoystick.Vertical != 0)
            {
                if (GameController_PileDriverMinigame1.instance.tutorial1.activeSelf)
                {
                    GameController_PileDriverMinigame1.instance.tutorial1.SetActive(false);
                    GameController_PileDriverMinigame1.instance.tutorial1.transform.DOKill();
                }
            }
            direction = new Vector2(variableJoystick.Horizontal, variableJoystick.Vertical * 0.7f);
            rb.velocity = direction * speed;
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, -mainCamera.orthographicSize * screenRatio + 2, mainCamera.orthographicSize * screenRatio - 2), Mathf.Clamp(transform.position.y, -mainCamera.orthographicSize + 1, mainCamera.orthographicSize - 1));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            isReadyPile = true;
            GameController_PileDriverMinigame1.instance.btnPile.GetComponent<Image>().color = new Color(1, 1, 1);
            if (collision.transform.GetChild(2).gameObject.activeSelf)
            {
                if (GameController_PileDriverMinigame1.instance.isTutorial2)
                {
                    GameController_PileDriverMinigame1.instance.isTutorial2 = false;
                    GameController_PileDriverMinigame1.instance.tutorial2.transform.position = new Vector2(9.54f, -1.9f);
                    GameController_PileDriverMinigame1.instance.tutorial2.SetActive(true);
                    GameController_PileDriverMinigame1.instance.tutorial2.transform.DOMove(new Vector2(9.74f, -3.3f), 1).SetLoops(-1);
                    speed = 0;
                }
                isPitHaveMole = true;
                for (int i = 0; i < GameController_PileDriverMinigame1.instance.listIndexPitActive.Count; i++)
                {
                    if (collision.gameObject.Equals(GameController_PileDriverMinigame1.instance.listPit[GameController_PileDriverMinigame1.instance.listIndexPitActive[i]]))
                    {
                        GameController_PileDriverMinigame1.instance.thisMole = collision.transform.GetChild(2).gameObject;
                    }
                }
            }
            else
            {
                isPitHaveMole = false;
                GameController_PileDriverMinigame1.instance.thisMole = null;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            isReadyPile = false;
            isPitHaveMole = false;
            GameController_PileDriverMinigame1.instance.btnPile.GetComponent<Image>().color = new Color(0.66f, 0.66f, 0.66f);
            GameController_PileDriverMinigame1.instance.thisMole = null;
        }
    }
}
