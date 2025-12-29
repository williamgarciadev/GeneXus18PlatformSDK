using Artech.Common.Framework.Commands;

namespace Acme.Packages.Menu
{
	public class CommandKeys
	{


        private static CommandKey cmdGenerateLogDebugForm = new CommandKey(MenuPackage.guid, "CmdGenerateLogDebugForm");
        public static CommandKey WGExtractProcedure =  new CommandKey(MenuPackage.guid, "WGExtractProcedure");
        public static CommandKey WGExtractVariable = new CommandKey(MenuPackage.guid, "WGExtractVariable");
        public static CommandKey CmdExportTableStructure = new CommandKey(MenuPackage.guid, "CmdExportTableStructure");
        public static CommandKey ShowObjectHistory = new CommandKey(MenuPackage.guid, "ShowObjectHistory");
        public static CommandKey CmdExportProcedureSource = new CommandKey(MenuPackage.guid, "CmdExportProcedureSource");
        public static CommandKey CmdCountCodeLines = new CommandKey(MenuPackage.guid, "CmdCountCodeLines");
        public static CommandKey CmdExportObjectsWithSourceLines = new CommandKey(MenuPackage.guid, "CmdExportObjectsWithSourceLines");
        public static CommandKey CmdGenerateMarkdownDocs = new CommandKey(MenuPackage.guid, "CmdGenerateMarkdownDocs");
        public static CommandKey CmdCleanUnusedVariables = new CommandKey(MenuPackage.guid, "CmdCleanUnusedVariables");
        public static CommandKey CmdSmartFixVariables = new CommandKey(MenuPackage.guid, "CmdSmartFixVariables");
        public static CommandKey CmdTraceVariable = new CommandKey(MenuPackage.guid, "CmdTraceVariable");
        public static CommandKey CmdGoToSubroutine = new CommandKey(MenuPackage.guid, "CmdGoToSubroutine");
        public static CommandKey CmdBackFromGoTo = new CommandKey(MenuPackage.guid, "CmdBackFromGoTo");
        public static CommandKey CmdFindUnreferencedObjects = new CommandKey(MenuPackage.guid, "CmdFindUnreferencedObjects");
        public static CommandKey CmdInventoryExternalObjects = new CommandKey(MenuPackage.guid, "CmdInventoryExternalObjects");
        public static CommandKey CmdListWebPanelFormClass = new CommandKey(MenuPackage.guid, "CmdListWebPanelFormClass");




        public static CommandKey CmdGenerateLogDebugForm => cmdGenerateLogDebugForm;




    }
}
