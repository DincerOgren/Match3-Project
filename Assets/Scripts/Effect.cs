using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] float killTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, killTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
