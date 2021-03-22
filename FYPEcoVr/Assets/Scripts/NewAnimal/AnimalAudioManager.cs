
using UnityEngine;
using UnityEngine.Audio;

public class AnimalAudioManager : MonoBehaviour
{
    public AudioClip footstep;
    public AudioClip attack;
    public AudioClip ambient;
    public AudioSource footstepSource;
    public AudioSource attackSource;
    public AudioSource ambientSource;

    public float randRangeLow = 8;
    public float randRangeHigh = 20;
    
    public AudioMixerGroup audioMixerGroup;
    
    public void Initialize()
    {
        audioMixerGroup = Resources.Load<AudioMixer>("MainMixer").FindMatchingGroups("Master")[0];
            
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.outputAudioMixerGroup = audioMixerGroup;
        footstepSource.clip = footstep;
        setDefault(footstepSource,30,.7f);
        //sources.Add(footstepSource);
        attackSource = gameObject.AddComponent<AudioSource>();
        attackSource.outputAudioMixerGroup = audioMixerGroup;
        attackSource.clip = attack;
        setDefault(attackSource,30,.9f);
        //sources.Add(attackSource);
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.outputAudioMixerGroup = audioMixerGroup;
        ambientSource.clip = ambient;
        setDefault(ambientSource,30,.9f);
        //ambientSource.playOnAwake = true;
        //ambientSource.loop = true;
        //sources.Add(ambientSource);

        //StartCoroutine(RepeatSpawn());
        InvokeRepeating("playAmbient", Random.Range(0,randRangeLow), Random.Range(randRangeLow,randRangeHigh));

    }

    public void StartAudio()
    {
//        print("start audio");
        InvokeRepeating("playAmbient", Random.Range(0,10), Random.Range(5,25));
    }
    
    public void StopAudio()
    {
//        print("stop audio");
        CancelInvoke();
    }

    void setDefault(AudioSource source,float dist,float vol)
    {
        source.spatialBlend = 1f;//3d
        source.playOnAwake = false;
        source.maxDistance = dist;
        source.volume = vol;
    }

    public void playAttack()
    {
        if (!attackSource.isPlaying && gameObject.activeInHierarchy)
        {
            attackSource.pitch = Random.Range(0.6f, 1.2f);
            attackSource.PlayOneShot(attack);
        }
        
    }
    
    public void playFootStep()
    {
        if(footstepSource.isPlaying && gameObject.activeInHierarchy)
            footstepSource.Stop();
        footstepSource.pitch = Random.Range(0.7f, 1f);
        footstepSource.PlayOneShot(footstep);
    }

    public void playAmbient()
    {
//        print("playing ambient");
        if (!ambientSource.isPlaying && gameObject.activeInHierarchy)
        {
            ambientSource.pitch = Random.Range(0.5f, 1.1f);
            ambientSource.PlayOneShot(ambient);
        }
    }
    
    /*
     //Doesnt work for some reason stops after 1 second, work around with invoke repeating
    IEnumerator  RandAmbientAudio()
    {
        while (true)//Parent hasnt been destroyed
        {
            Debug.Log("play 1");
            yield return new WaitForSeconds(1);
            
            //print("play ambient");
            //playAmbient();
        }
    }
    */
}
