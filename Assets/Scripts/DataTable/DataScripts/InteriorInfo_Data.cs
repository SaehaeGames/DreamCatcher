using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기존 InteriorInfo 자산을 읽기 위한 보관용 형식입니다.
/// 현재 인테리어는 StoreInfo와 Inspector의 상품·화면 연결을 사용하므로 실행 및 테이블 갱신 대상에서 제외합니다.
/// 자산의 스크립트 GUID와 기존 데이터 구조를 유지하기 위해 타입과 필드만 보존합니다.
/// </summary>
public class InteriorInfo_Data : ScriptableObject
{
    public List<InteriorInfo_Object> dataList = new List<InteriorInfo_Object>();
}
