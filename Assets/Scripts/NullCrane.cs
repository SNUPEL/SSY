
/// <summary>
/// Crane 정보가 없는 DiscreteEvent를 처리하기 위해 필요한 널 형식의 크레인 클래스
/// </summary>
public class NullCrane : Crane
{
    public override void move(float spent, float delta)
    {

    }

    public void OnGUI()
    {
        
    }
}