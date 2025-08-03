using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts.Projectiles
{
    public class VisualProjectile : MonoBehaviour
    {
        private float _speed = 10f;

        public void Initialize(Vector3 position, Vector3 direction, float speed)
        {
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(direction);
            _speed = speed;
        }

        void Update()
        {
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }
    }
}