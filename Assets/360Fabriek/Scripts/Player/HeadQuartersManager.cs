using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HeadQuartersManager : MonoBehaviour
{
    public int Health;
    public static HeadQuartersManager Instance;

    public TMP_Text text;

    private void Start()
    {
        Instance = this;
    }

    public void TakeDamage(int health)
    {
        Health -= health;

        if (Health <= 0)
        {
            Time.timeScale = 0;
            StartCoroutine(YouLose());
        }
    }

    private IEnumerator YouLose()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        text.text = $"HP: {Health}";
    }
}
