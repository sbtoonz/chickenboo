using System;
using UnityEngine;

public class HelmetMounter : MonoBehaviour
{
#pragma warning disable CS0414
    private string m_hoverText = "Equip Helmet?";
#pragma warning restore CS0414
    [SerializeField] internal Transform HelmetMountPoint;
    [SerializeField] internal GameObject HelmetObject;
    [SerializeField] internal ZNetView _zNetView;
    [SerializeField] internal GameObject GravesSombrero;
    
    internal bool HelmetMounted;

    private void Awake()
    {

        var test = _zNetView.m_zdo.GetBool("$chicken_hat");
        if (test)
        {
            HelmetObject.SetActive(true);
            HelmetMounted = true;
        }

        var tmp = _zNetView.m_zdo.GetBool("$chicken_sombrero");
        if (tmp)
        {
            GravesSombrero.SetActive(true);
            HelmetMounted = true;
        }

        if (!tmp || !test)
        {
            HelmetMounted = false;
        }
            
    }

    private void OnEnable()
    {
        switch (HelmetObject.activeInHierarchy || GravesSombrero.activeInHierarchy)
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