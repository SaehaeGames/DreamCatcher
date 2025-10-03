using UnityEngine;

public abstract class QuestActionBase : MonoBehaviour
{
    public abstract void Enter();
    public abstract void Execute(QuestActionPipeline bundle);
    public abstract void Exit();
}
