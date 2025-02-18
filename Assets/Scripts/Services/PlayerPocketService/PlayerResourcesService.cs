using System;
using System.Collections.Generic;
using Settings;
using Static;

namespace Services
{
    public class PlayerResourcesService : IDisposable
    {
        private PlayerDataManager _dataManager;
        private Dictionary<ResourceNames, Resource> _resourcesConfig;

        private Dictionary<ResourceType, List<ResourceNames>> _resourcesByType;
        private Dictionary<ResourceType, List<ResourceDemand>> _resourcesWithCount;

        private bool _isFreezeMode;
        private ResourcesHolder _freezeResourcesHolder = new ResourcesHolder();
        private ResourcesHolder ResourcesHolder => _dataManager.PlayerResources;

        public ResourcesHolder FrozenResourcesHolder => _freezeResourcesHolder;
        public Action<ResourceType> OnResourcesUpdated;
        public Action<ResourceNames, float> OnResourceAdded;
        public Dictionary<ResourceType, List<ResourceDemand>> ResourcesWithCount => _resourcesWithCount;
        
        public PlayerResourcesService(SettingsService settingsService, PlayerDataManager dataManager)
        {
            _dataManager = dataManager;
            _resourcesConfig = settingsService.GameResources;
        }

        public void Init()
        {
            ResourcesHolder.OnItemChanged += RebuildResourcesLists;
            _resourcesByType = new Dictionary<ResourceType, List<ResourceNames>>();
            _resourcesWithCount = new Dictionary<ResourceType, List<ResourceDemand>>();

            foreach (var resource in ResourcesHolder.Values)
            {
                var currentResourceType = _resourcesConfig[resource.Key].ResourceType;
                if (!_resourcesByType.TryGetValue(currentResourceType, out var resourceList))
                {
                    resourceList = new List<ResourceNames>();
                    _resourcesByType[currentResourceType] = resourceList;
                }

                if (resourceList.Contains(resource.Key))
                {
                    continue;
                }
                resourceList.Add(resource.Key);
            }

            foreach (var kv in _resourcesByType)
            {
                RebuildResourceContainers(kv.Key);
            }
        }
        
        public void Dispose()
        {
            ResourcesHolder.OnItemChanged -= RebuildResourcesLists;
        }
        
        public void AddResource(ResourceNames id, float value)
        {
            if (_isFreezeMode)
            {
                _freezeResourcesHolder.AddResource(id, value);
                return;
            }
            ResourcesHolder.AddResource(id, value);
            OnResourceAdded(id, value);
        }

        public float GetResource(ResourceNames id)
        {
            return ResourcesHolder.Get(id);
        }

        private void RebuildResourcesLists(ResourceNames id)
        {
            var currentResourceType = _resourcesConfig[id].ResourceType;
            if (!_resourcesByType.TryGetValue(currentResourceType, out var resourceList))
            {
                resourceList = new List<ResourceNames>();
                _resourcesByType[currentResourceType] = resourceList;
            }

            if (!resourceList.Contains(id))
            {
                resourceList.Add(id);
            }
            RebuildResourceContainers(currentResourceType);
        }
        
        private void RebuildResourceContainers(ResourceType resourceType)
        {
            if (!_resourcesWithCount.TryGetValue(resourceType, out var l))
            {
                l = new List<ResourceDemand>();
                _resourcesWithCount[resourceType] = l;
            } 
            l.Clear();

            if (!_resourcesByType.TryGetValue(resourceType, out var savedResources))
            {
                savedResources = new List<ResourceNames>();
                _resourcesByType.Add(resourceType, savedResources);
            }
            for (int i = 0; i < savedResources.Count; i++)
            {
                var id = savedResources[i];
                var value = ResourcesHolder.Get(id);
                l.Add(new ResourceDemand(id, (int)Math.Floor(value)));
            }
            OnResourcesUpdated?.Invoke(resourceType);
        }

        public bool CanBuy(List<ResourceDemand> resources)
        {
            if (resources == null)
            {
                return false;
            }
            foreach (var r in resources)
            {
                if (!CanBuy(r))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanBuy(ResourceDemand resource)
        {
            var a = ResourcesHolder.Get(resource.ResourceId);
            if (_isFreezeMode)
            {
                a += _freezeResourcesHolder.Get(resource.ResourceId);
            }
            var b = resource.Value;
            return a >= b;
        }
        
        public bool TryBuy(List<ResourceDemand> resources)
        {
            if (!CanBuy(resources))
            {
                return false;
            }
            
            foreach (var r in resources)
            {
                SpendResource(r.ResourceId, r.Value);
            }

            return true;
        }
        
        public bool TryBuy(ResourceDemand resource)
        {
            if (!CanBuy(resource))
            {
                return false;
            }
            
            SpendResource(resource.ResourceId, resource.Value);
            return true;
        }

        private void SpendResource(ResourceNames id, int value)
        {
            if (!_isFreezeMode)
            {
                ResourcesHolder.AddResource(id, -value);
                return;
            }
            
            float inBasePocket = ResourcesHolder.Get(id);
            float newValue = inBasePocket - value;
            if (newValue >= 0)
            {
                ResourcesHolder.AddResource(id, -value);
                return;
            }
            ResourcesHolder.AddResource(id, -inBasePocket);
            _freezeResourcesHolder.AddResource(id, newValue);
        }

        public void FreezePocket()
        {
            _isFreezeMode = true;
        }
        public void UnfreezePocket()
        {
            _isFreezeMode = false;
        }

        public void TransferFreezeResources(float multiplier = 1)
        {
            if (_freezeResourcesHolder == null || _freezeResourcesHolder.Values == null)
            {
                return;
            }
            foreach (var item in _freezeResourcesHolder.Values)
            {
                AddResource(item.Key, item.Value * multiplier);
            }
            _freezeResourcesHolder.Values.Clear();
        }
    }
}