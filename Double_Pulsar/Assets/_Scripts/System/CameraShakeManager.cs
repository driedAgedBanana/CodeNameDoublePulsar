using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public float globalShakeForce = 1;

    private CinemachineImpulseDefinition _impulseDefinition;
    [SerializeField] private CinemachineImpulseListener _impulseListener;

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        // Apply settings
        SetUpScreenShakeSettings(profile, impulseSource);

        // Screen shake
        impulseSource.GenerateImpulseWithForce(profile.impulseForce);
    }

    private void SetUpScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        _impulseDefinition = impulseSource.m_ImpulseDefinition;

        // Change the impulse source settings
        _impulseDefinition.m_ImpulseDuration = profile.impulseTime;
        impulseSource.m_DefaultVelocity = profile.defaultVelocity;
        _impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        // Change the impulse listener settings
        _impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        _impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        _impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }
}
