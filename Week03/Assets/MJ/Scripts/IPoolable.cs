using UnityEngine;
using UnityEngine.Pool;

//오브젝트 풀링을 사용하고 싶은 오브젝트는 이 인터페이스를 상속받아야합니다
public interface IPoolable 
{
    //만약 상속받는다면 이것도 구현해야합니다(자동구현 프로퍼티도 가능)
    public IObjectPool<GameObject> pool { get; set; }

    //간단하게 pool.release(gameobject)만 해도 됩니다.
    public void ReleaseObject();

}
