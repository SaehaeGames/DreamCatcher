using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveSequencePipeline : MonoBehaviour
{
    public abstract void MoveToNextSequence();

    public virtual void SetSceneState(SceneState sceneState)
    {
    }
}
