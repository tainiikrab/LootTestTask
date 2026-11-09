using System.Collections.Generic;
using LootTestTask.Inventory;
using UnityEngine;

namespace LootTestTask.UI
{
public class LootScreenMarkerDrawer : MonoBehaviour
{
    [SerializeField] private LootMarker _markerPrefab;
    [SerializeField] private ItemCollector _collector;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _markerOffset = new(0, 3, 0);

    [SerializeField] private int _poolPrewarmCount = 8;

    private readonly List<LootMarker> _markerPool = new();
    private int _usedCount;

    private void Awake()
    {
        for (int i = 0; i < _poolPrewarmCount; i++)
        {
            CreateMarker();
        }
    }

    private LootMarker CreateMarker()
    {
        LootMarker instance = Instantiate(_markerPrefab, transform);
        _markerPool.Add(instance);
        instance.ToggleVisibility(false);
        return instance;
    }

    private LootMarker GetMarker()
    {
        if (_usedCount == _markerPool.Count)
            CreateMarker();
        return _markerPool[_usedCount++];
    }

    private void Update()
    {
        int prevUsed = _usedCount;
        _usedCount = 0;

        foreach (WorldItem worldItem in _collector.CloseItems)
        {
            Vector3 position = _camera.WorldToScreenPoint(worldItem.transform.position + _markerOffset);
            if (position.z < 0) continue;

            LootMarker marker = GetMarker();
            marker.ToggleVisibility(true);

            if (worldItem == _collector.BestToPickUp)
            {
                marker.SetActiveColor();
            }
            else
            {
                marker.SetDefaultColor();
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas, position, null, out Vector2 localPosition);
            marker.RectTransform.anchoredPosition = localPosition;
        }

        for (int i = _usedCount; i < prevUsed; i++)
        {
            _markerPool[i].ToggleVisibility(false);
            _markerPool[i].SetDefaultColor();
        }
    }
}
}