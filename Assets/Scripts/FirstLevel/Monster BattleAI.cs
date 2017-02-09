/*SKRYPT ODPOWIADAJĄCY ZA ZACHOWANIE NPCTA PODCZAS WALKI
STANY:
BattleRoarState - odpowiada za kontrole okrzyku podczas pierwszego kontaktu z graczem
BattleAttackState - odpowiada za walkę wręcz(mały dystans)
BattleRunState - odpowiada za gonienie gracza gdy ten jest za daleko lub gdy ucieka
BattleRetreatState - odpowida za ucieczkę gdy npc ma mało życia
BattleBeatenState - odpowiada za zachowanie npc podczas otrzymywania obrażeń
BattleFlyBackState - odpowiada za odsunięcie się od gracza gdy ten jest za blisko
BattleFIreballState - odpowiada za atak z dystansu
DieState - odpowiada za zgon npc
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{
    public class BattleRoarState : FiniteStateMachineSystem {

	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Quaternion LookRotation;
	private Quaternion StartRotation;

	private float time_t;
	private Animator anim;

	private bool notice;
	UnityEngine.AI.NavMeshAgent agent;
	Monster mon;
	AudioSource audioo;
	bool roarsound;
	MonoBehaviour lol;
	public BattleRoarState(GameObject player, GameObject npc){
		lol = npc.GetComponent<MonoBehaviour> ();
		mon = npc.GetComponent<Monster> ();	
		audioo = npc.GetComponent<AudioSource> ();	
		stateID = StateID.BattleRoar;
		CurPlayer = player;
		CurNpc = npc;
		time_t = 0;
		MDPQstate = new QState(Action.Roar);
		anim = npc.GetComponent<Animator>();
		agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void DoBeforeEntering()
	{
		roarsound = true;
		agent.ResetPath ();
		notice = true;
		Vector3 RoarDirection = CurPlayer.transform.position - CurNpc.transform.position;
		LookRotation = Quaternion.LookRotation (RoarDirection);
		LookRotation.eulerAngles = new Vector3 (0, LookRotation.eulerAngles.y, 0);
		StartRotation.eulerAngles = new Vector3 (0, CurNpc.transform.eulerAngles.y, 0);
	}

	public override void Reason(GameObject player, GameObject npc){
		float dist = Vector3.Distance (player.transform.position, npc.transform.position);
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("creature1roar") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1.0) {
			if (dist < npc.GetComponent<Monster> ().attackDistance) {
				MDPQstate.EndState.StateAnalyze (CurPlayer, CurNpc);
				npc.GetComponent<Monster> ().SetTransition (Transition.Attack);
				Debug.Log ("roar.reason.attack");

				return;
			}

			if (dist > npc.GetComponent<Monster> ().fireBallDistance) {
				npc.GetComponent<Monster> ().SetTransition (Transition.FarAway);
				Debug.Log ("roar.reason.faraway");
				return;
			}
			if(dist > npc.GetComponent<Monster>().attackDistance && dist < npc.GetComponent<Monster>().fireBallDistance){
				MDPQstate.EndState.StateAnalyze (CurPlayer, CurNpc);
				npc.GetComponent<Monster> ().SetTransition (Transition.Falling);
				Debug.Log ("roar.reason.falling");
				return;
			}
		}
	}

	public override void Act(GameObject player, GameObject npc){
		if (roarsound) {
			audioo.clip = mon.roarClip;
			lol.StartCoroutine (Wait ());
			roarsound = false;
		}
		float angle = Quaternion.Angle (StartRotation, LookRotation);
		float RotateTime = angle / npc.GetComponent<Monster> ().RotateSpeed;
		if (angle > 0) {
			npc.transform.rotation = Quaternion.Slerp (StartRotation, LookRotation, time_t / RotateTime);
			time_t += Time.deltaTime;
		}
		if (time_t >= RotateTime && notice) {
			anim.SetTrigger ("notice");
			notice = false;
			Debug.Log ("roar.act");
		}
		return;

	}
	IEnumerator Wait(){
		if (audioo.isPlaying == false) {
			audioo.Play ();
			if (audioo.clip != null)
				yield return new WaitForSeconds (audioo.clip.length);
		}
	}
}
	
public class BattleAttackState : FiniteStateMachineSystem
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Quaternion LookRotation;
	private Quaternion StartRotation;

	private float time_t;
	private Animator anim;

	private bool Attack;
	private bool AttackAnimation;

	private float NextAttackTime;
	private float AttackRate;
	UnityEngine.AI.NavMeshAgent agent;
	public BattleAttackState (GameObject player, GameObject npc){
		stateID = StateID.BattleAttack;
		CurPlayer = player;
		CurNpc = npc;
		time_t = 0;
		anim = npc.GetComponent<Animator> ();
		NextAttackTime = 0;
		AttackRate = 1;
		MDPQstate = new QState (Action.Attack);
		agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void DoBeforeEntering()
	{
		agent.ResetPath ();
		MDPQstate = new QState (Action.Attack);
		time_t = 0;
		Attack = true;
		AttackAnimation = true;
		Vector3 AttackDirection = CurPlayer.transform.position - CurNpc.transform.position;
		LookRotation = Quaternion.LookRotation (AttackDirection);
		LookRotation.eulerAngles = new Vector3 (0, LookRotation.eulerAngles.y, 0);
		StartRotation.eulerAngles = new Vector3 (0, CurNpc.transform.eulerAngles.y, 0);

		MDPQstate.StartState.StateAnalyze (CurPlayer, CurNpc);
	}

	public override void Reason(GameObject player, GameObject npc){
		float dist = Vector3.Distance (player.transform.position, npc.transform.position);
		int CurHealth = npc.GetComponent<Monster> ().HP;

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("creature1Attack2") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1.0 && Attack == false) {
			if (CurHealth > npc.GetComponent<Monster> ().retreatHealth && dist < npc.GetComponent<Monster> ().attackDistance) {
				npc.GetComponent<Monster> ().SetTransition (Transition.Attack);
				Debug.Log ("attack.reason.attack");
				return;
			}
			if (CurHealth > npc.GetComponent<Monster> ().retreatHealth && dist > npc.GetComponent<Monster> ().fireBallDistance) {
				npc.GetComponent<Monster> ().SetTransition (Transition.FarAway);
				Debug.Log ("attack.reason.faraway");
				return;
			}

			if ( CurHealth > npc.GetComponent<Monster> ().retreatHealth && dist > npc.GetComponent<Monster> ().attackDistance && dist < npc.GetComponent<Monster> ().fireBallDistance) {
				npc.GetComponent<Monster> ().SetTransition (Transition.Falling);
				Debug.Log ("attack.reason.falling");
				return;
			}
			if (CurHealth < npc.GetComponent<Monster> ().retreatHealth) {
				npc.GetComponent<Monster> ().SetTransition (Transition.LowHealth);
				Debug.Log ("attack.reason.lowhealth");
				return;
			}


		}
		if(CurHealth<= 0){
			npc.GetComponent<Monster> ().SetTransition (Transition.StartDie);
			Debug.Log ("attack.reason.die");
			return;
		}

	}

	private IEnumerator GetActionResult()
	{
		yield return new WaitForSeconds (1);
		MDPQstate.EndState.StateAnalyze (CurPlayer, CurNpc);
		MDPQstate.UtilityAnalyze (CurPlayer, CurNpc);
		CurNpc.GetComponent<Monster> ().EnemyKnowledge.UpdateKnowledge (MDPQstate);
	}
		
	public override void Act(GameObject player, GameObject npc)
	{
		if (Attack && Time.time > NextAttackTime) {
			float angle = Quaternion.Angle (StartRotation, LookRotation);
			float RotateTime = angle / npc.GetComponent<Monster> ().RotateSpeed;
			if (angle > 0) {
				npc.transform.rotation = Quaternion.Slerp (StartRotation, LookRotation, time_t / RotateTime);
				time_t += Time.deltaTime;
			} 
				

			if (time_t >= RotateTime && AttackAnimation) {
				//if(anim.GetBool("beatenrecovera") == false)
					anim.SetTrigger("Attack");
				player.GetComponent<Player> ().DamagePlayer();
				AttackAnimation = false;

			}

			if (anim.GetCurrentAnimatorStateInfo(0).IsName("creature1Attack2") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.25 && anim.GetCurrentAnimatorStateInfo (0).normalizedTime < 1.0) {
				//Debug.Log ("Fire");
				Vector3 SpawnLocation = npc.transform.TransformPoint (0, 40, 165);
				//Object.Instantiate(npc.GetComponent<Monster>().)
				Attack = false;
				NextAttackTime = Time.time + AttackRate;
				npc.GetComponent<Monster> ().StartCoroutine (GetActionResult ());
				Debug.Log ("attack.act");
			
			}
		}

	}
}

public class BattleRunState : FiniteStateMachineSystem
{
	private GameObject CurPlayer;
	private GameObject CurNpc;
	UnityEngine.AI.NavMeshAgent agent;
	private Animator anim;
	MonoBehaviour lol;
	float runspeed;
	bool setpath;
	public BattleRunState(GameObject player, GameObject npc)
	{
		stateID = StateID.BattleRun;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator> ();
		MDPQstate = new QState (Action.Chasing);
		agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		lol = npc.GetComponent<MonoBehaviour> ();
		runspeed =  0.5f;
		setpath = true;
	}

	public override void DoBeforeLeaving()
	{
		anim.SetBool ("run", false);

	}

	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);

		if(dist < npc.GetComponent<Monster>().attackDistance)
		{
			MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
			npc.GetComponent<Monster>().SetTransition(Transition.Attack);
			Debug.Log ("Run.reason.attack");
			return;
		}

		if(dist > npc.GetComponent<Monster>().attackDistance && dist < npc.GetComponent<Monster>().fireBallDistance)
		{
			MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
			npc.GetComponent<Monster>().SetTransition(Transition.Falling);
			Debug.Log ("Run.reason.falling");
			return;
		}

		int CurHealth = npc.GetComponent<Monster>().HP;

		if(CurHealth < npc.GetComponent<Monster>().retreatHealth)
		{
			npc.GetComponent<Monster>().SetTransition(Transition.LowHealth);
			Debug.Log ("Run.reason.lowhealth");
			return;
		}

		if(CurHealth<= 0){
			npc.GetComponent<Monster> ().SetTransition (Transition.StartDie);
			Debug.Log ("run.reason.die");
			return;
		}
	}

	public override void Act(GameObject player, GameObject npc)
	{
		
		//runspeed -= Time.deltaTime;
	

			agent.SetDestination (player.transform.position);
			anim.SetBool ("run", true);
			Debug.Log ("Run.act");
		//lol.StartCoroutine (FollowTarget (player));
		/*if (runspeed <= 0) {
			Vector3 previousTargetPos = new Vector3 (float.PositiveInfinity, float.PositiveInfinity);
			while (Vector3.SqrMagnitude (agent.transform.position - player.transform.position) > 0.1f) {
				if (Vector3.SqrMagnitude (previousTargetPos - player.transform.position) > 0.1f) {
					agent.SetDestination (player.transform.position);
					previousTargetPos = player.transform.position;
					Debug.Log ("Run.act");
					anim.SetBool ("run", true);
				}

			}
			runspeed = 0.5f;
		}*/

		/*Vector3 direction = player.transform.position - npc.transform.position;

		Quaternion LookRotation = Quaternion.LookRotation(direction);
		npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, LookRotation, Time.deltaTime * 5);

		npc.transform.Translate(Vector3.forward * Time.deltaTime * 5);
		Debug.Log ("Run.act");
		anim.SetBool("run", true);*/
	}


	/*IEnumerator FollowTarget(GameObject target){
		Vector3 previousTargetPos = new Vector3 (float.PositiveInfinity, float.PositiveInfinity);
		while (Vector3.SqrMagnitude(agent.transform.position - target.transform.position)> 0.1f){
			if (Vector3.SqrMagnitude (previousTargetPos - target.transform.position) > 0.1f ){
				agent.SetDestination (target.transform.position);
				previousTargetPos = target.transform.position;
				Debug.Log ("Run.act");
				anim.SetBool ("run", true);
			}
			yield return new WaitForSeconds (0.3f);
		}
		yield return null;
	}*/
}


