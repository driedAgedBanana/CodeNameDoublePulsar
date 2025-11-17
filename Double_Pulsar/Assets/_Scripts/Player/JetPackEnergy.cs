using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JetPackEnergy : MonoBehaviour
{
    public static JetPackEnergy Instance;

    [Header("UI")]
    public Slider jetPackEnergyBar;
    public ParticleSystem sweat;
    public float maxEnergy = 100f;
    public float regainEnergyRate = 10f;
    public float waitTime = 1;

    [HideInInspector] public float currentEnergy;
    [HideInInspector] public bool isEnergyEmpty;

    private bool _isRecharging = false;
    [HideInInspector] public bool isPlayerTired = false;
    private Coroutine rechargeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if(sweat == null)
        {
            sweat = GetComponentInChildren<ParticleSystem>();
            sweat.Stop();
        }

        jetPackEnergyBar.gameObject.SetActive(false);
        currentEnergy = maxEnergy;
        UpdateEnergyBar();
    }

    private void Update()
    {
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        isEnergyEmpty = currentEnergy <= 0;
        UpdateEnergyBar();

        Debug.Log(currentEnergy);

        if (currentEnergy <= 0)
        {
            sweat.Play();
        }
        else
        {
            sweat.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void DrainEnergy(float energyDrainRate)
    {
        currentEnergy -= energyDrainRate;

        // Stop recharging if player is draining
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
            _isRecharging = false;
        }

        // Restart recharge delay
        rechargeCoroutine = StartCoroutine(WaitBeforeRecharge());
    }

    private IEnumerator WaitBeforeRecharge()
    {
        _isRecharging = true;
        yield return new WaitForSeconds(waitTime);

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += regainEnergyRate * Time.deltaTime;
            yield return null;
        }

        _isRecharging = false;
    }

    private void UpdateEnergyBar()
    {
        jetPackEnergyBar.value = currentEnergy / maxEnergy;
    }
}
