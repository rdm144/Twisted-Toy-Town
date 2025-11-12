using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelationEffect : MonoBehaviour
{
    RawImage pixelationViewport;
    public RenderTexture viewportTexture;
    
    [Range(0.01f, 1)]
    private float pixelationScale;
    const float SCALE_MIN_VALUE = 0.01f;
    private Vector2Int defaultViewportTextureResolution;

    public bool scaleTest;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        // Find the raw image object in our children and set its texture
        pixelationViewport = transform.Find("Canvas/RawImage").GetComponent<RawImage>();
        if (pixelationViewport != null && viewportTexture != null)
            pixelationViewport.texture = viewportTexture;

        // Store the default size of the render texture
        if (viewportTexture != null)
            defaultViewportTextureResolution = new Vector2Int(1920, 1080);

        pixelationScale = 1;
        scaleTest = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(scaleTest == true)
        {
            StartCoroutine("Unpixelate", duration);
            scaleTest = false;
        }
    }

    public IEnumerator Pixelate(float durationSeconds)
    {
        float timeElapsed = 0;
        if (durationSeconds <= 0)
            durationSeconds = SCALE_MIN_VALUE;
        SetPixelationScale(1);
        ScaleTexture();
        yield return new WaitForFixedUpdate();

        while (pixelationScale > SCALE_MIN_VALUE)
        {
            timeElapsed += Time.deltaTime;
            SetPixelationScale(1 * Mathf.Pow((1 - (0.088f/durationSeconds)), timeElapsed / Time.deltaTime));
            ScaleTexture();
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator Unpixelate(float durationSeconds)
    {
        float timeElapsed = 0;
        if (durationSeconds <= 0)
            durationSeconds = SCALE_MIN_VALUE;
        SetPixelationScale(SCALE_MIN_VALUE);
        ScaleTexture();
        yield return new WaitForFixedUpdate();

        while (pixelationScale < 1)
        {
            timeElapsed += Time.deltaTime;
            SetPixelationScale(SCALE_MIN_VALUE * Mathf.Pow((1 + (0.088f / durationSeconds)), timeElapsed / Time.deltaTime));
            ScaleTexture();
            yield return new WaitForFixedUpdate();
        }
    }

    public void ScaleTexture()
    {
        viewportTexture.Release();
        viewportTexture.width = (int)(defaultViewportTextureResolution.x * pixelationScale);
        viewportTexture.height = (int)(defaultViewportTextureResolution.y * pixelationScale);
        viewportTexture.Create();
    }

    public void ScaleTexture(float scale)
    {
        SetPixelationScale(scale);
        viewportTexture.Release();
        viewportTexture.width = (int)(defaultViewportTextureResolution.x * pixelationScale);
        viewportTexture.height = (int)(defaultViewportTextureResolution.y * pixelationScale);
        viewportTexture.Create();
    }

    public void SetPixelationScale(float scale)
    {
        if (scale >= SCALE_MIN_VALUE && scale <= 1)
            pixelationScale = scale;
        else if(scale <= SCALE_MIN_VALUE)
            pixelationScale = SCALE_MIN_VALUE;
        else
            pixelationScale = 1;
    }

    // Create() is unsafe memory-wise, so the texture must be released before closing
    private void OnDestroy()
    {
        if(viewportTexture != null)
        {
            viewportTexture.Release();
            viewportTexture = null;
        }
    }

    // The texture's size must be reset before closing
    private void OnApplicationQuit()
    {
        if(viewportTexture != null)
        {
            viewportTexture.Release();
            viewportTexture.width = defaultViewportTextureResolution.x;
            viewportTexture.height = defaultViewportTextureResolution.y;
            viewportTexture.Create();
        }
    }
}
