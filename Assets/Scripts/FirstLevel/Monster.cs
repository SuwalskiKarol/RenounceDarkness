// SKRYPT SŁUŻY DO TWORZENIA MAPY STANY/AKCJE. DODAWANIE AKCJI DO POSZCZEGÓLNYCH STANÓW, ODPOWIADA RÓWNIEŻ ZA PODSTAWOWE FUNKCJE NPC TAKIE JAK OTRZYMYWANIE OBRAŻEŃ CZY USTALANIE ILOŚCI ŻYCIA
//SAME STANY TWORZONE SĄ W SKRYPTACH MonsterbattleAI, MonsterRelaxAI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.FirstLevel;

public class Monster : MonoBehaviour {
	public bool getHit = false;
	public List<GameObject> weapons = new List<GameObject>();
	public GameObject cross;
	Animator anim;
	ChangeWeapons skrypt;
	public int HP;
	public int DamageTaken;
	public float hittingTime = 0f;


	public bool hitting;
	public GameObject fireballSpawn;

	// new
	public GameObject player;
	public GameObject FireBall;
	private FSMSystem fsm;

	public KnowledgeSystem EnemyKnowledge { get; set;}
	public Dictionary<StateID, Action> StateIDToActionID{ get; set;}
	public int RotateSpeed = 120;
	public int FireBallDistance = 20;
	public int AttackDistance = 3;
	public int RetreatHealth = 5;
	private int rand;
	public AudioClip[] gethitSounds;
	AudioSource audioo;
	public AudioClip[] idleSounds;
	public AudioClip dieSounds;
	public AudioClip roarClip;
	public AudioClip fireballSouund;

