using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class Resource
    {
        public string name;
        public int value;
        public int minValue;
        public int maxValue;
    }

    [SerializeField] private List<Resource> resources = new List<Resource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateResource(string resourceName, int amount)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        if (resource != null)
        {
            resource.value = Mathf.Clamp(resource.value + amount, resource.minValue, resource.maxValue);
            Debug.Log($"{resourceName} updated to {resource.value}");
        }
    }

    public int GetResourceValue(string resourceName)
    {
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource?.value ?? 0;
    }
} 