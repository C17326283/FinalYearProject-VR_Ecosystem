
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;

public class SettingsControl : MonoBehaviour
{
    public AudioSource musicSource;
    //public AudioListener audioListener;
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider globalSlider;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        UpdateQuality(1);

    }


    public void UpdateVolume()
    {
//        print("update slider "+musicSource.volume+":"+musicSlider.value);
        musicSource.volume = musicSlider.value;
        audioMixer.SetFloat("MasterVolume",globalSlider.value);

    }
    
    public void UpdateQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName,LoadSceneMode.Single);
    }

    public void RespawnPlayer()
    {
        if (GameObject.Find("Core") != null)
        {
            GetPointOnPlanet corePointFinder = GameObject.Find("Core").GetComponent<GetPointOnPlanet>();//this can happen before core is spawned so dont allow
            
            RaycastHit? hitPoint = corePointFinder.GetPoint("Ground", 100);
            if (hitPoint != null)
            {
                RaycastHit hit = hitPoint.Value;
                GameObject.FindWithTag("Player").transform.position = hit.point;
            }
        }
        
    }
}
