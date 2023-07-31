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
    //DOTween.To(value => { }, startValue: 0, endValue: 1, duration: 0.05f)
    //		.OnComplete(() =>
    //		{
    //			gridLayoutGroup.enabled = false;
    //			contentSizeFitter.enabled = false;
    //		});
    // Test_Manager.Instance.Debugger();
    //public void Debugger(string debugMessage)
    //{
    //    Debug.Log(debugMessage);
    //}
}