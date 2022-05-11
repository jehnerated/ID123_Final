using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireAnimation : MonoBehaviour
{
    public GameObject meshObject;
    public List<Texture> texture = new List<Texture>();

    Material material;

    int textureFrameRate = 12;
    int currentIndex = 0;

    Vector3 fireScale = new Vector3(0.0875f, 0.11f, 0.0875f);
    Vector3 rotateAmount = new Vector3(0, 90, 0);
    void Start()
    {
        material = meshObject.GetComponent<Renderer>().material;

        material.EnableKeyword("_MainTex");
        material.EnableKeyword("_EmissionMap");
        StartCoroutine(AnimateFire());
    }

    IEnumerator AnimateFire()
    {
        while (true)
        {
            currentIndex = (currentIndex + 1) % 40;

            fireScale.y = Random.Range(0.11f, 0.75f);
            fireScale.x = Random.Range(0.075f, 0.12f);
            fireScale.z = fireScale.x;

            material.SetTexture("_MainTex", texture[currentIndex]);
            material.SetTexture("_EmissionMap", texture[currentIndex]);
            yield return new WaitForSeconds(1 / textureFrameRate);
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, fireScale, 0.1f);
        transform.Rotate(rotateAmount * Random.Range(0.1f,1.25f));
    }
}
