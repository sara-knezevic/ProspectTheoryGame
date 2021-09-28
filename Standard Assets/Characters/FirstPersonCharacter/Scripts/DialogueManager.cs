using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    public GameObject dialoguePanel;

    public Text dialogueText;
    private GameObject buttonOne;
    private GameObject buttonTwo;
    private GameObject continueButton;

    void Start()
    {
        sentences = new Queue<string>();

        dialogueText = dialoguePanel.transform.GetChild(0).gameObject.GetComponent<Text>();
        buttonOne = dialoguePanel.transform.GetChild(1).gameObject;
        buttonTwo = dialoguePanel.transform.GetChild(2).gameObject;
        continueButton = dialoguePanel.transform.GetChild(3).gameObject;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.name);
        dialoguePanel.SetActive(true);

        continueButton.SetActive(false);
        buttonOne.SetActive(true);
        buttonTwo.SetActive(true);

        sentences.Clear();

        dialogueText.text = dialogue.sentences[0];

        buttonOne.transform.GetChild(0).gameObject.GetComponent<Text>().text = dialogue.sentences[1];

        buttonTwo.transform.GetChild(0).gameObject.GetComponent<Text>().text = dialogue.sentences[2];
    }

    public void Continue(Dialogue dialogue)
    {
        continueButton.SetActive(true);
        buttonOne.SetActive(false);
        buttonTwo.SetActive(false);

        dialogueText.text = dialogue.sentences[3];

        // dialogueText.text = dialogue.sentences[3];
    }
}