public class BattleRetreatState : FiniteStateMachineSystem
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Animator anim;
	private Quaternion LookRotation;
	private UnityEngine.AI.NavMeshAgent agent;

	public BattleRetreatState(GameObject player, GameObject npc) 
	{ 
		agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		stateID = StateID.BattleRetreat;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		LookRotation.eulerAngles = new Vector3(0,0,0);
	}

	public override void DoBeforeEntering()
	{
		//anim.ResetTrigger ("Attack");
		agent.ResetPath ();
		//just set the retreat direction at the first time
		if(LookRotation.eulerAngles.y == 0)
		{
			Vector3 RetreatDirection = CurNpc.transform.position - CurPlayer.transform.position;
			LookRotation = Quaternion.LookRotation(RetreatDirection);
			LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		}
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		int CurHealth = npc.GetComponent<Monster> ().HP;
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("creature1GetHit") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8)
		{
			npc.GetComponent<Monster>().SetTransition(Transition.Beaten);
			Debug.Log ("retreat.reason.beaten");
			return;
		}

		if(CurHealth<= 0){
			npc.GetComponent<Monster> ().SetTransition (Transition.StartDie);
			Debug.Log ("retreat.reason.die");
			return;
		}
	}

	public override void Act(GameObject player, GameObject npc)
	{
		if(npc.GetComponent<Monster>().HP > 0)
		{
			npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, LookRotation, 2 * Time.deltaTime);
			if(Quaternion.Angle(npc.transform.rotation, LookRotation) < 10)
			{
				npc.transform.Translate(Vector3.forward * Time.deltaTime * 3);
				anim.SetBool("lowhealth", true);
				Debug.Log ("retreat.act");
			}
		}
	}
}

