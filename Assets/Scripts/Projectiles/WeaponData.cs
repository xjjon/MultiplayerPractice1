using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts.Projectiles
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public int Id;
        public ProjectileType Type;
        public GameObject VisualProjectilePrefab;
        public GameObject ImpactVfxPrefab;
        public AudioClip ImpactSfx;
        public float Speed = 10f;
        public float Lifetime = 5f;

        public float Range = 10;

        public LayerMask HitMask;
    }

    public enum ProjectileType
    {
        Raycast,
        Simulated
    }
}