using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CallbackOnEnd : StateMachineBehaviour {

    ContextMenu menu;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(menu == null)
            menu = FindObjectOfType<ContextMenu>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    if(stateInfo.normalizedTime >= 1 && menu != null)
        {
            menu.OnMenuReady();
        }
	}
}
