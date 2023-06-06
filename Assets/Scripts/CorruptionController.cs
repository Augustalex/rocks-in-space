using Game;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{
    private float _cameAlive;
    private bool _hasSpread;

    void Start()
    {
        var block = GetComponent<Block>();
        
        var blockTransform = block.transform;
        var blockPosition = blockTransform.position;
        var blockRotation = blockTransform.rotation;
        Instantiate(
            PrefabTemplateLibrary.Get().corruptionParticlesTemplate,
            blockPosition,
            blockRotation,
            transform
        );

        _cameAlive = GameTime.Time();
    }

    void Update()
    {
        if(_hasSpread) return;
        
        var timeAlive = GameTime.Time() - _cameAlive;
        if (timeAlive > 60)
        {
            _hasSpread = true;
            SpreadToNearbyBlocks();
        }
    }

    // Use Physics overlap sphere to detect nearby blocks within a 1 block radius
    private void SpreadToNearbyBlocks()
    {
        var block = GetComponent<Block>();
        var blockPosition = block.GetPosition();
        var nearbyBlocks = Physics.OverlapSphere(blockPosition, 1f);
        foreach (var nearbyBlock in nearbyBlocks)
        {
            var nearbyBlockComponent = nearbyBlock.GetComponent<Block>();
            if (nearbyBlockComponent)
            {
                if(!nearbyBlockComponent.IsCorrupted()) nearbyBlockComponent.Corrupt();
            }
        }
    }
}