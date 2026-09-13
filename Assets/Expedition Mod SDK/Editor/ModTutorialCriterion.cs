using System;
using UnityEditor;
using Unity.Tutorials.Editor;

namespace Expedition.ModSdk.Editor
{
    public sealed class ModTutorialCriterion : Criterion
    {
        public int Step;
        public override void StartTesting() { base.StartTesting(); EditorApplication.update += UpdateCompletion; }
        public override void StopTesting() { EditorApplication.update -= UpdateCompletion; base.StopTesting(); }
        protected override bool EvaluateCompletion() => Step switch
        {
            0 => EditorWindow.HasOpenInstances<ModWorkspaceWindow>(),
            1 => string.Equals(ModWorkspaceWindow.LastValidatedRoot, ModSdkPaths.SelectedModRoot, StringComparison.Ordinal),
            2 => string.Equals(ModWorkspaceWindow.LastBuiltRoot, ModSdkPaths.SelectedModRoot, StringComparison.Ordinal),
            3 => string.Equals(ModWorkspaceWindow.LastInstalledRoot, ModSdkPaths.SelectedModRoot, StringComparison.Ordinal),
            _ => false
        };
        public override bool AutoComplete() => EvaluateCompletion();
    }
}
