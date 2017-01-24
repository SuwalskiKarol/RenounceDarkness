/*  							NOTATKI
 *
 *
 *
		(http://staff.elka.pw.edu.pl/~pcichosz/um/wyklad/wyklad12/wyklad12.html    < wzory, schematy i inne matematyczne pierdy RL>)
		
				PROCES DECYZYJNY MARKOWA
MDP = <X, A, q(phi), a(delta)> (doświadczenie)
X = skończony zbiór stanów
A = skończony zbiór akcji
q = funkcja nagrody(na wyjście: zmienna losowa (r) oznaczająca nagrodę otrzymywaną po wykonaniu akcji a  w stanie x)
a = funkcja przejścia stanów(na wyjście: zmienna losowa oznaczająca następny stan po wykonaniu akcji a w stanie x)(strategia)

Własność Markowa :
q i a nie zależą od historii. W każdym kroku nagroda i następny stan zależą (probabilistycznie) tylko od aktualnego stanu i akcji.

Uczenie nieindukcyjne. Brak dużej liczby przykładów, brak nadzoru, brak uczenia wiedzy deklaratywnej.
Uczenie się wiedzy proceduralnej(umiejętności?). 


STRATEGIA - wybieranie akcji
Strategia optymalna - każda strategia, dla której nie istnieje strategia od niej lepsza.(każda strategia maksymalizująca wartość każdego stanu.)
strategia zachłanna - null;


Podstawą uczenia się ze wzmocnieniem jest uczenie się funkcji wartości lub funkcji wartości akcji BEZ ZNAJOMOŚCI ŚRODOWISKA. 
Większość wykorzystywanych w tym celu algorytmów opiera się na METODACH RÓŻNIC CZASOWYCH (temporal differences, TD).
<na końcu pliku>
ALGORYTM TD<podstawa uczenia strategii, inne algorytmy są rozbudowanie tego>


------> UCZENIE SIE STRATEGII<------------
Celem uczenia się ze wzmocnieniem jest nauczenie się strategii optymalnej (lub strategii dobrze przybliżającej strategię optymalną), 
zaś uczenie się funkcji wartości może być jedynie środkiem do tego celu. 

ALGORYTMY:
AHC - null (podobno rzadko używany, trudniejszy do zrozumienia teoretycznie niz q-learning. nie czytam);


Q-LEARNING
- uczy się optymalnej funkcji wartości akcji, tak aby móc uzyskać strategię optymalną jako zachłanną względem niej.
 tablica 2 < http://staff.elka.pw.edu.pl/~pcichosz/um/wyklad/wyklad13/wyklad13.html> 

Wartości Q wyznaczają oczywiście pośrednio strategię (zachłanną), 
jednak w przeciwieństwie do algorytmu AHC dla procesu uczenia się nie wymaga się,
aby akcje były w kolejnych krokach wybierane zgodnie z tą strategią (choćby w sensie zgodności probabilistycznej), 
w związku z czym Q-learning należy do kategorii algorytmów OFF-POLICY (może posługiwać się inną strategią, niż ta, której się uczy). 
<MONTE-CARLO poczytac>


w każdej chwili w każdym stanie każda akcja może zostać wybrana do wykonania z pewnym niezerowym prawdopodobieństwem 
(inaczej mówiąc, w każdym stanie każda akcja będzie wykonana nieskończenie wiele razy, jeśli algorytm działa nieskończenie długo).

Dostateczna eksploracja jest jednym z warunków zbieżności algorytmu Q-learning.

WYBÓR AKCJI

Eksploracja a eksploatacja - wymiana między działaniem w celu pozyskania wiedzy a działaniem w celu pozyskania nagród. 
Jest oczywiste, że oczekujemy od ucznia poprawy działania (czyli zwiększania dochodów) w trakcie uczenia się, a więc eksploatacji. 
Z drugiej strony, jeśli jego aktualna strategia nie jest optymalna, to musi on poznać (i docenić) efekty innych akcji, niż wynikające z tej strategii, a więc eksplorować. 

-strategia zachłanna-null
-strategia oparta na rozkladzie boltzmanna - null
-strategie licznikowe - null;

ALGORYTM UCB
<https://jeremykun.com/2013/10/28/optimism-in-the-face-of-uncertainty-the-ucb1-algorithm/>

UCB korzysta z badań nad problemem z teorii gier – MAB (ang. Multi-Armed Bandit). W problemie
MAB rozważa się maszynę hazardową wyposażoną w N ramion. W każdym kroku, gracz
może wybrać jedno z N ramion urządzenia. Celem gracza jest maksymalizacja nagrody. 


REPREZENTACJA FUNKCJI; NULL


--------------------------------------------(info bonusowe)----------------------------------------------------------------------------------
PROGRAMOWANIE DYNAMICZNE(Bellman)	
Dla dowolnego procesu decyzyjnego Markowa istnieje przynajmniej jedna (stacjonarna, deterministyczna) strategia optymalna. 
Każdej strategii optymalnej odpowiada ta sama optymalna funkcja wartości i optymalna funkcja wartości akcji. 
Metody programowania dynamicznego pozwalają na wyznaczenie dowolnej z tych funkcji pod warunkiem znajomości  funkcji przejścia i wzmocnienia.

Wartościowanie strategii(równianie Bellmana. Tablica 4, 5)


wyznaczanie strategi optymalnej

Istnieją dwa warianty metod programowania dynamicznego do wyznaczania strategii optymalnych. 
Pierwszy z nich, nazywany iteracją strategii, generuje ciąg strategii, w którym każda kolejna strategia jest lepsza od następnej (lub obie są optymalne). 
Drugi, nazywany iteracją wartości, oblicza optymalną funkcję wartości (lub wartości akcji) za pomocą stosowania równania optymalności Bellmana jako reguły aktualizacji.
(tablica 6,7)

porównanie uczenie ze wzmocnieniem a programowanie dynamiczne (str 27)

-dynamiczne - wymagana znajomość funcji przejścia, RL- funkcja przejscia jest nieznana, wykorzystuje faktycznie zaobserwowane nagrody i przejścia stanów.
-programowanie dynamiczne opiera się na wyczerpującym przeglądaniu całej przestrzeni stanów i akcji, podczas uczenie się ze wzmocnieniem wykorzystuje faktyczne trajektorie,
-programowanie dynamiczne prowadzi do obliczenia pełnej strategii optymalnej, 
podczas gdy uczenie się ze wzmocnieniem ma w gruncie rzeczy na celu działanie (w przybliżeniu) optymalne, 
które może być oparte na częściowej strategii (nie jest konieczne nauczenie się optymalnej strategii dla stanów, które nie występują w trakcie faktycznego działania ucznia).


----------------------------------------------------------------------------------------------------------------------------------------------------------------------



*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{
    public enum Action
    {
        None,
        Attack,
        FireBall,
        Roar,
        Chasing,
        FlyBack,
        Beaten,
        Die,
    }
    //obserwacja stanu. Metoda wykorzystywana w stanach: BattleAttackState, BattleFireballState, BattleRunState
    public class State : System.IEquatable<State>
    {
        public int PlayerNpcDistance { get; set; }
        public int PlayerNpcEulerAngle { get; set; }

        public State(int distance, int EulerAngle, int PlayerHp, int NpcHp)
        {
            PlayerNpcDistance = distance;
            PlayerNpcEulerAngle = EulerAngle;
        }

        public State() { }

        public void StateAnalyze(GameObject player, GameObject npc)
        {
            float distance = Vector3.Distance(player.transform.position, npc.transform.position);
            PlayerNpcDistance = (int)(distance >= 10 ? 10 : distance) / 3;

            Vector3 TargetDirection = player.transform.position - npc.transform.position;
            float EulerAngle = Mathf.Acos(Vector3.Dot(TargetDirection.normalized, npc.transform.forward.normalized)) * Mathf.Rad2Deg;

            //PlayerNpcEulerAngle = (int)(EulerAngle > 180 ? EulerAngle - 180 : EulerAngle) / 90;
        }

        public bool Equals(State other)
        {
            var otherState = other as State;
            if (otherState == null)
                return false;
            return PlayerNpcDistance == otherState.PlayerNpcDistance;// && PlayerNpcEulerAngle == otherState.PlayerNpcEulerAngle; 
        }

        public override int GetHashCode()
        {
            return PlayerNpcDistance * 10;//+ PlayerNpcEulerAngle;
        }
    }

    //obliczanie nagrody.
    public class QState
    {
        public State StartState { get; set; }
        public Action CurrentAction { get; set; }
        public float Reward { get; set; }
        public State EndState { get; set; }

        public QState(Action CurAction)
        {
            StartState = new State();
            CurrentAction = CurAction;
            Reward = 0;
            EndState = new State();
        }

        public void UtilityAnalyze(GameObject player, GameObject npc)
        {
            float PlayerSufferDamage = player.GetComponent<Player>().DamageTaken;
            float NormalizedPlayerSufferDamage = PlayerSufferDamage /
                (player.GetComponent<Player>().HP + PlayerSufferDamage);

            float EnemySufferDamage = npc.GetComponent<Monster>().DamageTaken;
            float NormalizedEnemySufferDamage = EnemySufferDamage /
                (npc.GetComponent<Monster>().HP + EnemySufferDamage);

            Reward = NormalizedPlayerSufferDamage - NormalizedEnemySufferDamage;

            player.GetComponent<Player>().DamageTaken = 0;
            npc.GetComponent<Monster>().DamageTaken = 0;
        }
    }

    //okreslenie akcji
    class StateActionValue
    {
        public Dictionary<Action, float> ActionValue;
        public Dictionary<Action, int> ActionCount;
        public Action MaxAction;
        public float MaxValue;
        public int TotalActionCount;

        public StateActionValue()
        {
            ActionValue = new Dictionary<Action, float>();
            ActionCount = new Dictionary<Action, int>();
            MaxAction = Action.None;
            MaxValue = 0;
            TotalActionCount = 0;

            List<Action> StateAction = new List<Action>() {
            Action.FireBall,
            Action.Attack
        };
            foreach (Action action in StateAction)
            {
                ActionValue.Add(action, 0);
                ActionCount.Add(action, 0);
            }
        }
    }

    public class KnowledgeSystem
    {

        private Dictionary<State, StateActionValue> Experience;

        //określa, w jakim stopniu nowo nabyte informacje zastąpią stare informacje
        private float LeaningRate;

        //współczynnik dyskontowania [0,1] reguluje względną ważność krótko- i długoterminowych nagród. paczaj wyklad 12.
        private float DiscountRate;

        //klucz: wartosc, akcja: stan
        private Dictionary<Action, StateID> ActionIDToStateID;

        //Służy do "transportu" stanu. jeśli true wrzuć stan do strategii
        private Dictionary<State, bool> StateLearningSwitch;

        public KnowledgeSystem()
        {
            Experience = new Dictionary<State, StateActionValue>();
            StateLearningSwitch = new Dictionary<State, bool>();
            LeaningRate = 0.1f;
            DiscountRate = 0.5f;
            ActionIDToStateID = new Dictionary<Action, StateID>() {
            { Action.Attack, StateID.BattleAttack },
            { Action.FireBall, StateID.BattleFireball },
            { Action.Chasing, StateID.BattleRun }

        };

        }

        public bool GetStateLearningInfo(State CurState)
        {
            if (StateLearningSwitch.ContainsKey(CurState))
                return StateLearningSwitch[CurState];
            else
            {
                StateLearningSwitch.Add(CurState, false);
                return false;
            }
        }

        public float GetStateActionValue(State CurState, Action CurAction)
        {
            if (Experience.ContainsKey(CurState))
                return Experience[CurState].ActionValue[CurAction];
            else
                return 0.0f;

        }

        // STRATEGIA wybierania  akcji zapewniającej największą wygraną w danym stanie. UCB1. Eliminacja przypadkowości.
        public StateID UCBPolicy(State CurState)
        {
            Action MaxUCBAction = Action.None;
            float MaxUCBValue = 0;
            float TempUCBValue = 0;
            foreach (var item in Experience[CurState].ActionValue)
            {
                if (Experience[CurState].ActionCount[item.Key] == 0)
                    return ActionIDToStateID[item.Key];
                else
                {
                    //
                    TempUCBValue = item.Value + Mathf.Sqrt(0.1f * Mathf.Log(Experience[CurState].TotalActionCount) /
                        Experience[CurState].ActionCount[item.Key]);
                    if (TempUCBValue > MaxUCBValue)
                    {
                        MaxUCBValue = TempUCBValue;
                        MaxUCBAction = item.Key;
                    }
                }

            }

            StateLearningSwitch[CurState] = true;
            return ActionIDToStateID[MaxUCBAction];

        }
        // uczenie na podstawie doświadczenia, Q-LEARNING how its work: tabela 2 wykład 13.(podstawa: algorytm TD)
        public void UpdateKnowledge(QState CurQState)
        {
            State CurState = CurQState.StartState;
            Action CurAction = CurQState.CurrentAction;
            State NextState = CurQState.EndState;
            float Reward = CurQState.Reward;

            if (Experience.ContainsKey(CurState) == false)
                Experience[CurState] = new StateActionValue();
            Experience[CurState].TotalActionCount += 1;

            if (Experience.ContainsKey(NextState))
                Experience[CurState].ActionValue[CurAction] += LeaningRate *
                    (Reward + DiscountRate * Experience[NextState].MaxValue - Experience[CurState].ActionValue[CurAction]);
            else
                Experience[CurState].ActionValue[CurAction] += LeaningRate *
                    (Reward - Experience[CurState].ActionValue[CurAction]);
            Experience[CurState].ActionCount[CurAction] += 1;

            //wypisanie w logu wsyzstkich danych pod koniec akcji
            Debug.Log("Action: " + CurQState.CurrentAction.ToString() + "State: (" + CurState.PlayerNpcDistance.ToString() +
                "," + CurState.PlayerNpcEulerAngle.ToString() + ")"
                + " Reward: " + Reward.ToString() + " Utility: " + Experience[CurState].ActionValue[CurAction].ToString() +
                " ActionCount: " + Experience[CurState].ActionCount[CurAction].ToString());

            if (Experience[CurState].ActionValue[CurAction] > Experience[CurState].MaxValue ||
                Experience[CurState].MaxAction == Action.None)
            {
                Experience[CurState].MaxAction = CurAction;
                Experience[CurState].MaxValue = Experience[CurState].ActionValue[CurAction];
            }

        }
    }
}
