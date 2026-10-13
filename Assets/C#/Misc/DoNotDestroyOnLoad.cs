using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    public static GameObject MyGameObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