public class BattleBeatenState : FiniteStateMachineSystem
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Animator anim;
	UnityEngine.AI.NavMeshAgent agent;
	bool canSee = true;
	int rand;
	public BattleBeatenState(GameObject player, GameObject npc) 
	{ 
		
		agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		stateID = StateID.BattleBeaten;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		MDPQstate =  new QState(Action.Beaten);

	}

	public override void DoBeforeEntering(){
		agent.ResetPath ();
	}
	public override void Reason(GameObject player, GameObject npc)
	{
		int CurHealth = npc.GetComponent<Monster>().HP;
		int ThreadHealth = npc.GetComponent<Monster>().retreatHealth;
		float dist = Vector3.Distance (player.transform.position, npc.transform.position);


		if(anim.GetCurrentAnimatorStateInfo(0).IsName("creature1GetHit") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8)
		{
			rand = Random.Range (1, 2);
			if(rand == 1 && (CurHealth >= ThreadHealth) && dist < npc.GetComponent<Monster> ().attackDistance )
			{
				npc.GetComponent<Monster>().SetTransition(Transition.FlyBack);
				anim.SetTrigger("flyback");
				Debug.Log ("beaten.reason.flyback");
				return;
			}

			if(CurHealth >= ThreadHealth && dist <= npc.GetComponent<Monster> ().attackDistance)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				anim.SetTrigger("beatenrecover");
				npc.GetComponent<Monster>().SetTransition(Transition.Attack);
				Debug.Log ("beaten.reason.attack");
				return;
			}

			if(CurHealth < ThreadHealth)
			{
				anim.SetTrigger("retreatebeatenrecover");
				npc.GetComponent<Monster>().SetTransition(Transition.LowHealth);
				Debug.Log ("beaten.reason.lowhealth");
				return;
			}

			if ( CurHealth >= 100 && dist > npc.GetComponent<Monster> ().attackDistance && canSee == true) {
				anim.SetTrigger("notice");
				npc.GetComponent<Monster>().SetTransition(Transition.SawPlayer);
				Debug.Log ("beaten.reason.sawplayer");
				canSee = false;
				return;
			}
			if(dist > npc.GetComponent<Monster> ().fireBallDistance){
				anim.SetTrigger ("follow");
				npc.GetComponent<Monster>().SetTransition(Transition.FarAway);
				return;
			}
			//if (anim.GetBool ("Attack") == false){
			if (CurHealth >= ThreadHealth && dist > npc.GetComponent<Monster> ().attackDistance && dist <= npc.GetComponent<Monster> ().fireBallDistance) {
				MDPQstate.EndState.StateAnalyze (CurPlayer, CurNpc);
				anim.SetTrigger ("beatenrecovera");
				npc.GetComponent<Monster> ().SetTransition (Transition.Falling);
				Debug.Log ("beaten.reason.fireball");
				return;
			//}
			}
			if(CurHealth<= 0){
				npc.GetComponent<Monster> ().SetTransition (Transition.StartDie);
				Debug.Log ("beaten.reason.die");
				return;
			}
		}

	}
	public override void Act(GameObject player, GameObject npc)
	{
		Debug.Log ("monster has been beaten");
		return;
	}
}

