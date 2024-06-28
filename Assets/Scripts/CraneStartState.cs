using UnityEngine;

public class CraneStartState : MonoBehaviour, ICraneState
{
    private CraneController2 _craneController;

    public void Handle(CraneController2 craneController)
    {
        if (!_craneController)
            _craneController = craneController;
        _craneController.CurrentSpeed = _craneController.maxSpeed;
    }
}


/*
 * 상태 패턴의 2가지 한계는 유니티 애니메이션 시스템과 기본 상태 기계로 극복할 수 있다. 애니메이션 상태를 쉽게 정의할 수 있으며 설정한 상태에 애니메이션 클립과 스크립트를 각각 붙일 수 있다.
 * 
 * 대안 살펴보기
 * 블랙보드/행동 트리: NPC 캐릭터의 복잡한 AI 동작을 구현하려고 한다면 블랙보드 같은 패턴 혹은 행동 트리(Behavior Tree, BT)같은 개념을 고려하자. 동적으로 결정하는 동작을 하는 AI를 구현한다면 행동 트리를 사용하여 행동을 구현하는 BT가 적절하다.
 * 유한 기계 상태: 상태 패턴은 객체의 상태 종속적인 동작을 캡슐화하는 것과 관련이 있다는 점이 유한 상태 기계와 다르다. 유한 상태 기계는 특정 입력 트리거를 기반으로 하는 유한 상태 간 전환에 더 깊이 관여한다. 자동 기계 같은 시스템을 구현하는 데 더 적합하다.
 * 메멘토: 메멘토는 상태 패턴과 비슷하지만 개체에 이전 상태로 돌아가는 기능을 제공한다. 자체적으로 변경된 것으로 되돌리는 기능이 필요한 시스템을 구현할 때 유용하다.
 * 
 */