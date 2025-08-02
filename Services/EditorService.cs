using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Framework.Commands;
using System;
using System.Windows.Forms;

namespace Acme.Packages.Menu
{
    public static class EditorService
    {
        public static void PasteTextInEditor(string text)
        {
            try
            {
                Clipboard.SetText(text);
                UIServices.CommandDispatcher.Dispatch(new CommandKey(new Guid("98121D96-A7D8-468b-9310-B1F468F812AE"), "Paste"));
                LogService.ShowInfo("Texto pegado en el editor correctamente.");
            }
            catch (Exception ex)
            {
                LogService.ShowError($"Error al pegar el texto en el editor: {ex.Message}");
            }
        }

        public static string GetClipboardText()
        {
            return Clipboard.ContainsText() ? Clipboard.GetText().Trim() : string.Empty;
        }
    }
}
