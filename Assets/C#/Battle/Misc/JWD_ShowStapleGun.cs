using UnityEngine;

public class JWD_ShowStapleGun : MonoBehaviour
{
    public GameObject StapleGun;

    private void Start()
    {
        if(StapleGun != null)
            StapleGun.SetActive(false);
        else
            Debug.LogWarning("JWD_ShowStapleGun on JWD Jimmy has no reference to his staple gun!");
    }

    // Used by JWD Jimmy's retry animation to make the staple gun appear mid jump.
    public void ShowStapleGun()
    {
        if (StapleGun != null)
            StapleGun.SetActive(true);
        else
            Debug.LogWarning("JWD_ShowStapleGun on JWD Jimmy has no reference to his staple gun!");
    }
}
