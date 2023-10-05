using System;
using TMPro;
using UnityEngine;

public enum TutorialFor
{
    MAIN,
    WORLDMOVE,
    WORLDPICKUPS,
    WORLDHEAL,
    WORLDITEM,
    WORLDREMOVE,
    WORLDBATTLE,
    WORLDBOSS,
    BATTLEACTIONS,
    BATTLEDAMAGE,
    BATTLEBARRIER,
    BATTLEDODAMAGE,
    WORLDCRAFTING,
    WORLDDECK,
    WORLDCRAFTREMINDER,
    WORLDVEHICLE,
    WORLDORB,
    WORLDIMPASSABLE
    //others
}

public class Tutorial : MonoBehaviour
{
    [SerializeField] TMP_Text tutorialText;
    public static TMP_Text TutorialText;

    [SerializeField] GameObject tutorialBubble;
    public static GameObject TutorialBubble;

    public static TutorialFor activeTutorial;
    public static Tutorial Master;

    public static Vector3 BubblePosition;


    private void Awake()
    {
        Master = this;
        TutorialText = tutorialText;
        TutorialBubble = tutorialBubble;
        BubblePosition = TutorialBubble.transform.localPosition;
    }

    //tutorial states: 0 not activated, 1+ in progress, -1 complete
    public static void Initiate(TutorialFor tutorial, TutorialFor requirement)
    {
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1 || PlayerPrefs.GetInt(tutorial.ToString()) == -1) return;
        if (requirement != TutorialFor.MAIN && PlayerPrefs.GetInt(requirement.ToString()) != -1) return;
        PlayerPrefs.SetInt(tutorial.ToString(), 1);
    }

    public static void EnterStage(TutorialFor tutorial, int stage, string dialogue)
    {
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1 || PlayerPrefs.GetInt(tutorial.ToString()) != stage) return;
        if (activeTutorial != tutorial)
        {
            Master.StopAllCoroutines();
            TutorialBubble.transform.localPosition = BubblePosition;
            Master.StartCoroutine(TutorialBubble.SlideIn(.2f, .1f));
            TutorialText.text = dialogue;
        }
        activeTutorial = tutorial;
    }
    public static void CompleteStage(TutorialFor tutorial, int stage, bool complete = false)
    {
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1 || PlayerPrefs.GetInt(tutorial.ToString()) != stage) return;
        TutorialBubble.SetActive(false);
        activeTutorial = TutorialFor.MAIN;
        if (complete)
        {
            PlayerPrefs.SetInt(tutorial.ToString(), -1);
            foreach (string name in Enum.GetNames(typeof(TutorialFor)))
            {
                if (PlayerPrefs.GetInt(name) > -1 && name != nameof(TutorialFor.MAIN))
                {
                    return;
                }
            }
            PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), -1);
        }
        else
        {
            PlayerPrefs.SetInt(tutorial.ToString(), stage + 1);
        }
    }
}
