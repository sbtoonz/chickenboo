using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEggLayer : MonoBehaviour
{
    [SerializeField] internal Tameable _tameable;
    [SerializeField] internal MonsterAI _monsterAI;
    [SerializeField] internal Humanoid _humanoid;
    [SerializeField] internal GameObject EggObject;
    [SerializeField] internal HelmetMounter _helmetMounter;

    internal DropTable TestTable;
    internal DropTable.DropData testItem;
    private ItemDrop itemDrop;

    private void Awake()
    {
        _tameable = GetComponent<Tameable>();
        _monsterAI = GetComponent<MonsterAI>();
        _humanoid = GetComponent<Humanoid>();
        testItem = new DropTable.DropData
        {
            m_item = ChickenBoo.ChickenBoo.RawEgg,
            m_weight = 0.15f,
            m_stackMax = 12,
            m_stackMin = 1
        };
        TestTable = new DropTable
        {
            m_dropChance = 0.15f,
            m_dropMax = ChickenBoo.ChickenBoo.SpawnVol1.Value,
            m_dropMin = ChickenBoo.ChickenBoo.SpawnVol2.Value,
            m_drops = new List<DropTable.DropData>
            {
                testItem,
            }
        };
    }
    
    private void OnEnable()
    {
        InvokeRepeating(nameof(EggLayer), 0f, Random.Range(ChickenBoo.ChickenBoo.MinimumSpawnTimeForEgg.Value, ChickenBoo.ChickenBoo.MaximumSpawnTimeForEgg.Value));
    }
    
    
    private void EggLayer()
    {
        if (!_tameable.m_character.m_tamed) return;
        if (!ShouldLayEgg()) return;
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
                if (itemDrop == null || itemDrop.m_itemData == null) return;
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
                if (itemDrop == null || itemDrop.m_itemData == null) return;
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
                if (itemDrop == null || itemDrop.m_itemData == null) return;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol3.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();
                break;
            }
        }
    }
    
    
    private bool ShouldLayEgg()
    {
        var hitcolliders = Physics.OverlapSphere(transform.position, 15);
        
            foreach (var collider in hitcolliders)
            {
                if (collider.gameObject.name.StartsWith("raw_egg")) 
                return false;
            }
            
            return true;
    }
    
}
