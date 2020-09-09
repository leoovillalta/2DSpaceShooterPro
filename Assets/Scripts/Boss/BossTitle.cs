using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTitle : MonoBehaviour
{
    public Image image;
    public Material material;
    public float Threshold;
    public float Noise;
    
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        material = image.material;        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("Noise", Noise);
        material.SetFloat("_ThresholdSliderVariable", Threshold);
    }
    
}