public class BattleFlyBackState : FiniteStateMachineSystem
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Animator anim;

	private Vector3 FlyDirection;

	private float FlyTime;
	private float FlyTotalTime;
	private bool CompleteFlag;

	public BattleFlyBackState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleFlyBack;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		FlyDirection = new Vector3(0,0,0);
		FlyTime = 0;
		FlyTotalTime = 1.5f;
		CompleteFlag = false;
		MDPQstate = new QState(Action.FlyBack);
	}

	public override void DoBeforeEntering()
	{
		FlyDirection = (CurNpc.transform.position - CurPlayer.transform.position).normalized;

		FlyTime = 0;
		CompleteFlag = false;
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		int CurHealth = npc.GetComponent<Monster> ().HP;
		if(CompleteFlag)
		{
			float dist = Vector3.Distance(player.transform.position, npc.transform.position);

			if(dist < npc.GetComponent<Monster>().attackDistance)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				npc.GetComponent<Monster>().SetTransition(Transition.Attack);
				Debug.Log ("flyback.reason.attack");
				return;
			}

			if(dist > npc.GetComponent<Monster>().attackDistance && dist < npc.GetComponent<Monster>().fireBallDistance)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				npc.GetComponent<Monster>().SetTransition(Transition.Falling);
				Debug.Log ("flyback.reason.falling");
				return;
			}

			if(dist > npc.GetComponent<Monster>().fireBallDistance)
			{
				npc.GetComponent<Monster>().SetTransition(Transition.FarAway);
				Debug.Log ("flyback.reason.faraway");
				return;
			}
		}
		if(CurHealth<= 0){
			npc.GetComponent<Monster> ().SetTransition (Transition.StartDie);
			Debug.Log ("flyback.reason.die");
			return;
		}
	}

	public override void Act(GameObject player, GameObject npc)
	{
		//npc.GetComponent<Rigidbody>().MovePosition(npc.transform.position + FlyDirection * Time.deltaTime * 10);
		npc.transform.Translate(Vector3.back * Time.deltaTime * 5);
		FlyTime += Time.deltaTime;
		if(FlyTime >= FlyTotalTime)
		{
			anim.SetTrigger("flybackrecover");
			CompleteFlag = true;
			Debug.Log ("flyback.act");
		}
	}
}

