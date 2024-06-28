public class CraneStateContext
{
    // ������ ���¸� ��Ÿ����.
    public ICraneState CurrentState { get; set; }

    private readonly CraneController2 _craneController;

    /// <summary>
    /// ũ������ ���� ���¸� ����Ű�� public �Ӽ��� �����Ͽ� ��� ���� ������ �ν��Ѵ�.
    /// ��ü �Ӽ��� ���� ���� ���¸� ������Ʈ�ϰ� Transition() �Լ��� ȣ���Ͽ� ������Ʈ�� ���·� ��ȯ�� �� �ִ�.
    /// ���� ��� �� ���� Ŭ������ ���� ���� Ŭ������ �����ؼ� ���¸� �Բ� ������ ü���� ����� ��쿡 ������ ��Ŀ�����̴�. 
    /// </summary>
    /// <param name="craneController"></param>
    public CraneStateContext(CraneController2 craneController)
    {
        this._craneController = craneController;
    }

    /// <summary>
    /// �������� Context ������Ʈ�� Transition() �Լ��� ȣ���Ͽ� ����� ���¸� ��ȯ�� �� �ִ�. �ݵ�� �ʿ��� ����� �ƴϹǷ� �����ε��� Transition() �Լ��� ȣ���ϰ� ��ȯ�ϰ� ���� ���¸� ������ ���̴�.
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