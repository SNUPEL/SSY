public class CraneStateContext
{
    // 현재의 상태를 나타낸다.
    public ICraneState CurrentState { get; set; }

    private readonly CraneController2 _craneController;

    /// <summary>
    /// 크레인의 현재 상태를 가리키는 public 속성을 노출하여 모든 상태 변경을 인식한다.
    /// 개체 속성을 통해 현재 상태를 업데이트하고 Transition() 함수를 호출하여 업데이트한 상태로 전환할 수 있다.
    /// 예를 들면 각 상태 클래스가 다음 상태 클래스를 선언해서 상태를 함께 연결해 체인을 만드는 경우에 유용한 메커니즘이다. 
    /// </summary>
    /// <param name="craneController"></param>
    public CraneStateContext(CraneController2 craneController)
    {
        this._craneController = craneController;
    }

    /// <summary>
    /// 다음으로 Context 오브젝트의 Transition() 함수를 호출하여 연결된 상태를 순환할 수 있다. 반드시 필요한 방식은 아니므로 오버로드한 Transition() 함수를 호출하고 전환하고 싶은 상태를 전달할 것이다.
    /// </summary>
    public void Transition()
    {
        CurrentState.Handle(_craneController);
    }

    public void Transition(ICraneState state)
    {
        CurrentState = state;
        CurrentState.Handle(_craneController);
    }
}