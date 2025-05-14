using UnityEngine;

public static class FindObject
{
    // GameObject�� ��η� ã��
    public static GameObject Find(string path)
    {
        return GameObject.Find(path);
    }

    // Ư�� ������Ʈ�� ��η� ã��
    public static T Find<T>(string path) where T : Component
    {
        var go = GameObject.Find(path);
        return go ? go.GetComponent<T>() : null;
    }

    // Ư�� Tag�� ������Ʈ ã�� (�ɼ�)
    public static GameObject FindWithTag(string tag)
    {
        return GameObject.FindWithTag(tag);
    }

    // Tag�� ������Ʈ ã�� (�ɼ�)
    public static T FindWithTag<T>(string tag) where T : Component
    {
        var go = GameObject.FindWithTag(tag);
        return go ? go.GetComponent<T>() : null;
    }
}
