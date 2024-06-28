using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CraneController2 : MonoBehaviour
{
    public float maxSpeed = 2.0f;
    public float turnDistance = 2.0f;

    public float CurrentSpeed { get; set; }
    public Direction CurrentTurnDirection { get; private set; }

    private ICraneState _startState, _stopState, _turnState;

    /// <summary>
    /// 크레인의 현재 상태를 나타내는 변수임
    /// </summary>
    private CraneStateContext _craneStateContext;

    /// <summary>
    /// 크레인 상태 변수,
    /// start, stop, turn 상태,
    /// 초기 상태는 stop으로 전환한다.
    /// </summary>
    private void Start()
    {
        _craneStateContext = new CraneStateContext(this);

        _startState = gameObject.AddComponent<CraneStartState>();
        _stopState = gameObject.AddComponent<CraneStopState>();
        _turnState = gameObject.AddComponent<CraneTurnState>();

        _craneStateContext.Transition(_stopState);
    }

    /// <summary>
    /// start 상태로 전환한다.
    /// </summary>
    public void StartCrane()
    {
        _craneStateContext.Transition(_startState);
    }

    /// <summary>
    /// stop 상태로 전환한다.
    /// </summary>
    public void StopCrane()
    {
        _craneStateContext.Transition(_stopState);
    }
    
    /// <summary>
    /// Turn 상태로 전환한다. 이 때 방향 또한 direction으로 바꾼다.
    /// </summary>
    /// <param name="direction">변경할 방향</param>
    public void Turn(Direction direction)
    {
        CurrentTurnDirection = direction;
        _craneStateContext.Transition(_turnState);
    }
    /*
     * 상태 패턴은 클래스를 간소화하고 유지 및 관리가 쉽도록 만든다. 그리고 오토바이의 핵심 컴포넌트를 관리하는 책임을 BikeController에 돌려준다. 이는 오토바이를 제어하고 설정할 수 있는 속성을 노출하고 구조적 종속성을 관리하기 위한 인터페이스를 제공하기 위함이다.
     * 
     * 지금부터 세 가지 상태 클래스를 살펴본다. 각 클래스는 IBikeState 인터페이스를 구현한다는 점을 명심하면서 BikeStopState부터 알아보자.
     */
}