using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UndoSelection : UndoAction
{
    LevelEntity[] oldSelection;
    LevelEntity[] newSelection;

    // Reference to the manipulator to be updated
    ObjectManipulator manipulator;

    public UndoSelection(ObjectManipulator manipulator, LevelEntity[] oldSelection, LevelEntity[] newSelection)
    {
        this.manipulator = manipulator;
        this.oldSelection = oldSelection.Clone() as LevelEntity[];
        this.newSelection = newSelection.Clone() as LevelEntity[];
        
        Validate();
    }

    public UndoSelection(ObjectManipulator manipulator, LevelEntity[] oldSelection, List<LevelEntity> newSelection)
    {
        this.manipulator = manipulator;
        this.oldSelection = oldSelection.Clone() as LevelEntity[];
        this.newSelection = newSelection.ToArray();
        
        Validate();
    }

    public UndoSelection(ObjectManipulator manipulator, List<LevelEntity> oldSelection, List<LevelEntity> newSelection)
    {
        this.manipulator = manipulator;
        this.oldSelection = oldSelection.ToArray();
        this.newSelection = newSelection.ToArray();

        Validate();
    }

    void Validate()
    {
        // If both are empty or both selections are identical, we ignore it.
        if ((oldSelection.Length == 0 && newSelection.Length == 0) || (oldSelection.SequenceEqual(newSelection))) return;

        // add self to manager.
        UndoManager.Instance.AddAction(this);
    }


    public override void Redo()
    {
        manipulator.SetSelection(newSelection.ToList(), true);
    }

    public override void Undo()
    {
        manipulator.SetSelection(oldSelection.ToList(), true);
    }
    
}
