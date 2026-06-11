using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DreamCatcherThumbnailRenderer : MonoBehaviour
{
    [SerializeField] private Camera captureCamera;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private DreamCatcherPreview previewRenderer;

    public string SaveThumbnail(DreamCatcher dreamCatcher)
    {
        Texture2D tex = RenderThumbnail(dreamCatcher);

        Debug.Log(GetThumbnailPath(dreamCatcher.GetId()));
        string path = GetThumbnailPath(dreamCatcher.GetId());

        SaveTexture(tex, path);

        Destroy(tex);

        return path;
    }

    private Texture2D RenderThumbnail(DreamCatcher dreamCatcher)
    {
        previewRenderer.MakeDreamCatcherImg(dreamCatcher);

        RenderTexture currentRT = RenderTexture.active;

        captureCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        captureCamera.Render();

        Texture2D tex = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBA32,
            false);

        tex.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height),
            0,
            0);

        tex.Apply();

        RenderTexture.active = currentRT;

        return tex;
    }

    private void SaveTexture(Texture2D tex, string path)
    {
        string directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(path, tex.EncodeToPNG());
    }

    public string GetThumbnailPath(string id)
    {
        return GetThumbnailPathStatic(id);
    }

    public static string GetThumbnailPathStatic(string id)
    {
        return Path.Combine(
            Application.persistentDataPath,
            "DreamCatcherThumbnails",
            $"DreamCatcher_{id}.png");
    }
}
