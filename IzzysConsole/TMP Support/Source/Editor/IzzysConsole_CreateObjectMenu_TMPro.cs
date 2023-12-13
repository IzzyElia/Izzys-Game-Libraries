using UnityEditor;
using UnityEngine;
using IzzysConsole.TMPro;
using IzzysConsole.Utils;

namespace IzzysConsole.Editor.TMPro
{
    public static class IzzysConsole_CreateObjectMenu_TMPro
    {
        const string TMProConsoleName = "Debugging Console (TMPro)";
        [MenuItem("GameObject/UI/Debugging Consoles/" + TMProConsoleName)]
        static void CreateLegacyConsole(MenuCommand menuCommand)
        {
            GameObject selectedObject = menuCommand.context as GameObject;
            Canvas canvas = UIUtils.GetOrCreateCanvasInContext(selectedObject);
            RectTransform targetParentTransform = selectedObject?.GetComponent<RectTransform>() != null
                ? selectedObject.transform as RectTransform
                : canvas.transform as RectTransform;
            UIConsoleController_TMPro consoleController = ConsoleFactory_TMPro.Generate(targetParentTransform);
            consoleController.GetComponent<RectTransform>().SetAnchorsAndMargins(new Vector4(0.6f, 0.06f, 0.94f, 0.94f), Vector4.zero);
            Undo.RegisterCreatedObjectUndo(consoleController.gameObject, $"Create {TMProConsoleName}");
        }
    }
}