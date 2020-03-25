using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    private const string infoString = "Lifetime: {0:F0}\nCars: {1}\nScore: {2}\nHealth: {3}%";

    [SerializeField]
    private Text info = null;
    private void Update()
    {
        info.text = string.Format(infoString, Time.time, LevelManager.Instance.Cars, LevelManager.Instance.Score, LevelManager.Instance.Player.HealthPercentage * 100);
    }
}