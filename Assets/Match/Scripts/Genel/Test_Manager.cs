using UnityEngine;

public class Test_Manager : MonoBehaviour
{
    private static Test_Manager instance;
    public static Test_Manager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}