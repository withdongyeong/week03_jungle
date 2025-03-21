using UnityEngine;
using UnityEngine.Pool;

//������Ʈ Ǯ���� ����ϰ� ���� ������Ʈ�� �� �������̽��� ��ӹ޾ƾ��մϴ�
public interface IPoolable 
{
    //���� ��ӹ޴´ٸ� �̰͵� �����ؾ��մϴ�(�ڵ����� ������Ƽ�� ����)
    public IObjectPool<GameObject> pool { get; set; }

    //�����ϰ� pool.release(gameobject)�� �ص� �˴ϴ�.
    public void ReleaseObject();

}
