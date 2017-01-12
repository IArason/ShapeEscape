using System;
using System.Linq;
using System.Collections.Generic;

public static class EditorLevelEditorUtils
{
    public static List<string> ContextMenuOptionTypes
    {
        get
        {
            if(contextMenuOptionTypes == null)
            {
                contextMenuOptionTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                 from assemblyType in domainAssembly.GetTypes()
                 where typeof(ContextMenuOption).IsAssignableFrom(assemblyType)
                 select assemblyType).Select(x => x.ToString()).Where(x => x != "ContextMenuOption").ToList();
            }
            return contextMenuOptionTypes;
        }
    }
    static List<string> contextMenuOptionTypes;
}
