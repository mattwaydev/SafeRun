using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.1f;

    public Image heroImage;
    public Image villainImage;
    [Range(0f, 1f)] public float fadedAlpha = 0.4f;

    public string nextScene;
    public string nextSpawn;

    private int index;
    private bool ended;

    void Start()
    {
        textComponent.text = string.Empty;
        index = 0;
        ended = false;
        UpdateCharacterFocus();
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (ended) return;
        if (AdvancePressed())
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    bool AdvancePressed()
    {
        bool space = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool pad = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
        return space || pad;
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
            UpdateCharacterFocus();
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if (ended) return;
        ended = true;

        if (string.IsNullOrWhiteSpace(nextScene))
        {
            gameObject.SetActive(false);
            return;
        }

        var gestor = SafeRun.Core.GestorEscenas.Instancia;
        if (gestor != null)
        {
            if (!string.IsNullOrWhiteSpace(nextSpawn))
                gestor.DefinirSpawnDestino(nextSpawn);
            gestor.IrASala(nextScene);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    void UpdateCharacterFocus()
    {
        bool heroSpeaks = (index % 2) == 0;
        SetAlpha(heroImage, heroSpeaks ? 1f : fadedAlpha);
        SetAlpha(villainImage, heroSpeaks ? fadedAlpha : 1f);
    }

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}
