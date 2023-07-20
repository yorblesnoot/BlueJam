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
    WORLDDECK
    //others
}

public class Tutorial : MonoBehaviour
{
    [SerializeField] TMP_Text tutorialText;
    public static TMP_Text TutorialText;

    [SerializeField] GameObject tutorialBubble;
    public static GameObject TutorialBubble;

    public static TutorialFor activeTutorial;


    private void Awake()
    {
        if(tutorialText != null) TutorialText = tutorialText;
        if (tutorialBubble != null) TutorialBubble = tutorialBubble;
    }

    //tutorial states: 0 not activated, 1+ in progress, -1 complete
    public static void Initiate(TutorialFor tutorial, TutorialFor requirement)
    {
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1 || PlayerPrefs.GetInt(tutorial.ToString()) == -1) return;
        if (requirement != TutorialFor.MAIN && PlayerPrefs.GetInt(requirement.ToString()) != -1) return;
        //if (activeTutorial != TutorialFor.MAIN) return;
        PlayerPrefs.SetInt(tutorial.ToString(), 1);
    }

    public static void EnterStage(TutorialFor tutorial, int stage, string dialogue)
    {
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1 || PlayerPrefs.GetInt(tutorial.ToString()) != stage) return;
        TutorialBubble.SetActive(true);
        TutorialText.text = dialogue;
    }
    public static void CompleteStage(TutorialFor tutorial, int stage, bool complete = false)
    {
        if (PlayerPrefs.GetInt(nameof(TutorialFor.MAIN)) == -1 || PlayerPrefs.GetInt(tutorial.ToString()) != stage) return;
        TutorialBubble.SetActive(false);
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
