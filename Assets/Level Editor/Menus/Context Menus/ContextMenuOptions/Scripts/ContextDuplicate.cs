using System.Linq;

public class ContextDuplicate : ContextMenuOption
{
    /// <summary>
    /// Begins a process to link this triggerable object to a trigger.
    /// </summary>
    public override void OnSelect()
    {
        base.OnSelect();
        ObjectManipulator.Instance.SetSelection(
            owners.Select(x => x.GetComponent<LevelEntity>()).ToList());
        ObjectManipulator.Instance.DuplicateSelection();
    }
}