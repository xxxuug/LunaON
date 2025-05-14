using UnityEngine;

public static class FindObject
{
    // GameObject를 경로로 찾기
    public static GameObject Find(string path)
    {
        return GameObject.Find(path);
    }

    // 특정 컴포넌트를 경로로 찾기
    public static T Find<T>(string path) where T : Component
    {
        var go = GameObject.Find(path);
        return go ? go.GetComponent<T>() : null;
    }

    // 특정 Tag로 오브젝트 찾기 (옵션)
    public static GameObject FindWithTag(string tag)
    {
        return GameObject.FindWithTag(tag);
    }

    // Tag로 컴포넌트 찾기 (옵션)
    public static T FindWithTag<T>(string tag) where T : Component
    {
        var go = GameObject.FindWithTag(tag);
        return go ? go.GetComponent<T>() : null;
    }
}
