using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem _dustJump;
    [SerializeField] private ParticleSystem _dustRun;
    [SerializeField] private ParticleSystem _dustImpact;
    [SerializeField] private ParticleSystem _dashEffect;
    private ParticleSystem.EmissionModule dustRunEmission;
    private ParticleSystem.EmissionModule dustImpactEmission;
    private Animator _animator;

    public void Initialize()
    {
        _animator = gameObject.GetComponent<Animator>();

        dustRunEmission = _dustRun.emission;
        dustImpactEmission = _dustImpact.emission;
    }

    public void PlayAnimations(bool isGrounded, bool wasOnGround, bool isOnWall, bool isDead, float gravityScale, float horizontalInput)
    {
        PlayParticlesEffects(isGrounded, wasOnGround, gravityScale, horizontalInput);
        SetAnimatorValues(isGrounded, isOnWall, isDead, horizontalInput);
    }

    private void PlayParticlesEffects(bool isGrounded, bool wasOnGround, float gravityScale, float horizontalInput)
    {
        //PARTICLES PLAY
        if (!wasOnGround && isGrounded)
        {
            dustImpactEmission.rateOverTime = 100 * gravityScale;
            _dustImpact.Play();
        }

        if (horizontalInput != 0 && isGrounded)
        {
            dustRunEmission.rateOverTime = 30f;
        }
        else
        {
            dustRunEmission.rateOverTime = 0;
        }
    }

    private void SetAnimatorValues(bool isGrounded, bool isOnWall, bool isDead, float horizontalInput)
    {
        _animator.SetBool("run", horizontalInput != 0);
        _animator.SetBool("grounded", isGrounded);
        _animator.SetBool("onwall", isOnWall);
        _animator.SetBool("dead", isDead);
    }

    public void PlayDamageAnimation()
    {
        StartCoroutine(DamageAnimation());
    }
    IEnumerator DamageAnimation()
    {
        _animator.SetBool("damaged", true);
        yield return new WaitForSeconds(.5f);
        _animator.SetBool("damaged", false);
    }

    public void JumpDustEffect()
    {
        _dustJump.Play();
    }

    public void DashEffect()
    {
        _dashEffect.Play();
    }
}
