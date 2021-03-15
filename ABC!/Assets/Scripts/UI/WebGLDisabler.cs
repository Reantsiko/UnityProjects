using UnityEngine;

public class WebGLDisabler : MonoBehaviour
{
#if UNITY_WEBGL
    void Start()
    {
        gameObject.SetActive(false);
    }
#endif
}
