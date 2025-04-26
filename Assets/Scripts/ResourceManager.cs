using UnityEngine;
using UnityEngine.Events;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [System.Serializable]
    public class Resource
    {
        public string name;
        public int value;
        public int minValue;
        public int maxValue;
        public UnityEvent<int> OnValueChanged;
    }

    [Header("Resources")]
    [SerializeField] private Resource economy = new Resource { name = "Economy", value = 50, minValue = 0, maxValue = 100 };
    [SerializeField] private Resource technology = new Resource { name = "Technology", value = 50, minValue = 0, maxValue = 100 };
    [SerializeField] private Resource environment = new Resource { name = "Environment", value = 50, minValue = 0, maxValue = 100 };
    [SerializeField] private Resource happiness = new Resource { name = "Happiness", value = 50, minValue = 0, maxValue = 100 };

    [Header("Resource Dependencies")]
    [SerializeField] private float economyHappinessFactor = 0.3f;
    [SerializeField] private float technologyEconomyFactor = 0.2f;
    [SerializeField] private float environmentHappinessFactor = 0.4f;

    public UnityEvent<Resource[]> OnResourcesUpdated;

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
        Resource resource = GetResourceByName(resourceName);
        if (resource != null)
        {
            int oldValue = resource.value;
            resource.value = Mathf.Clamp(resource.value + amount, resource.minValue, resource.maxValue);
            
            if (oldValue != resource.value)
            {
                resource.OnValueChanged?.Invoke(resource.value);
                UpdateDependentResources(resourceName, amount);
                OnResourcesUpdated?.Invoke(GetAllResources());
            }
        }
    }

    private void UpdateDependentResources(string changedResource, int changeAmount)
    {
        switch (changedResource)
        {
            case "Economy":
                // Economy affects Happiness
                int happinessChange = Mathf.RoundToInt(changeAmount * economyHappinessFactor);
                UpdateResource("Happiness", happinessChange);
                break;

            case "Technology":
                // Technology affects Economy
                int economyChange = Mathf.RoundToInt(changeAmount * technologyEconomyFactor);
                UpdateResource("Economy", economyChange);
                break;

            case "Environment":
                // Environment affects Happiness
                int envHappinessChange = Mathf.RoundToInt(changeAmount * environmentHappinessFactor);
                UpdateResource("Happiness", envHappinessChange);
                break;
        }
    }

    public Resource GetResourceByName(string name)
    {
        switch (name)
        {
            case "Economy": return economy;
            case "Technology": return technology;
            case "Environment": return environment;
            case "Happiness": return happiness;
            default: return null;
        }
    }

    public Resource[] GetAllResources()
    {
        return new Resource[] { economy, technology, environment, happiness };
    }

    public int GetResourceValue(string resourceName)
    {
        Resource resource = GetResourceByName(resourceName);
        return resource?.value ?? 0;
    }

    public bool CheckGameOver()
    {
        // Game over if any resource reaches its minimum or maximum
        foreach (var resource in GetAllResources())
        {
            if (resource.value <= resource.minValue || resource.value >= resource.maxValue)
            {
                return true;
            }
        }
        return false;
    }
} 