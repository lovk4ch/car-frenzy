using System;
using UnityEngine;

public class SimsHealthBar : HealthBar
{
    private float alphaZero = 0, alphaCurrent = 0, alphaFull = 1, twinkleSpeed = 3;

    [SerializeField]
    private new Renderer renderer = null;
    [SerializeField]
    private ParticleSystem sparks = null;
    [SerializeField]
    private Color deadColor = Color.black;

    private ParticleSystem.MainModule mainModule;

    private void Awake()
    {
        mainModule = sparks.main;
    }

    private void Start()
    {
        SetColor(fullColor);
    }

    public override void Initialize(PlayerController player)
    {
        base.Initialize(player);
        transform.position = player.transform.position + Vector3.up * offset;
    }

    private void SetColor(Color color)
    {
        renderer.material.SetColor("_EmissionColor", color);
        mainModule.startColor = color;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        if (Math.Round(unitHealth - player.HealthPercentage, 2) != 0)
        {
            unitHealth = Mathf.Lerp(unitHealth, player.HealthPercentage,
                smoothSpeed * delta);

            if (twinkle != Twinkle.Increase) {
                twinkle = Twinkle.Increase;
            }
        }

        if (twinkle != Twinkle.Stay)
        {
            if (twinkle == Twinkle.Increase)
            {
                if (alphaCurrent < alphaFull) {
                    alphaCurrent += delta * twinkleSpeed;
                }
                else {
                    twinkle = Twinkle.Decrease;
                }
            }
            else
            {
                if (alphaCurrent > alphaZero) {
                    alphaCurrent -= delta * twinkleSpeed;
                }
                else {
                    twinkle = Twinkle.Stay;
                    alphaCurrent = alphaZero;
                }
            }
            if (unitHealth > 0.6f)
                SetColor(Color.Lerp(middColor, fullColor, (unitHealth - 0.6f) * 2.5f));
            else if (unitHealth > 0.2f)
                SetColor(Color.Lerp(zeroColor, middColor, (unitHealth - 0.2f) * 2.5f));
            else
                SetColor(Color.Lerp(deadColor, zeroColor, unitHealth * 5));
        }

        
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime * 30;

        transform.position = Vector3.Lerp(transform.position, player.transform.position + Vector3.up * offset, delta);
        transform.Rotate(Vector3.up * delta);
    }
}