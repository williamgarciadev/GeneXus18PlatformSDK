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




        public static CommandKey CmdGenerateLogDebugForm => cmdGenerateLogDebugForm;




    }
}
