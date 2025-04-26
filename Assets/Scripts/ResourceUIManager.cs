using UnityEngine;
using System.Collections.Generic;

public class ResourceUIManager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceIconMapping
    {
        public string resourceName;
        public ResourceIconUI iconUI;
    }

    [SerializeField] private List<ResourceIconMapping> resourceIcons;
    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            resourceManager.OnResourcesUpdated.AddListener(UpdateResourceIcons);
        }
    }

    private void UpdateResourceIcons(ResourceManager.Resource[] resources)
    {
        foreach (var resource in resources)
        {
            var iconMapping = resourceIcons.Find(x => x.resourceName == resource.name);
            if (iconMapping != null && iconMapping.iconUI != null)
            {
                iconMapping.iconUI.SetValue(resource.value);
            }
        }
    }
} 