public class BattleFireballState : FiniteStateMachineSystem
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Quaternion LookRotation;
	private Quaternion StartRotation;

	private float time_t;
	private Animator anim;

	public bool Fire;
	private bool FireAnimation;

	private float NextFireTime;
	private float FireRate;


	UnityEngine.AI.NavMeshAgent agent;
	GameControl controled;
	GameObject controledObject;
	Monster mon;
	AudioSource audioo;
	bool fireballsound;
	MonoBehaviour lol;
	public BattleFireballState(GameObject player, GameObject npc) 
	{ 
		lol = npc.GetComponent<MonoBehaviour> ();
		mon = npc.GetComponent<Monster> ();	
		audioo = npc.GetComponent<AudioSource> ();	
		agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		stateID = StateID.BattleFireball;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		FireRate = 1;
		NextFireTime = 0;
		MDPQstate = new QState(Action.FireBall);
		controledObject = GameObject.Find ("GameControl");
		controled = controledObject.GetComponent<GameControl> ();
	}
	public override void DoBeforeEntering()
	{
		fireballsound = true;
		agent.ResetPath ();
		MDPQstate = new QState(Action.FireBall);
		time_t = 0;
		Fire = true;
		FireAnimation = true;
		LookRotation = Quaternion.LookRotation(CurPlayer.transform.position - CurNpc.transform.position);
		LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		StartRotation.eulerAngles = new Vector3(0,CurNpc.transform.eulerAngles.y,0);

		MDPQstate.StartState.StateAnalyze(CurPlayer, CurNpc);
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		int CurHealth = npc.GetComponent<Monster>().HP;

		if(anim.GetCurrentAnimatorStateInfo(0).IsName("creature1Attack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0 && Fire == false)
		{
			if(CurHealth > npc.GetComponent<Monster>().retreatHealth && dist < npc.GetComponent<Monster>().attackDistance)
			{
				npc.GetComponent<Monster>().SetTransition(Transition.Attack);
				Debug.Log ("fireball.reason.attack");
				return;
			}

			if(CurHealth > npc.GetComponent<Monster>().retreatHealth && dist > npc.GetComponent<Monster>().attackDistance && dist <= npc.GetComponent<Monster>().fireBallDistance)
			{
				npc.GetComponent<Monster>().SetTransition(Transition.Falling);
				Debug.Log ("fireball.reason.falling");
				return;
			}

			if(CurHealth > npc.GetComponent<Monster>().retreatHealth && dist > npc.GetComponent<Monster>().fireBallDistance)
			{
				npc.GetComponent<Monster>().SetTransition(Transition.FarAway);
				Debug.Log ("fireball.reason.faraway");
				return;
			}
		}

		if(CurHealth < npc.GetComponent<Monster>().retreatHealth)
		{
			npc.GetComponent<Monster>().SetTransition(Transition.LowHealth);
			return;
		}
		if(CurHealth<= 0){
			npc.GetComponent<Monster> ().SetTransition (Transition.StartDie);
			Debug.Log ("fireball.reason.die");
			return;
		}
	}

	private IEnumerator GetActionResult()
	{
		yield return new WaitForSeconds(1.5f);
		MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
		MDPQstate.UtilityAnalyze(CurPlayer, CurNpc);
		CurNpc.GetComponent<Monster> ().EnemyKnowledge.UpdateKnowledge (MDPQstate);
	}

	public override void Act(GameObject player, GameObject npc)
	{
		if(Fire && Time.time > NextFireTime && controled.canfireball)
		{
			float angle = Quaternion.Angle(StartRotation, LookRotation);
			float RotateTime = angle / npc.GetComponent<Monster>().RotateSpeed;
			if(angle > 0)
			{
				//rotation finish in one second
				npc.transform.rotation = Quaternion.Slerp(StartRotation, LookRotation, time_t/RotateTime);
				time_t += Time.deltaTime;
			}
			//check rotation complete
			if(time_t >= RotateTime && FireAnimation)
			{
				anim.SetTrigger("fireball");
				FireAnimation = false;
				Debug.Log ("fireball.act");
			}

			if(anim.GetCurrentAnimatorStateInfo(0).IsName("creature1Attack1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0)
			{
				
				//Debug.Log("fireBall");
				Object.Instantiate(npc.GetComponent<Monster>().fireBall, npc.GetComponent<Monster>().fireballSpawn.transform.position, npc.transform.rotation);
				Fire = false;

				NextFireTime = Time.time + FireRate;
				npc.GetComponent<Monster>().StartCoroutine(GetActionResult());

			}
			if (fireballsound) {
				audioo.clip = mon.fireballSouund;
				lol.StartCoroutine (Wait(npc));
				fireballsound = false;
			}
		}

	}

	IEnumerator Wait(GameObject npc){
		//if (audioo.isPlaying == false) {
			audioo.Play ();
			if (audioo.clip != null)
				yield return new WaitForSeconds (audioo.clip.length);
		}
	//}
}

    public class DieState : FiniteStateMachineSystem
    {

        MonoBehaviour lol;
        private Animator anim;
        UnityEngine.AI.NavMeshAgent agent;
        GameObject score;
        GameObject[] weapons;
        public bool dying = false;

        Monster mon;
        AudioSource audioo;
        bool diesound;

        public DieState(GameObject player, GameObject npc)
        {

            mon = npc.GetComponent<Monster>();
            audioo = npc.GetComponent<AudioSource>();
            weapons = GameObject.FindGameObjectsWithTag("Gun");
            score = GameObject.Find("GameControl");
            stateID = StateID.Die;
            MDPQstate = new QState(Action.Die);
            anim = npc.GetComponent<Animator>();
            agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
            lol = npc.GetComponent<MonoBehaviour>();
        }
        public override void DoBeforeEntering()
        {

            diesound = true;
            score.GetComponent<GameControl>().score -= 1;
            //Debug.Log ("lalalalalalalalalalaal");
            agent.ResetPath();
            anim.SetBool("Die", true);
        }


        public override void Reason(GameObject player, GameObject npc)
        {

        }

        public override void Act(GameObject player, GameObject npc)
        {

            if (diesound)
            {
                audioo.clip = mon.dieSounds;
                lol.StartCoroutine(Wait());
                diesound = false;
            }
            dying = true;

            if (npc.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && npc.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("creature1Die"))
            {
                Debug.Log("trolololo");
                MonoBehaviour.Destroy(npc);
            }


            //lol.StartCoroutine (DieNow(npc));
        }
        /*private IEnumerator DieNow(GameObject npc){
            Debug.Log ("omomomomm");
            yield return new WaitForSeconds (5);
            MonoBehaviour.Destroy (npc);
            lol.StopCoroutine (DieNow (npc));

        }*/
        IEnumerator Wait()
        {
            if (audioo.isPlaying == false)
            {
                audioo.Play();
                if (audioo.clip != null)
                    yield return new WaitForSeconds(audioo.clip.length);
            }
        }
    }
}
