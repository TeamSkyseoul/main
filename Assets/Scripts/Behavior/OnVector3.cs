using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnVector3")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnVector3", message: "On [Vector3]", category: "Events", id: "f9523e103f102c603ed404ccf5d08414")]
public sealed partial class OnVector3 : EventChannel<Vector3> { }

