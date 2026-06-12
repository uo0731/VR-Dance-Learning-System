using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BoneUI
{
    public string boneName;               // ボーン名
    public TextMeshProUGUI labelText;     // 左側の "Head"
    public Slider scoreSlider;            // スコアスライダー
    public TextMeshProUGUI valueText;     // 右側の数値 "60.3"
    public Image fillImage;               // Slider → Fill の Image
}

public class BoneScoreUI : MonoBehaviour
{
    public BoneUI[] boneUIs; // 12ボーン分をInspectorにドラッグ＆ドロップ

    public void UpdateBoneScore(string boneName, float score)
    {
        bool found = false;

        foreach (var boneUI in boneUIs)
        {
            if (boneUI.boneName == boneName)
            {
                found = true;

                // 左のラベル（固定）
                boneUI.labelText.text = boneName;

                // 右の数値だけ更新
                boneUI.valueText.text = $"{score:F1}";

                boneUI.scoreSlider.value = score;

                // 色分け
                if (boneUI.fillImage != null)
                {
                    if (score >= 80f)
                        boneUI.fillImage.color = Color.green;
                    else if (score >= 50f)
                        boneUI.fillImage.color = Color.yellow;
                    else
                        boneUI.fillImage.color = Color.red;
                }
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"UpdateBoneScore: '{boneName}' に一致する BoneUI が見つかりません。");
        }
    }

    public Dictionary<string, float> GetAverages()
    {
        Dictionary<string, float> result = new Dictionary<string, float>();
        foreach (var boneUI in boneUIs)
        {
            result[boneUI.boneName] = boneUI.scoreSlider.value;
        }
        return result;
    }
}
