using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadarChartRenderer : MonoBehaviour
{
    public RawImage chartImage;
    public int chartSize = 300;
    public RectTransform labelParent;
    public GameObject labelPrefab;

    private List<GameObject> labelObjects = new List<GameObject>();
    private Texture2D texture;

    private void Awake()
    {
        texture = new Texture2D(chartSize, chartSize);
        chartImage.texture = texture;
    }

    public void DrawChart(float[] values, string[] labels)
    {
        texture.Clear(Color.clear);
        ClearLabels();

        int n = values.Length;
        Vector2 center = new Vector2(chartSize / 2f, chartSize / 2f);
        float radius = chartSize * 0.45f;

        Vector2[] points = new Vector2[n];

        // -----------------------
        // 頂点計算
        // -----------------------
        for (int i = 0; i < n; i++)
        {
            float val = Mathf.Clamp(values[i] / 100f, 0f, 1f);
            float angle = (Mathf.PI * 2 / n) * i - Mathf.PI / 2;

            float x = center.x + Mathf.Cos(angle) * radius * val;
            float y = center.y + Mathf.Sin(angle) * radius * val;

            points[i] = new Vector2(x, y);
        }

        // -----------------------
        // 軸（スパイン）
        // -----------------------
        for (int i = 0; i < n; i++)
        {
            float angle = (Mathf.PI * 2 / n) * i - Mathf.PI / 2;
            Vector2 end = new Vector2(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius
            );

            texture.DrawLine(center, end, new Color(1, 1, 1, 0.4f));
        }

        // -----------------------
        // 多角形の同心円
        // -----------------------
        int steps = 4;
        for (int s = 1; s <= steps; s++)
        {
            float t = (float)s / steps;
            Vector2[] stepPoints = new Vector2[n];

            for (int i = 0; i < n; i++)
            {
                float angle = (Mathf.PI * 2 / n) * i - Mathf.PI / 2;
                float x = center.x + Mathf.Cos(angle) * radius * t;
                float y = center.y + Mathf.Sin(angle) * radius * t;
                stepPoints[i] = new Vector2(x, y);
            }

            for (int i = 0; i < n; i++)
            {
                texture.DrawLine(stepPoints[i], stepPoints[(i + 1) % n], new Color(1, 1, 1, 0.15f));
            }
        }

        // -----------------------
        // 塗りつぶし
        // -----------------------
        for (int i = 0; i < n; i++)
        {
            Vector2 a = points[i];
            Vector2 b = points[(i + 1) % n];
            texture.DrawTriangle(center, a, b, new Color(0.3f, 0.7f, 1f, 0.4f));
        }

        // -----------------------
        // 外枠
        // -----------------------
        for (int i = 0; i < n; i++)
        {
            texture.DrawLine(points[i], points[(i + 1) % n], Color.white);
        }

        texture.Apply();

        // ★★★ ラベル生成 ★★★
        GenerateLabels(labels, center, radius);
    }

    // -----------------------
    // ラベル生成
    // -----------------------
    private void GenerateLabels(string[] labels, Vector2 center, float radius)
    {
        int n = labels.Length;

        for (int i = 0; i < n; i++)
        {
            float angle = (Mathf.PI * 2 / n) * i - Mathf.PI / 2;

            Vector2 pos = new Vector2(
                center.x + Mathf.Cos(angle) * (radius + 20),
                center.y + Mathf.Sin(angle) * (radius + 20)
            );

            GameObject label = Instantiate(labelPrefab, labelParent);
            label.GetComponent<RectTransform>().anchoredPosition = pos;

            TMP_Text text = label.GetComponent<TMP_Text>();
            text.text = labels[i];

            labelObjects.Add(label);
        }
    }

    private void ClearLabels()
    {
        foreach (var obj in labelObjects)
            Destroy(obj);

        labelObjects.Clear();
    }
}
