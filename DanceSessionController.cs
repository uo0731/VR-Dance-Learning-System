using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DanceSessionController : MonoBehaviour
{
    public Animator instructorAnimator;
    public ScoreRecorder scoreRecorder;
    public GameObject feedbackPanel;
    public TMP_Text titleText;
    public TMP_Text totalScoreText;
    public TMP_Text boneScoreText;   
    public TMP_Text commentText;
    public Button startButton;
    public GameObject boneScorePanel;
    public RadarChartRenderer radarChart;
    public RawImage radarChartImage;
    public float bpm = 120f;
    public BeatManager beatManager;

    private bool isPlaying = false;

    private void Start()
    {
        feedbackPanel.SetActive(false);

        if (instructorAnimator != null)
            instructorAnimator.speed = 0f;   // ←自動で動かないようにする
    }

    //StartButton から呼ばれる
    public void StartDanceSession()
    {
        Debug.Log("=== Start Button Pressed ===");

        feedbackPanel.SetActive(false);

        // Instructor アニメーション再生リセット・開始
        if (instructorAnimator != null)
        {
            instructorAnimator.Play("4yaka", 0, 0f);
            instructorAnimator.speed = 1f;
        }

        scoreRecorder.StartRecording();
        isPlaying = true;

        //ここでカウント音スタート！
        if (beatManager != null)
            beatManager.StartMetronome();

        if (startButton != null)
            startButton.gameObject.SetActive(false);

        if (boneScorePanel != null)
            boneScorePanel.SetActive(true);

        //アニメーションはまだ動かさない
        if (instructorAnimator != null)
            instructorAnimator.speed = 0f;

        // カウント開始
        StartCoroutine(CountAndStartDance());
    }

    private IEnumerator CountAndStartDance()
    {
        float oneCountTime = 60f / bpm; // BPM120 → 0.5秒

        // 4カウント待つ
        for (int i = 1; i <= 7; i++)
        {
            Debug.Log($"Count {i}");
            yield return new WaitForSeconds(oneCountTime);
        }

        // 5カウント目：ダンス開始
        Debug.Log("Dance Start!");

        if (instructorAnimator != null)
            instructorAnimator.speed = 1f;

        scoreRecorder.StartRecording();
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying) return;

        AnimatorStateInfo state = instructorAnimator.GetCurrentAnimatorStateInfo(0);

        if (state.normalizedTime >= 1f)
        {
            Debug.Log("アニメーション終了を検出");
            EndDanceEvaluation();
            isPlaying = false;
        }
    }

    public void EndDanceEvaluation()
    {
        Debug.Log("EndDanceEvaluation() 呼び出し");

        scoreRecorder.StopRecording();

        if (boneScorePanel != null)
            boneScorePanel.SetActive(false);

        Dictionary<string, float> boneScores = scoreRecorder.GetBoneScores();
        float totalScore = scoreRecorder.GetTotalScore();

        if (titleText != null)
            titleText.text = "Dance Evaluation Results";

        if (totalScoreText != null)
            totalScoreText.text = $"Total Score: {totalScore:F1}";

        string boneScoreBody = "  <b>[Per-bone Average Score]</b>\n";

        int maxKeyLength = 0;
        foreach (var bone in boneScores)
            if (bone.Key.Length > maxKeyLength)
                maxKeyLength = bone.Key.Length;

        foreach (var bone in boneScores)
            boneScoreBody += $"         {bone.Key,-15}: {bone.Value,5:F1}\n";

        if (boneScoreText != null)
            boneScoreText.text = boneScoreBody;

        // ★ GenerateFeedback を呼び出す（ここ重要）
        commentText.text = "[Comment]\n" + GenerateFeedback(totalScore, boneScores);

        // --- boneScores を配列にしてレーダーチャートへ ---
        float[] scoreArray = boneScores.Values.ToArray();
        string[] boneLabels = boneScores.Keys.ToArray();

        radarChart.DrawChart(scoreArray, boneLabels);

        if (feedbackPanel != null)
            feedbackPanel.SetActive(true);

        Debug.Log("Feedback panel activated!");
    }

    private string GenerateFeedback(float totalScore, Dictionary<string, float> boneScores)
    {
        string feedback = "";

        // --- Total Score コメント ---
        if (totalScore >= 80f)
        {
            string[] list = {
            "Excellent! Your movement quality is very high!",
            "Great job! Your posture and rhythm were very accurate!",
            "Amazing control! Keep dancing at this level!"
        };
            feedback = list[UnityEngine.Random.Range(0, list.Length)];
        }
        else if (totalScore >= 60f)
        {
            string[] list = {
            "Good work! You're dancing with solid form!",
            "Nice! Your movement accuracy is improving!",
            "Well done! Stay focused and keep refining your style."
        };
            feedback = list[UnityEngine.Random.Range(0, list.Length)];
        }
        else if (totalScore >= 40f)
        {
            string[] list = {
            "Keep it up! Try to move with a bit more confidence.",
            "You're on the right track! Focus on keeping your rhythm steady.",
            "Nice try! Try making your movements a little bigger."
        };
            feedback = list[UnityEngine.Random.Range(0, list.Length)];
        }
        else
        {
            string[] list = {
            "Try to keep a steady rhythm throughout your movements.",
            "Work on making your steps clearer and more defined.",
            "Let’s improve step by step—keep practicing!"
        };
            feedback = list[UnityEngine.Random.Range(0, list.Length)];
        }

        // --- 最もスコアが低い部位 ---
        string lowestBone = "";
        float lowestValue = float.MaxValue;

        foreach (var kvp in boneScores)
        {
            if (kvp.Value < lowestValue)
            {
                lowestBone = kvp.Key;
                lowestValue = kvp.Value;
            }
        }

        // --- 部位ごとのアドバイス追加 ---
        if (!string.IsNullOrEmpty(lowestBone))
        {
            feedback += "\n";  

            switch (lowestBone)
            {
                case "Head":
                case "Hip":
                    feedback += "Try to keep your upper body more stable.";
                    break;

                case "LeftUpperLeg":
                case "RightUpperLeg":
                case "LeftFoot":
                case "RightFoot":
                case "LeftToes":
                case "RightToes":
                    feedback += "Try to add more strength and clarity to your steps.";
                    break;

                case "LeftUpperArm":
                case "RightUpperArm":
                case "LeftHand":
                case "RightHand":
                    feedback += "Make your arm movements bigger and more expressive.";
                    break;

                default:
                    feedback += "Keep practicing each body part to improve overall form.";
                    break;
            }
        }

        return feedback;
    }

}
