// SKRYPT ODPOWIADAJĄCY ZA ZACHOWANIE NPCTA W CZASIE SPOCZYNKU(przed zaatakowaniem go lub zauwazeniem gracza)
//STANY: 
//IdleState- zachowanie podczas stania
//RandomWalkState - zachowanie podczas chodzenia


using UnityEngine;
using System.Collections;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{
    public class IdleState : FiniteStateMachineSystem {
	private Animator anim;
	private GameObject CurPlayer;
	private GameObject CurNpc;

	public bool canSee= true;

	public IdleState(GameObject player, GameObject npc)
	{
		stateID = StateID.Idle;
		CurPlayer = player;
		CurNpc = npc;
		anim = CurNpc.GetComponent<Animator>();
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		Animator anim = npc.GetComponent<Animator> ();
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);

		float dist = Vector3.Distance (player.transform.position, npc.transform.position);
		if ((dist < npc.GetComponent<Monster> ().fireBallDistance || player.GetComponentInChildren<newGun> ().coloring)&& anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
			npc.GetComponent<Monster> ().SetTransition (Transition.SawPlayer);
			Debug.Log ("idle.reason.sawplayer");

			return;

		}
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.9) {
			npc.GetComponent<Monster> ().SetTransition (Transition.IdleEnd);
			Debug.Log ("idle.reason.idleend");
			return;
		}
		/*if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle") && player.GetComponentInChildren<newGun> ().coloring) {
			npc.GetComponent<Monster> ().SetTransition (Transition.Beaten);
			Debug.Log ("idle.reason.beaten");
			return;
		}*/
	}

	public override void Act(GameObject Player, GameObject npc)
	{
		Debug.Log ("idle.act");
		//anim.SetBool ("idle", false);
		anim.SetInteger("speed", 0);
		return;
	}
}

    public class RandomWalkState : FiniteStateMachineSystem
    {
        private bool WalkTime;
        private float WalkDist;
        private GameObject CurPlayer;
        private GameObject CurNpc;

        Vector3 WalkDirection;
        private Quaternion LookRotation;
        private Quaternion StartRotation;

        private float time_t;
        private Animator anim;

        private int walkRadius;
        private UnityEngine.AI.NavMeshAgent agent;
        Vector3 randomDirection;
        MonoBehaviour lol;

        public GameObject[] waypoints;
        int desPoint = 0;
        public bool canSee = true;
        Monster mon;
        AudioSource audioo;
        bool idlesound;

        public RandomWalkState(GameObject player, GameObject npc)
        {
            mon = npc.GetComponent<Monster>();
            audioo = npc.GetComponent<AudioSource>();
            waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
            lol = npc.GetComponent<MonoBehaviour>();
            stateID = StateID.RandomWalk;
            CurPlayer = player;
            CurNpc = npc;
            anim = npc.GetComponent<Animator>();
            agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();

        }

        public override void DoBeforeEntering()
        {
            idlesound = true;

            walkRadius = 10;
            randomDirection = Random.insideUnitSphere * walkRadius;

            time_t = 0;
            WalkDirection = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            WalkTime = true;//Random.Range (2, 5);
            WalkDist = Random.Range(2, 5);
            LookRotation = Quaternion.LookRotation(WalkDirection);
            LookRotation.eulerAngles = new Vector3(0, LookRotation.eulerAngles.y, 0);
            StartRotation.eulerAngles = new Vector3(0, CurNpc.transform.eulerAngles.y, 0);

        }


        public override void Reason(GameObject player, GameObject npc)
        {

            float dist = Vector3.Distance(player.transform.position, npc.transform.position);
            if (dist < npc.GetComponent<Monster>().fireBallDistance && anim.GetCurrentAnimatorStateInfo(0).IsName(/*"creature1run"*/ "creature1walkforward"))
            {
                npc.GetComponent<Monster>().SetTransition(Transition.SawPlayer);
                Debug.Log("randomwalk.reason.sawplayer");
                return;
            }
            if (WalkTime == false && anim.GetCurrentAnimatorStateInfo(0).IsName("creature1walkforward"))
            {
                //anim.SetBool ("walk", false);
                npc.GetComponent<Monster>().SetTransition(Transition.WalkEnd);
                Debug.Log("randomwalk.reason.walkend");
                return;
            }
        }

        public override void Act(GameObject player, GameObject npc)
        {
            if (idlesound)
            {
                audioo.clip = mon.idleSounds[Random.Range(0, mon.idleSounds.Length)];
                lol.StartCoroutine(Wait());
                idlesound = false;
            }

            //float angle = Quaternion.Angle (StartRotation, LookRotation);
            //float RotateTime = angle / npc.GetComponent<Monster> ().RotateSpeed;

            //agent.destination = waypoints [desPoint].transform.position;
            //desPoint = (desPoint + 1) % waypoints.Length;
            //-------------------------------------------------

            randomDirection += lol.transform.position;
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
            Vector3 finalPosition = hit.position;

            agent.SetDestination(finalPosition);
            anim.SetInteger("speed", 1);
            Debug.Log("randomwalk.act");
            if (/*agent.remainingDistance <= dist*/Vector3.Distance(npc.transform.position, agent.destination) <= 0.5f)
            {

                //Debug.Log ("lolololololol");
                WalkTime = false;

            }
            //-------------------------------------------------
            /*if (angle > 0) {
                npc.transform.rotation = Quaternion.Slerp (StartRotation, LookRotation, time_t / RotateTime);
                time_t += Time.deltaTime;
            }

            if (time_t >= RotateTime) {
                npc.transform.Translate (Vector3.forward * Time.deltaTime * 2);
                //anim.SetBool ("walk", true);
                anim.SetInteger("speed", 1);
                WalkTime -= Time.deltaTime;
                Debug.Log ("randomwalk.act");
            }*/
        }
        IEnumerator Wait()
        {
            audioo.Play();
            if (audioo.clip != null)
                yield return new WaitForSeconds(audioo.clip.length);
        }
    }
}

