using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public AudioSource metronome;   // カウント音
    public Animator instructorAnimator;
    public float bpm = 120f;
    private bool hasStopped = false;

    void Start()
    {
        //Unity再生時には鳴らさない
        metronome.Stop();
    }

    void Update()
    {
        if (hasStopped) return;

        AnimatorStateInfo info = instructorAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 1f && !instructorAnimator.IsInTransition(0))
        {
            StopMetronome();
        }
    }

    //Startボタンから呼ぶ用
    public void StartMetronome()
    {
        hasStopped = false;
        metronome.Play();
        Debug.Log("Metronome started.");
    }

    void StopMetronome()
    {
        hasStopped = true;
        metronome.Stop();
        Debug.Log("Instructor animation finished → Metronome stopped.");
    }
}
