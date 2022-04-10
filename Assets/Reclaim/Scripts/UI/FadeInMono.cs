using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInMono : MonoBehaviour
{
    public float FadeSpeed;
    Image m_Image;
    float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        t += FadeSpeed * Time.deltaTime;
        var alpha = Mathf.Lerp(0, 255, t);

        Color c = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, alpha);
        m_Image.color = c;
    }
}
