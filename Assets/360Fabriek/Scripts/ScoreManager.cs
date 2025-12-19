using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 100;
    [SerializeField] private TMP_Text m_Text;

    void Update()
    {
        m_Text.text = $"Score: {score}";
    }
}
