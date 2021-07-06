using System.Collections;
using System.Collections.Generic;
using AV.Inspector.Runtime;
using AV.UITK;
using UnityEngine;
using UnityEngine.UIElements;

namespace AV.Inspector
{
    public class InspectorPatchesPanel : VisualElement
    {
        static InspectorPrefs prefs = InspectorPrefs.Loaded;
        
        public InspectorPatchesPanel()
        {
            var me = this.Fluent();

            var importantBox = me.NewHelpBox("Disabling patches will break the functionality, use this only for debugging purposes.", HelpMessageType.Important);
            importantBox.x.content.icon.size = 16;
            me.Add(importantBox);

            var toolsIcon = FluentUITK.GetEditorIcon("SceneViewTools");

            var group = me.NewHeader("Patches", toolsIcon).Style(Styles.Separator);
            me.Add(group);
            
            var bar = me.NewRow().Add(
                me.NewFlexibleSpace(),
                me.NewButton("Enable All").OnClick(_ => AllToggles().ForEach(e => e.value = true)),
                me.NewButton("Disable All").OnClick(_ => AllToggles().ForEach(e => e.value = false))
            );
            group.x.content.Add(bar);

            UQueryBuilder<Toggle> AllToggles()
            {
                return group.x.Query<Toggle>();
            }
            
            foreach (var patch in Patcher.GetPatches())
            {
                var text = patch.GetType().Name;

                var toggle = new Toggle(text).Fluent();
                
                toggle.x.value = !patch.forceSkip;
                toggle.OnChange<bool>((evt, c) =>
                {
                    patch.forceSkip = !evt.newValue;

                    if (!prefs.enabled)
                        return;
                    
                    if (patch.forceSkip)
                    {
                        patch.UnpatchAll();
                    }
                    else
                    {
                        patch.ApplyPatches(PatchBase.Apply.OnLoad);
                        patch.ApplyPatches(PatchBase.Apply.OnGUI);
                    }
                });
                
                group.Add(toggle);
            }    
        }
    }
}
