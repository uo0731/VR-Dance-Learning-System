using UnityEngine;

public class RealTimeScoreManager : MonoBehaviour
{
    [System.Serializable]
    public class BonePair
    {
        public string boneName;
        public Transform instructorBone;
        public Transform playerBone;
    }

    public BonePair[] bonePairs;   // Inspectorで12ボーンを設定
    public BoneScoreUI boneScoreUI;
    public ScoreRecorder scoreRecorder; // ← Inspector で設定

    void Update()
    {
        foreach (var pair in bonePairs)
        {
            if (pair.instructorBone == null || pair.playerBone == null) continue;

            // 角度差を計算
            float angleDiff = Quaternion.Angle(pair.instructorBone.rotation, pair.playerBone.rotation);

            // スコア化 (0°=100点, 60°以上=0点)
            float score = Mathf.Clamp01(1f - (angleDiff / 60f)) * 100f;

            // UIに反映
            if (boneScoreUI != null)
            {
                boneScoreUI.UpdateBoneScore(pair.boneName, score);
            }

            //ここでスコアを記録
            if (scoreRecorder != null)
            {
                scoreRecorder.RecordScore(pair.boneName, score);
            }
        }
    }
}
