using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public TMP_Text countdownText;      // カウントダウンを表示する Text
    public Animator instructorAnimator; // Instructor の Animator

    void Start()
    {
        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        // 3,2,1 を表示
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // GO! を表示
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        // カウントダウンを非表示
        countdownText.text = "";

        // Instructor のアニメーション再生
        instructorAnimator.SetTrigger("StartDance");
    }
}
