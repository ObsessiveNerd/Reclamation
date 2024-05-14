using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    Vector2 m_EndPosition;
    [HideInInspector]
    public TextMeshProUGUI Text;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        m_EndPosition = transform.position;
        m_EndPosition.y += 200;

        StartCoroutine(DestroyAfter());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, m_EndPosition, Time.deltaTime * 5);
        var startColor = Text.color;
        startColor.a -= Time.deltaTime;
        Text.color = startColor;
    }
    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
