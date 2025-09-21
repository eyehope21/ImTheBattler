using UnityEngine;

public class ToastTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToastManager.Instance.ShowToast("Test Toast from keypress!");
        }
    }
}
