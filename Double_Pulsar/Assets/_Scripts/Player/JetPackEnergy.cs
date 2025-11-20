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

    [HideInInspector] public bool isRecharging = false;
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
        if (sweat == null)
        {
            sweat = GetComponentInChildren<ParticleSystem>();
            sweat.Stop();
        }
        currentEnergy = maxEnergy;
        UpdateEnergyBar();
    }

    private void Update()
    {
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        isEnergyEmpty = currentEnergy <= 0;
        UpdateEnergyBar();
    }

    public void DrainEnergy(float energyDrainRate)
    {
        currentEnergy -= energyDrainRate;

        // Stop recharging if player is draining
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
            isRecharging = false;
        }

        if (currentEnergy <= 0)
        {
            sweat.Play();
        }

        rechargeCoroutine = StartCoroutine(WaitBeforeRecharge());

    }

    private IEnumerator WaitBeforeRecharge()
    {
        isRecharging = true;
        isPlayerTired = true;
        yield return new WaitForSeconds(waitTime);

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += regainEnergyRate * Time.deltaTime;
            yield return null;
        }

        isRecharging = false;
        isPlayerTired = false;

        sweat.Stop();
        sweat.Clear();
    }

    private void UpdateEnergyBar()
    {
        jetPackEnergyBar.value = currentEnergy / maxEnergy;
    }
}
