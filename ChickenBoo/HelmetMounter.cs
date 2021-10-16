using System;
using UnityEngine;

public class HelmetMounter : MonoBehaviour
{
    private string m_hoverText = "Equip Helmet?";
    [SerializeField] internal Transform HelmetMountPoint;
    [SerializeField] internal GameObject HelmetObject;
    [SerializeField] internal ZNetView _zNetView;

    internal bool HelmetMounted;

    private void Awake()
    {
        try
        {
            var test = _zNetView.m_zdo.GetBool("$chicken_hat");
            if (test)
            {
                HelmetObject.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void OnEnable()
    {
        switch (HelmetObject.activeInHierarchy)
        {
            case false:
                HelmetMounted = false;
                break;
            case true:
                HelmetMounted = true;
                break;
        }
    }
}