	public GameObject spawnFire;
	//int randomGetHitSound;
	void Start () {
		Instantiate (spawnFire, transform.position, Quaternion.identity);

		audioo = GetComponent<AudioSource> ();
		weapons.Add(GameObject.Find("SciFiGun_Specular"));
		weapons.Add(GameObject.Find("AssaultRifle"));
		cross = GameObject.Find ("Crosshair");
		HP = 100;
		DamageTaken = 0;
		anim = GetComponent<Animator> ();
		skrypt = cross.GetComponent<ChangeWeapons> ();
		EnemyKnowledge = new KnowledgeSystem ();

		player = GameObject.FindGameObjectWithTag ("Player");
		//-----------------------------------------------------------
		StateIDToActionID = new Dictionary<StateID, Action> () {
			{ StateID.BattleFireball, Action.FireBall },
			{ StateID.BattleAttack, Action.Attack }
		};

		IdleState idle = new IdleState (player, gameObject);
		idle.AddTransition (Transition.SawPlayer, StateID.BattleRoar);
		idle.AddTransition (Transition.IdleEnd, StateID.RandomWalk);
		idle.AddTransition(Transition.Beaten, StateID.BattleBeaten);


		RandomWalkState walk = new RandomWalkState (player, gameObject);
		walk.AddTransition (Transition.SawPlayer, StateID.BattleRoar);
		walk.AddTransition (Transition.WalkEnd, StateID.Idle);
		walk.AddTransition(Transition.Beaten, StateID.BattleBeaten);


		BattleRoarState roar = new BattleRoarState (player, gameObject);
		roar.AddTransition(Transition.Falling, StateID.BattleFireball);
		roar.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		roar.AddTransition(Transition.Attack, StateID.BattleAttack);
		roar.AddTransition(Transition.FarAway, StateID.BattleRun);


		BattleAttackState attack = new BattleAttackState (player, gameObject);
		attack.AddTransition(Transition.Attack, StateID.BattleAttack);
		attack.AddTransition(Transition.FarAway, StateID.BattleRun);
		attack.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		attack.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		attack.AddTransition(Transition.Falling, StateID.BattleFireball);
		attack.AddTransition(Transition.StartDie, StateID.Die);
	

		BattleRunState run = new BattleRunState(player, gameObject);
		run.AddTransition(Transition.Attack, StateID.BattleAttack);
		run.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		run.AddTransition(Transition.Falling, StateID.BattleFireball);
		run.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		run.AddTransition(Transition.StartDie, StateID.Die);

		BattleRetreatState retreat = new BattleRetreatState(player, gameObject);
		retreat.AddTransition (Transition.Beaten, StateID.BattleBeaten);
		retreat.AddTransition(Transition.StartDie, StateID.Die);

		BattleBeatenState beaten = new BattleBeatenState(player, gameObject);
		beaten.AddTransition(Transition.Attack, StateID.BattleAttack);
		beaten.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		beaten.AddTransition(Transition.FlyBack, StateID.BattleFlyBack);
		beaten.AddTransition (Transition.Falling, StateID.BattleFireball);
		beaten.AddTransition (Transition.SawPlayer, StateID.BattleRoar);
		beaten.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		beaten.AddTransition(Transition.FarAway, StateID.BattleRun);
		beaten.AddTransition(Transition.StartDie, StateID.Die);

		BattleFlyBackState flyback = new BattleFlyBackState(player, gameObject);
		flyback.AddTransition(Transition.Attack, StateID.BattleAttack);
		flyback.AddTransition(Transition.Falling, StateID.BattleFireball);
		flyback.AddTransition(Transition.FarAway, StateID.BattleRun);
		flyback.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		flyback.AddTransition(Transition.StartDie, StateID.Die);
		
		BattleFireballState fireball = new BattleFireballState(player, gameObject);
		fireball.AddTransition(Transition.Attack, StateID.BattleAttack);
		fireball.AddTransition(Transition.FarAway, StateID.BattleRun);
		fireball.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		fireball.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		fireball.AddTransition(Transition.Falling, StateID.BattleFireball);
		fireball.AddTransition(Transition.StartDie, StateID.Die);

		DieState die = new DieState(player, gameObject);

		fsm = new FSMSystem ();
		fsm.AddState(idle);
		fsm.AddState(walk);
		fsm.AddState(roar);
		fsm.AddState(attack);
		fsm.AddState(run);
		fsm.AddState(retreat);
		fsm.AddState(beaten);
		fsm.AddState(flyback);
		fsm.AddState(fireball);
		fsm.AddState(die);
		//---------------------------------------------------

	}
	// Update is called once per frame
	void Update () {
		
		//================
		if (anim.GetBool ("Attack") == true && anim.GetBool ("beatenrecovera") == true) {
			//Debug.Log ("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
			//SetTransition (Transition.StartDie);
			anim.ResetTrigger ("Attack");
			anim.ResetTrigger ("beatenrecovera");
			anim.SetBool ("Die", true);
			audioo.clip = dieSounds;
			audioo.Play ();
			Debug.Log ("laalalala");
			StartCoroutine (Wait ());

		}
		//================
	
		fsm.CurrentState.Reason (player, gameObject);
		fsm.CurrentState.Act (player, gameObject);

		/*
		foreach (GameObject go in weapons) {
			if(go.GetComponent<newGun>().hit.transform.name == this.gameObject.name && skrypt.cross.color == Color.red  go.GetComponent<newGun>().CrossColor()){
				anim.SetBool ("GetHit", true);

				if (go.GetComponent<Gun> ().isEquipment) {
					
					if (go.GetComponent<Gun> ().HandGun) {
						HP -= 10;

					}
					if (go.GetComponent<Gun> ().AssaultRifle) {
						
						hittingTime -= Time.deltaTime;
						if (hittingTime <= 0) {
							HP -= 1;
							hittingTime = 0.5f;
						}
					}

				}
			}else
				anim.SetBool ("GetHit", false);

			
		}

*/
		if (hitting && !anim.GetCurrentAnimatorStateInfo(0).IsName("creature1GetHit")) {
			rand = Random.Range (1, 3);
			foreach (GameObject go in weapons) {
				if (go.GetComponent<Gun> ().HandGun && Input.GetMouseButtonDown(0) ||go.GetComponent<Gun> ().AssaultRifle && Input.GetMouseButton(0)) {
					hittingTime -= Time.deltaTime;
					if (hittingTime < 0) {
						//anim.SetBool ("GetHit", true);
						if(anim.GetCurrentAnimatorStateInfo(0).IsName("creature1Die") != true){
						if (rand == 2) {
							audioo.clip =gethitSounds[ Random.Range (0, gethitSounds.Length)];
							SetTransition (Transition.Beaten);
							anim.SetTrigger ("beaten");
							audioo.Play ();
						}
						}
						HP -= 20;
						DamageTaken += 20;
						hittingTime = 0.4f;
					}//else
						//anim.SetBool ("GetHit", false);
				}
			}
		}


	}

	IEnumerator Wait(){
		yield return new WaitForSeconds (2);
		Debug.Log ("trolololololololololololol");
		Destroy (this.gameObject);
	}
	public void SetTransition(Transition t)
	{
		StateID NextActionStateID = fsm.CurrentState.GetOutputState (t);
		if (StateIDToActionID.ContainsKey (NextActionStateID)) {
			Action nextAction = StateIDToActionID [NextActionStateID];
			float ActionValue = EnemyKnowledge.GetStateActionValue (fsm.CurrentState.MDPQstate.EndState, nextAction);
			if (ActionValue < -0.006 || EnemyKnowledge.GetStateLearningInfo (fsm.CurrentState.MDPQstate.EndState)) {
				StateID NextStateID = EnemyKnowledge.UCBPolicy (fsm.CurrentState.MDPQstate.EndState);
				fsm.PerformAITransition (NextStateID);
				return;

			}
		}
		fsm.PerformTransition (t);
	}

	void Attack(){
	}

}
