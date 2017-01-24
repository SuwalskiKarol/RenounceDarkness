using UnityEngine;
using System.Collections;
namespace Scripts.FirstLevel
{
    public class FireBall : MonoBehaviour
    {
        [Header("Fireball speed")]
        [Range(0, 100)]
        public int speed = 1;
        private GameObject player;
        public GameObject boom;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Destroy(gameObject, 3);
        }
        void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag != "Enemy")
            {
                Instantiate(boom, transform.position, Quaternion.identity);
                return;
            }
            if (collision.gameObject.tag == "Player")
            {
                Destroy(gameObject);
                collision.gameObject.GetComponent<Player>().DamagePlayer();
                collision.gameObject.GetComponent<Player>().getHit = true;
                return;
            }
            if (collision.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
