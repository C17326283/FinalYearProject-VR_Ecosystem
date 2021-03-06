using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public Animator _handAnimator;

    public float grip;

    public float pinch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _handAnimator.SetFloat("Grip",grip);
        print(_handAnimator.GetFloat("Grip"));
        _handAnimator.SetFloat("Trigger",pinch);

    }
}
