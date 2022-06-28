using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using System;

public class CharacterAudio : MonoBehaviour
{
    [SerializeField]
    private GameObject characterSignalsInterfaceTarget;
    private ICharacterSignals _characterSignals;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;
    [SerializeField] private AudioClip[] steps;

    private void Awake()
    {
        _characterSignals = characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _characterSignals.Jumped.Subscribe(_ => this.audioSource.PlayOneShot(jump)).AddTo(this);
        _characterSignals.Landed.Subscribe(_ => this.audioSource.PlayOneShot(land)).AddTo(this);

        // step event stream => random clip stream => audio player
        SelectItemObservables
            .SelectRandom<AudioClip>(_characterSignals.Stepped, steps)
            .Subscribe(clip => this.audioSource.PlayOneShot(clip))
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
