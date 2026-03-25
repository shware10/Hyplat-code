using UnityEngine;

// 제네릭 싱글톤 클래스
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 실제 인스턴스를 저장용 static 변수
    private static T _instance;

    // 외부에서 접근하는 싱글톤 프로퍼티
    public static T Instance
    {
        get
        {
            // 아직 인스턴스가 없다면
            if(_instance == null)
            {
                // 씬에 이미 존재하는 객체를 먼저 찾음
                var t = FindFirstObjectByType<T>();

                // 찾았다면 그걸 instance로 사용
                if (t != null) _instance = t;

                else
                {
                    // 없다면 새 GameObject를 생성해서 붙임
                    var newObj = new GameObject(typeof(T).Name);

                    // T 컴포넌트 추가
                    newObj.AddComponent<T>();

                    // 생성된 컴포넌트를 instance로 설정
                    _instance = newObj.GetComponent<T>();
                }
            }

            // instance 반환
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 아직 instance가 없다면
        if (_instance == null)
        {
            // 현재 객체를 instance로 등록
            _instance = this as T;

            // 씬이 바뀌어도 삭제되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 instance가 존재하면 중복 생성된 것이므로 제거
            Destroy(gameObject);
        }
    }
}