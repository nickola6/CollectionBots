using UnityEngine;

[CreateAssetMenu(fileName = "ResourceSettings", menuName = "Scriptable Objects/Resource Settings")]
public class ResourceSettings : ScriptableObject
{
    [SerializeField] private ResourceType _type;

    public ResourceType Type => _type;
}