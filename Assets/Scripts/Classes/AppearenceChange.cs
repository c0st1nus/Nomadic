using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AppearenceChange : MonoBehaviour
{
    public Camera mainCamera;
    public Camera SecondaryCamera;
    public GameObject[] points;
    public GameObject textGroup;
    public String[] names = new String[] { "Цвет кожи", "Головной убор", "Верх", "Низ", "Обувь" };
    public GameObject text;
    private int currentPoint = 0;
    public float moveDuration = 1.0f;
    private bool isMoving = false;
    private Coroutine moveCoroutine;
    public Canvas canvas;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f;

    private Vector2 cameraStartPos = new Vector2(718, 1445);
    private Vector2 cameraEndPos = new Vector2(450, 1363);
    private Vector2 textGroupStartPos = new Vector2(0, 141);
    private Vector2 textGroupEndPos = new Vector2(-274, -693);

    void Start()
    {
        Debug.Log("Script is attached and object is active");
        mainCamera.gameObject.SetActive(true);            
        SecondaryCamera.gameObject.SetActive(false);
        textGroup.SetActive(false);
        text.gameObject.SetActive(true);
        canvas.worldCamera = mainCamera;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left arrow pressed");
            if (!isMoving)
            {
                ChangeModel_Previous();
            }
            MoveCameraToCurrentPoint();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right arrow pressed");
            if (!isMoving)
            {
                ChangeModel_Next();
            }
            MoveCameraToCurrentPoint();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                if (Mathf.Abs(swipeDelta.x) > swipeThreshold)
                {
                    if (swipeDelta.x < 0)
                    {
                        Debug.Log("Swipe left detected");
                        if (!isMoving)
                        {
                            ChangeModel_Next();
                        }
                        MoveCameraToCurrentPoint();
                    }
                    else if (swipeDelta.x > 0)
                    {
                        Debug.Log("Swipe right detected");
                        if (!isMoving)
                        {
                            ChangeModel_Previous();
                        }
                        MoveCameraToCurrentPoint();
                    }
                }
            }
        }
    }

    public void ChangeModel_Next()
    {
        if (currentPoint < points.Length - 1)
        {
            currentPoint++;
        }
        else
        {
            currentPoint = 0;
        }
    }

    public void ChangeModel_Previous()
    {
        if (currentPoint > 0)
        {
            currentPoint--;
        }
        else
        {
            currentPoint = points.Length - 1;
        }
    }

    public void MoveCameraToCurrentPoint()
    {
        Vector2 targetPosition = points[currentPoint].transform.position;
        MoveCameraToPosition(targetPosition);
    }

    public void MoveCameraToPosition(Vector2 targetPosition)
    {
        if (isMoving)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            mainCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z);
            isMoving = false;
        }
        else
        {
            moveCoroutine = StartCoroutine(SmoothMove(targetPosition));
        }
    }

    public void ButtonClick(bool right){
        if (right){
            print($"Элемент {names[currentPoint]} изменен на следующий");
        }
        else{
            print($"Элемент {names[currentPoint]} изменен на предыдущий");
        }
    }

    private IEnumerator SmoothMove(Vector2 targetPosition)
    {
        if (currentPoint == 0){
            mainCamera.gameObject.SetActive(true);            
            SecondaryCamera.gameObject.SetActive(false);
            textGroup.SetActive(false);
            text.gameObject.SetActive(true);
            text.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = names[currentPoint];
            canvas.worldCamera = mainCamera;
        }
        else{
            textGroup.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = names[currentPoint];
            textGroup.SetActive(true);
            text.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(false);            
            SecondaryCamera.gameObject.SetActive(true);
            canvas.worldCamera = SecondaryCamera;
            
            isMoving = true;

            Transform cameraTransform = SecondaryCamera.transform;
            Vector3 startPosition = cameraTransform.position;
            Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, cameraTransform.position.z);
            float elapsedTime = 0;

            while (elapsedTime < moveDuration)
            {
                cameraTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cameraTransform.position = endPosition;
            isMoving = false;
        }
    }
}