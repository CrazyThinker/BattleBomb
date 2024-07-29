using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Text textUp1, textDown1, textLeft1, textRight1, textBomb1, textUp2, textDown2, textLeft2, textRight2, textBomb2;

    void Start()
    {
        if (textUp1 != null) textUp1.text = PlayerPrefs.GetString("Up1", KeyCode.W.ToString());
        if (textDown1 != null) textDown1.text = PlayerPrefs.GetString("Down1", KeyCode.S.ToString());
        if (textLeft1 != null) textLeft1.text = PlayerPrefs.GetString("Left1", KeyCode.A.ToString());
        if (textRight1 != null) textRight1.text = PlayerPrefs.GetString("Right1", KeyCode.D.ToString());
        if (textBomb1 != null) textBomb1.text = PlayerPrefs.GetString("Bomb1", KeyCode.LeftControl.ToString());

        if (textUp2 != null) textUp2.text = PlayerPrefs.GetString("Up2", KeyCode.UpArrow.ToString());
        if (textDown2 != null) textDown2.text = PlayerPrefs.GetString("Down2", KeyCode.DownArrow.ToString());
        if (textLeft2 != null) textLeft2.text = PlayerPrefs.GetString("Left2", KeyCode.LeftArrow.ToString());
        if (textRight2 != null) textRight2.text = PlayerPrefs.GetString("Right2", KeyCode.RightArrow.ToString());
        if (textBomb2 != null) textBomb2.text = PlayerPrefs.GetString("Bomb2", KeyCode.Period.ToString());
    }

    public void buttonUp1()
    {
        StartCoroutine(WaitForKeyPress((key) => { textUp1.text = key.ToString(); }));
    }

    public void buttonDown1()
    {
        StartCoroutine(WaitForKeyPress((key) => { textDown1.text = key.ToString(); }));
    }

    public void buttonLeft1()
    {
        StartCoroutine(WaitForKeyPress((key) => { textLeft1.text = key.ToString(); }));
    }

    public void buttonRight1()
    {
        StartCoroutine(WaitForKeyPress((key) => { textRight1.text = key.ToString(); }));
    }

    public void buttonBomb1()
    {
        StartCoroutine(WaitForKeyPress((key) => { textBomb1.text = key.ToString(); }));
    }

    public void buttonUp2()
    {
        StartCoroutine(WaitForKeyPress((key) => { textUp2.text = key.ToString(); }));
    }

    public void buttonDown2()
    {
        StartCoroutine(WaitForKeyPress((key) => { textDown2.text = key.ToString(); }));
    }

    public void buttonLeft2()
    {
        StartCoroutine(WaitForKeyPress((key) => { textLeft2.text = key.ToString(); }));
    }

    public void buttonRight2()
    {
        StartCoroutine(WaitForKeyPress((key) => { textRight2.text = key.ToString(); }));
    }

    public void buttonBomb2()
    {
        StartCoroutine(WaitForKeyPress((key) => { textBomb2.text = key.ToString(); }));
    }

    private IEnumerator WaitForKeyPress(System.Action<KeyCode> onKeyPress)
    {
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                onKeyPress(keyCode);
                break;
            }
        }
    }

    public void settingOK()
    {
        PlayerPrefs.SetString("Up1", textUp1.text);
        PlayerPrefs.SetString("Down1", textDown1.text);
        PlayerPrefs.SetString("Left1", textLeft1.text);
        PlayerPrefs.SetString("Right1", textRight1.text);
        PlayerPrefs.SetString("Bomb1", textBomb1.text);
        PlayerPrefs.SetString("Up2", textUp2.text);
        PlayerPrefs.SetString("Down2", textDown2.text);
        PlayerPrefs.SetString("Left2", textLeft2.text);
        PlayerPrefs.SetString("Right2", textRight2.text);
        PlayerPrefs.SetString("Bomb2", textBomb2.text);

        SceneManager.LoadScene("OpeningScene");
    }

    public void settingCancel()
    {
        SceneManager.LoadScene("OpeningScene");
    }
}
