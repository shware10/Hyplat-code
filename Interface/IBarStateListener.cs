using UnityEngine;

// 바의 적중/미적중을 리스닝 하는 인터페이스
public interface IBarStateListener
{
    void Fit();
    void Unfit();
}
