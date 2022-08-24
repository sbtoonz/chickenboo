using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEggLayer : MonoBehaviour
{
    [SerializeField] internal Tameable _tameable;
    [SerializeField] internal MonsterAI _monsterAI;
    [SerializeField] internal Humanoid _humanoid;
    [SerializeField] internal GameObject EggObject;
    [SerializeField] internal HelmetMounter _helmetMounter;

    private ItemDrop? itemDrop;

    private void Awake()
    {
        _tameable = GetComponent<Tameable>();
        _monsterAI = GetComponent<MonsterAI>();
        _humanoid = GetComponent<Humanoid>();
    }

    private void Start()
    {
        Random.InitState(Time.renderedFrameCount);
    }

    private void OnEnable()
    {
        StartCoroutine(EggLayer());
    }
    
    
    private IEnumerator EggLayer()
    {
        if (!_tameable.m_character.m_tamed) yield break;
        if (!ShouldLayEgg()) yield break;
        yield return new WaitForSeconds(ChickenBoo.ChickenBoo.MinimumSpawnTimeForEgg.Value);
        var randValue = Random.value;
        switch (randValue)
        {
            // 45% of the time
            case < .45f:
            {
                // Do Egg Lay x 1
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                if (ChickenBoo.ChickenBoo.useRKEggs.Value)
                {
                    itemDrop = (ItemDrop)Instantiate(ChickenBoo.ChickenBoo.RK_Egg,
                        transform.position + transform.forward * 2f + Vector3.up + vector,
                        Quaternion.identity)?.GetComponent(typeof(ItemDrop))!; 
                }
                else
                {
                    itemDrop = (ItemDrop)Instantiate(EggObject,
                        transform.position + transform.forward * 2f + Vector3.up + vector,
                        Quaternion.identity)?.GetComponent(typeof(ItemDrop))!;
                }
                if (itemDrop == null || itemDrop.m_itemData == null) yield break;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol1.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();
                break;
            }
            // 45% of the time
            case < .9f:
            {
                // Do Egg lay x 6
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                if (ChickenBoo.ChickenBoo.useRKEggs.Value)
                {
                    itemDrop = (ItemDrop)Instantiate(ChickenBoo.ChickenBoo.RK_Egg,
                        transform.position + transform.forward * 2f + Vector3.up + vector,
                        Quaternion.identity)?.GetComponent(typeof(ItemDrop))!; 
                }
                else
                {
                    itemDrop = (ItemDrop)Instantiate(EggObject,
                        transform.position + transform.forward * 2f + Vector3.up + vector,
                        Quaternion.identity)?.GetComponent(typeof(ItemDrop))!;
                }
                if (itemDrop == null || itemDrop.m_itemData == null) yield break;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol2.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();
                ;
                break;
            }
            // 10% of the time
            default:
            {
                // Do Egg Lay x 12?
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                if (ChickenBoo.ChickenBoo.useRKEggs.Value)
                {
                    itemDrop = (ItemDrop)Instantiate(ChickenBoo.ChickenBoo.RK_Egg,
                        transform.position + transform.forward * 2f + Vector3.up + vector,
                        Quaternion.identity)?.GetComponent(typeof(ItemDrop))!; 
                }
                else
                {
                    itemDrop = (ItemDrop)Instantiate(EggObject,
                        transform.position + transform.forward * 2f + Vector3.up + vector,
                        Quaternion.identity)?.GetComponent(typeof(ItemDrop))!;
                }
                if (itemDrop == null || itemDrop.m_itemData == null) yield break;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol3.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();
                break;
            }
        }
    }
    
    
    private bool ShouldLayEgg()
    {
        var hitcolliders = Physics.OverlapSphere(transform.position, 15);

        return hitcolliders.All(collider => !collider.gameObject.name.StartsWith("raw_egg", StringComparison.Ordinal));
    }
    
}
