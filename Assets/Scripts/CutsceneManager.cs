using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject camera;
    public List<DialogueTrigger> dialogueTriggers;
    public List<PlayableDirector> timelines;
    public List<double> timelineEnds;
    public bool startWithDialogue;
    public bool startWithTimeline;
    public bool setCamFreeForTimeline;
    public float waitUntilDialogueStart;
    public bool loadNextSceneAfterDialogue;
    public bool loadNextSceneAfterTimeline;
    public List<int> playNextDialogueAfterTimelines;
    public List<int> playNextTimelineAfterDialogues;

    private int dialogueIndex = 0;
    private int timelineIndex = 0;
    private bool timelinePlaying = false;
    private SceneLoader sceneLoader;

    void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    void Start()
    {
        if (startWithDialogue)
        {
            StartCoroutine(WaitAndTriggerDialogue(0));
        }

        if (startWithTimeline)
        {
            StartCoroutine(WaitAndTriggerTimeline(0));
        }
    }

    void Update()
    {
        if (timelinePlaying)
        {
            if (timelines[timelineIndex].time >= timelineEnds[timelineIndex])
            {
                timelinePlaying = false;
                UpdateAfterTimeline();
            }
        }
    }

    public void UpdateAfterDialogue() // gets called from DialogueManager
    {
        dialogueIndex = dialogueManager.dialoguesFinished;

        if (loadNextSceneAfterDialogue)
        {
            if (dialogueIndex == dialogueTriggers.Count)
            {
                sceneLoader.LoadNextScene();
            }
        }

        for (int i = 0; i < playNextTimelineAfterDialogues.Count; i++)
        {
            if (dialogueIndex == playNextTimelineAfterDialogues[i])
            {
                if (setCamFreeForTimeline)
                {
                    SetCamFree();
                }
                timelines[timelineIndex].Play();
                timelinePlaying = true;
            }
        }
    }

    private void UpdateAfterTimeline()
    {
        timelineIndex++;
        if (loadNextSceneAfterTimeline)
        {
            if (timelineIndex == timelines.Count)
            {
                sceneLoader.LoadNextScene();
            }
        }

        for (int i = 0; i < playNextDialogueAfterTimelines.Count; i++)
        {
            if (timelineIndex == playNextDialogueAfterTimelines[i])
            {
                dialogueTriggers[dialogueIndex].TriggerDialogue();
                break;
            }
        }
    }

    private void SetCamFree()
    {
        camera.GetComponent<CameraFollow>().followObject = false;
    }

    private IEnumerator WaitAndTriggerDialogue(int index)
    {
        yield return new WaitForSeconds(waitUntilDialogueStart);
        dialogueTriggers[index].TriggerDialogue();
    }

    private IEnumerator WaitAndTriggerTimeline(int index)
    {
        yield return new WaitForSeconds(waitUntilDialogueStart);
        timelines[index].Play();
        timelinePlaying = true;
    }
}
