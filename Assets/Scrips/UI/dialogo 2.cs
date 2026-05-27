using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class dialogo2 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.05f;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        // NUEVO INPUT SYSTEM
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Si ya terminó de escribir
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                // Completa el texto instantáneamente
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}