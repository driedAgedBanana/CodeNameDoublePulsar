using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JetPackEnergy : MonoBehaviour
{
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

        if (currentEnergy <= maxEnergy)
        {
            CheckIfGroundedForRecharge();
        }
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

    }

    private IEnumerator WaitBeforeRecharge()
    {
        isRecharging = true;
        isPlayerTired = true;
        yield return new WaitForSeconds(waitTime);

        while (currentEnergy < maxEnergy)
        {
            sweat.Stop();
            sweat.Clear();
            currentEnergy += regainEnergyRate * Time.deltaTime;
            yield return null;
        }

        isRecharging = false;
        isPlayerTired = false;
    }

    private void CheckIfGroundedForRecharge()
    {
        bool grounded = PlayerController.Instance.IsGrounded();

        if (grounded && !isRecharging && currentEnergy < maxEnergy)
        {
            rechargeCoroutine = StartCoroutine(WaitBeforeRecharge());
        }

        if (!grounded && isRecharging)
        {
            StopCoroutine(rechargeCoroutine);
            isRecharging = false;
        }
    }


    private void UpdateEnergyBar()
    {
        jetPackEnergyBar.value = currentEnergy / maxEnergy;
    }
}
