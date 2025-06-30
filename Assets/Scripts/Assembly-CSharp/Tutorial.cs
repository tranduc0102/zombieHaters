using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject firstMoveUI;

    [SerializeField]
    private GameObject MoveUI;

    [SerializeField]
    private GameObject KillZombiesUI;

    [SerializeField]
    private GameObject FindSurvUI;

    [SerializeField]
    private NewSurvivorPointer survivorPointer;

    [SerializeField]
    private float tutorialWavePower = 0.4f;

    [SerializeField]
    private float tutorialSecondWavePower = 0.05f;

    [SerializeField]
    private int pauseTime = 3;

    [SerializeField]
    private Button buttonOK;

    [SerializeField]
    private Animator handAnim;

    private Text buttonOkText;

    private Transform cameraTarget;

    private float movedDistance;

    private Vector3 prevSurvPos;

    private int tutorialStage = -1;

    private bool haterSpawned;

    private float defaultPointerDistance;

    private bool tutorPrepared;

    private void Start()
    {
        StartCoroutine(WaitForScene());
        buttonOK.gameObject.SetActive(true);
    }

    private IEnumerator WaitForScene()
    {
        while (!GameManager.instance.operation.isDone)
        {
            yield return null;
        }
        StartTutorial();
    }

    public void StartTutorial()
    {
        Debug.LogWarning("DON'T CHANGE DEFAULT POSITION OF CameraTarget ON MAIN SCENE");
        cameraTarget = Object.FindObjectOfType<CameraTarget>().transform;
        firstMoveUI.SetActive(true);
        defaultPointerDistance = survivorPointer.showDistance;
        DataLoader.Instance.ResetLocalInfo();
        handAnim.SetTrigger("Joystick");
        buttonOkText = buttonOK.GetComponentInChildren<Text>();
        tutorPrepared = true;
    }
    private bool checkSpawn = false;
    private void Update()
    {
        if (!tutorPrepared)
        {
            return;
        }
        switch (tutorialStage)
        {
            case 0:
                if (cameraTarget.position != Vector3.zero)
                {
                    tutorialStage++;
                    prevSurvPos = cameraTarget.position;
                }
                break;
            case 1:
                if(checkSpawn) return;
                if (movedDistance > 7f)
                {
                    StartCoroutine(Pause());
                    WavesManager.instance.SpawnTutorialWave(tutorialWavePower, 1);
                    checkSpawn = true;
                    break;
                }
                if (movedDistance > 0.35f)
                {
                    firstMoveUI.SetActive(false);
                    if (!haterSpawned)
                    {
                        SpawnManager.instance.TutorialDrop();
                        haterSpawned = true;
                    }
                }
                movedDistance += Vector3.Distance(prevSurvPos, cameraTarget.position);
                prevSurvPos = cameraTarget.position;
                break;
            case 2:
                if (!checkSpawn)
                {
                    return;
                }
                if (GameManager.instance.zombies.Count <= 0)
                {
                    checkSpawn = false;
                    StartCoroutine(Pause());
                }
                break;
            case 3:
                {
                    buttonOK.gameObject.SetActive(false);
                    List<Vector3> list = new List<Vector3>();
                    foreach (SurvivorSpawn survivorSpawn in SpawnManager.instance.survivorSpawns)
                    {
                        if (survivorSpawn.newSurvivor != null)
                        {
                            list.Add(survivorSpawn.transform.position);
                        }
                    }
                    if (list.Count <= 0 && GameManager.instance.zombies.Count <= 0)
                    {
                        tutorialStage++;
                    }
                    else
                    {
                        survivorPointer.showDistance = 10000f;
                    }
                    break;
                }
            case 4:
                DataLoader.Instance.SetPlayerLevel(2);
                DataLoader.Instance.RefreshMoney(200.0, true);
                survivorPointer.showDistance = defaultPointerDistance;
                tutorialStage++;
                FindSurvUI.SetActive(false);
                GameManager.instance.GameOver();
                Object.Destroy(base.gameObject);
                break;
        }
    }

    private IEnumerator Pause()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        switch (tutorialStage)
        {
            case 1:
                KillZombiesUI.SetActive(true);
                break;
            case 2:
                FindSurvUI.SetActive(true);
                break;
        }
        SoundManager.Instance.PlayStepsSound(false);
        Time.timeScale = 0f;
        int pauseTimeRemaining = pauseTime;
        buttonOK.gameObject.SetActive(true);
        buttonOK.interactable = false;
        buttonOkText.color -= new Color(0f, 0f, 0f, 0.5f);
        buttonOkText.text = pauseTimeRemaining.ToString();
        yield return new WaitForSecondsRealtime(0.2f);
        while (pauseTimeRemaining > 0)
        {
            buttonOkText.text = pauseTimeRemaining.ToString();
            yield return new WaitForSecondsRealtime(0.2f);
            for (float i = pauseTime; i > (float)(pauseTime - 1); i -= 0.08f)
            {
                buttonOkText.transform.localScale = Vector3.Lerp(buttonOkText.transform.localScale, Vector3.zero, 0.05f);
                yield return new WaitForSecondsRealtime(0.02f);
            }
            pauseTimeRemaining--;
            buttonOkText.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        buttonOkText.color += new Color(0f, 0f, 0f, 0.5f);
        buttonOkText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Ok);
        buttonOK.interactable = true;
    }

    public void Continue()
    {
        buttonOK.gameObject.SetActive(false);
        switch (tutorialStage)
        {
            case -1:
                tutorialStage++;
                firstMoveUI.SetActive(false);
                break;
            case 1:
                tutorialStage++;
                KillZombiesUI.SetActive(false);
                break;
            case 2:
                tutorialStage++;
                FindSurvUI.SetActive(false);
                break;
        }
        Time.timeScale = 1f;
    }
}
