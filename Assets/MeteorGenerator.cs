using UnityEngine;

public class MeteorGenerator : MonoBehaviour
{
    public GameObject meteorTemplate;
    private int _count;

    void Update()
    {
        if (Random.value < .005f && _count < 100)
        {
            _count += 1;
            var meteor = SpawnMeteor();
            meteor.GetComponent<Meteor>().BeforeDestroy += () => _count -= 1;
        }
    }

    private GameObject SpawnMeteor()
    {
        var meteor = Instantiate(meteorTemplate);
        meteor.transform.position = (Random.insideUnitSphere).normalized * 2000f;

        return meteor;
    }
}
