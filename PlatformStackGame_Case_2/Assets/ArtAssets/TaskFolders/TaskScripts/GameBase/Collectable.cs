using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectable : MonoBehaviour
{
    ParticleSystem collectableParticle;
    AudioSource myAudio;

    [SerializeField] private AudioClip coinSound;

    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
        collectableParticle = GetComponentInChildren<ParticleSystem>();
        transform.DOMoveY(transform.position.y + .5f, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        transform.DORotate(new Vector3(0, 90, 0), 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

    }
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.TryGetComponent(out PlayerManager player);
        if (!isPlayer) return;

        collectableParticle.Play();
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        myAudio.PlayOneShot(coinSound);

        DOTween.Kill(transform);
        Destroy(gameObject, 2);
    }

   
 
}
