using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Dialog[] dialog;
    public Text title;
    public Text text;
    private int nextIndex;
    private Queue<string> lineQueue = new Queue<string>(0);

    public void StartDialog(int index)
    {
        title.text = dialog[index].name;
        nextIndex = dialog[index].nextDialogIndex;
        foreach (string line in dialog[index].lines)
        {
            lineQueue.Enqueue(line);
        }
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (lineQueue.Count > 0)
        {
            text.text = lineQueue.Dequeue();
        }
        else if(nextIndex != 0)
        {
            StartDialog(nextIndex);
        }
    }
}