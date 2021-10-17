using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEggLayer : MonoBehaviour
{
    [SerializeField] internal Tameable _tameable;
    [SerializeField] internal MonsterAI _monsterAI;
    [SerializeField] internal Humanoid _humanoid;
    [SerializeField] internal GameObject EggObject;
    [SerializeField] internal HelmetMounter _helmetMounter;
    
    private void Awake()
    {
        _tameable = GetComponent<Tameable>();
        _monsterAI = GetComponent<MonsterAI>();
        _humanoid = GetComponent<Humanoid>();
    }
    
    private void OnEnable()
    {
        InvokeRepeating(nameof(EggLayer), 0f, Random.Range(ChickenBoo.ChickenBoo.MinimumSpawnTimeForEgg.Value, ChickenBoo.ChickenBoo.MaximumSpawnTimeForEgg.Value));
    }
    
    
    private void EggLayer()
    {
        if (_tameable.m_character.m_tamed)
        {
            float randValue = Random.value;
            if (randValue < .45f) // 45% of the time
            {
                // Do Egg Lay x 1
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                var itemDrop = (ItemDrop)Instantiate(EggObject, transform.position + transform.forward * 2f + Vector3.up + vector,
                    Quaternion.identity)?.GetComponent(typeof(ItemDrop));
                if (itemDrop == null || itemDrop.m_itemData == null) return;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol1.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();
            }
            else if (randValue < .9f) // 45% of the time
            {
                // Do Egg lay x 6
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                var itemDrop = (ItemDrop)Instantiate(EggObject, transform.position + transform.forward * 2f + Vector3.up + vector,
                    Quaternion.identity)?.GetComponent(typeof(ItemDrop));
                if (itemDrop == null || itemDrop.m_itemData == null) return;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol2.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();;
            }
            else // 10% of the time
            {
                // Do Egg Lay x 12?
                Vector3 vector = Random.insideUnitSphere * 0.5f;
                var itemDrop = (ItemDrop)Instantiate(EggObject, transform.position + transform.forward * 2f + Vector3.up + vector,
                    Quaternion.identity)?.GetComponent(typeof(ItemDrop));
                if (itemDrop == null || itemDrop.m_itemData == null) return;
                itemDrop.m_itemData.m_stack = ChickenBoo.ChickenBoo.SpawnVol3.Value;
                itemDrop.m_itemData.m_durability = itemDrop.m_itemData.GetMaxDurability();
            }
            
        }
    }
}
