using UnityEngine;

public class HealthBar : MonoBehaviour
{
    protected enum Twinkle { Stay, Increase, Decrease };
    protected Twinkle twinkle = Twinkle.Stay;

    [SerializeField]
    protected Color fullColor = Color.green,
        middColor = Color.yellow,
        zeroColor = Color.red;

    protected float offset = 4;
    protected float unitHealth = 1;
    protected float smoothSpeed = 3f;

    protected PlayerController player;

    public virtual void Initialize(PlayerController player)
    {
        this.player = player;
    }
}