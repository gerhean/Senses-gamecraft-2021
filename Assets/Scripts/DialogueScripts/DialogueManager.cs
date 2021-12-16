using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    private Queue<string> sentences;
    private bool isSentenceTyping = false;
    private string lastSentence = "";
    private UnityEvent dialogueEndEvent;


    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            DisplayNextSentence();
        }
        else if (Input.GetKeyDown("x"))
        {
            EndDialogue();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);
        enabled = true;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        nameText.text = dialogue.name;
        dialogueEndEvent = dialogue.dialogueEndEvent;
        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (isSentenceTyping) {
            StopAllCoroutines();
            dialogueText.text = lastSentence;
            isSentenceTyping = false;
            return;
        }
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }
        lastSentence = sentences.Dequeue();
        if (lastSentence.Substring(0, 6) == "@name=") {
            nameText.text = lastSentence.Substring(6);
            return;
        }
        StartCoroutine(TypeSentence(lastSentence));
        isSentenceTyping = true;
    }

    IEnumerator TypeSentence (string sentence) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
        }
        isSentenceTyping = false;
    }

    public void EndDialogue() {
        StopAllCoroutines();
        animator.SetBool("IsOpen", false);
        isSentenceTyping = false;
        dialogueEndEvent?.Invoke();
        enabled = false;
    }
}
