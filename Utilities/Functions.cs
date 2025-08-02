using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Services;
using Artech.Common.Helpers.Structure;
using Artech.Genexus.Common;
using Artech.Genexus.Common.Objects;
using Artech.Genexus.Common.Parts;
using Artech.Genexus.Common.Parts.SDT;
using Artech.Udm.Framework;
using Artech.Udm.Framework.References;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Acme.Packages.Menu
{
    internal static class Functions
    {
        public static int MaxCodeBlock(string source)
        {
            int num1 = 0;
            int num2 = 0;
            using (StringReader stringReader = new StringReader(source))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    ++num2;
                    if (str.StartsWith("SUB ") || str.StartsWith("EVENT "))
                    {
                        num1 = num1 <= num2 ? num2 : num1;
                        num2 = 1;
                    }
                }
                num1 = num1 <= num2 ? num2 : num1;
            }
            return num1;
        }

        public static int ComplexityLevel(string source)
        {
            int num = 0;
            using (StringReader stringReader = new StringReader(source))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    string upper = str.TrimStart().ToUpper();
                    if (upper.StartsWith("DO WHILE") || upper.StartsWith("IF") || upper.StartsWith("DO CASE") || upper.StartsWith("FOR"))
                        ++num;
                }
            }
            return num;
        }

        public static int MaxNestLevel(string source)
        {
            int num1 = 0;
            int num2 = 0;
            using (StringReader stringReader = new StringReader(source))
            {
                string str;
                while ((str = stringReader.ReadLine()) != null)
                {
                    string upper = str.TrimStart().ToUpper();
                    if (!upper.StartsWith("DO '"))
                    {
                        if (upper.StartsWith("FOR ") || upper.StartsWith("IF ") || upper.StartsWith("DO ") || upper.StartsWith("NEW") || upper.StartsWith("SUB"))
                        {
                            ++num2;
                            num1 = num2 > num1 ? num2 : num1;
                        }
                        else if (upper.StartsWith("ENDFOR") || upper.StartsWith("ENDIF") || upper.StartsWith("ENDDO") || upper.StartsWith("ENDCASE") || upper.StartsWith("ENDNEW") || upper.StartsWith("ENDSUB"))
                            --num2;
                    }
                }
                return num1;
            }
        }

        public static bool ValidateINOUTinParm(KBObject obj)
        {
            bool flag1 = false;
            if (obj is ICallableObject callableObject)
            {
                foreach (Artech.Genexus.Common.Objects.Signature signature in callableObject.GetSignatures())
                {
                    bool flag2 = false;
                    foreach (Parameter parameter in signature.Parameters)
                    {
                        if (parameter.Accessor.ToString() == "PARM_INOUT")
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        RulesPart rulesPart = obj.Parts.Get<RulesPart>();
                        if ((KBPart<KBObject>)rulesPart != (KBPart<KBObject>)null)
                        {
                            Regex regex1 = new Regex("//.*", RegexOptions.None);
                            Regex regex2 = new Regex("parm\\(.*\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                            string source = rulesPart.Source;
                            string input = regex1.Replace(source, "");
                            Match match = regex2.Match(input);
                            if (match != null)
                                flag1 = match.ToString().Split(',').Length != match.ToString().Split(':').Length - 1;
                        }
                    }
                }
            }
            return flag1;
        }

        internal static void AddLineSummary(string fileName, string texto)
        {
            using (FileStream fileStream = new FileStream(UIServices.KB.CurrentKB.UserDirectory + "\\" + fileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                    streamWriter.WriteLine(DateTime.Now.ToString() + "," + texto);
            }
        }

        internal static void AddLine(string fileName, string texto)
        {
            using (FileStream fileStream = new FileStream(UIServices.KB.CurrentKB.UserDirectory + "\\" + fileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                    streamWriter.WriteLine(texto);
            }
        }

        public static string ObjectSourceUpper(KBObject obj)
        {
            string str = "";
            try
            {
                if (obj is Procedure)
                    str = obj.Parts.Get<ProcedurePart>().Source;
                if (obj is Artech.Genexus.Common.Objects.Transaction)
                    str = obj.Parts.Get<EventsPart>().Source;
                if (obj is WorkPanel)
                    str = obj.Parts.Get<EventsPart>().Source;
                if (obj is WebPanel)
                    str = obj.Parts.Get<EventsPart>().Source;
            }
            catch (Exception)
            {
                str = "";
            }
            return str.ToUpper();
        }

        public static bool isRunable(KBObject obj)
        {
            int num;
            switch (obj)
            {
                case Artech.Genexus.Common.Objects.Transaction _:
                case WorkPanel _:
                case WebPanel _:
                case DataProvider _:
                case DataSelector _:
                case Procedure _:
                    num = 1;
                    break;
                default:
                    num = obj is Menubar ? 1 : 0;
                    break;
            }
            return num != 0;
        }

        public static bool CanBeBuilt(KBObject obj)
        {
            int num;
            switch (obj)
            {
                case Artech.Genexus.Common.Objects.Transaction _:
                case WebPanel _:
                case Procedure _:
                case DataProvider _:
                    num = 1;
                    break;
                default:
                    num = obj is Menubar ? 1 : 0;
                    break;
            }
            return num != 0;
        }

        public static string ExtractComments(string source)
        {
            string str1 = "/\\*(.*?)\\*/";
            string str2 = "//(.*?)\\r?\\n";
            string str3 = "\"((\\\\[^\\n]|[^\"\\n])*)\"";
            string str4 = "@(\"[^\"]*\")+";
            string str5 = Regex.Replace(source, str1 + "|" + str2 + "|" + str3 + "|" + str4, (MatchEvaluator)(me => me.Value.StartsWith("/*") || me.Value.StartsWith("//") ? (me.Value.StartsWith("//") ? Environment.NewLine : "") : me.Value), RegexOptions.Singleline).Replace("(", " (").Replace(")", ") ").Replace("\"", "'").Replace("\t", " ").Replace("  ", " ");
            string comments;
            do
            {
                comments = str5;
                str5 = comments.Replace("  ", " ");
            }
            while (comments != str5);
            return comments;
        }

        public static string CodeCommented(string source)
        {
            string pattern = "[^\\/](\\/\\*)([\\b\\s]*(msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=))(\\*(?!\\/)|[^*])*(\\*\\/)|(\\/\\/)[\\b\\s]*((msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=)([^\\r\\n]+)?)";
            return Regex.Match(source, pattern).Value;
        }

        public static bool HasCodeCommented(string source)
        {
            string pattern = "[^\\/](\\/\\*)([\\b\\s]*(msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=))(\\*(?!\\/)|[^*])*(\\*\\/)|(\\/\\/)[\\b\\s]*((msg|do|call|udp|where|if|else|endif|endfor|for|defined by|while|enddo|&[A-Za-z0-9_\\-.\\s]*=)([^\\r\\n]+)?)";
            return Regex.Match(source, pattern).Value != "";
        }

        public static Domain DomainByName(string domainName)
        {
            foreach (Domain domain in Domain.GetAll(UIServices.KB.CurrentModel))
            {
                if (domain.Name == domainName)
                    return domain;
            }
            return (Domain)null;
        }

        public static string RemoveEmptyLines(string lines)
        {
            return Regex.Replace(lines, "^\\s*$\\n|\\r", "", RegexOptions.Multiline);
        }

        public static int LineCount(string s)
        {
            int num = 0;
            foreach (char ch in s)
            {
                if (ch == '\n')
                    ++num;
            }
            return num;
        }

        public static string linkObject(KBObject obj)
        {
            return "<a href=\"gx://?Command=fa2c542d-cd46-4df2-9317-bd5899a536eb;OpenObject&name=" + obj.Guid.ToString() + "\">" + obj.Name + "</a>";
        }

        public static string linkFile(string file)
        {
            return "<a href=\"file:///" + file + "\">" + file + "</a>";
        }

        public static string ExtractRuleParm(KBObject obj)
        {
            RulesPart rulesPart = obj.Parts.Get<RulesPart>();
            string ruleParm = "";
            if ((KBPart<KBObject>)rulesPart != (KBPart<KBObject>)null)
            {
                Regex regex1 = new Regex("//.*", RegexOptions.None);
                Regex regex2 = new Regex("parm\\(.*\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                string source = rulesPart.Source;
                string input = regex1.Replace(source, "");
                Match match = regex2.Match(input);
                ruleParm = match == null ? "" : match.ToString();
            }
            return ruleParm;
        }

        public static string CleanFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static string CreateOutputFile(IKBService kbserv, string title)
        {
            string path = kbserv.CurrentKB.UserDirectory + "\\kbdoctor." + Functions.CleanFileName(title) + ".html";
            if (File.Exists(path))
                File.Delete(path);
            return path;
        }

        public static bool AttIsSubtype(Artech.Genexus.Common.Objects.Attribute a)
        {
            return a.SuperTypeKey != (EntityKey)null;
        }

        public static void KillAttribute(Artech.Genexus.Common.Objects.Attribute a)
        {
            IOutputService output = CommonServices.Output;
            foreach (EntityReference entityReference in a.GetReferencesTo())
            {
                KBObject objRef = KBObject.Get(a.Model, entityReference.From);
                if (objRef != (KBObject)null)
                {
                    Functions.CleanVariablesBasedInAttribute(a, output, objRef);
                    Functions.CleanSDT(a, output, objRef);
                    if (!(objRef is DataView))
                    {
                        try
                        {
                            objRef.Save();
                        }
                        catch (Exception ex)
                        {
                            output.AddErrorLine("ERROR: Can't save object: " + objRef.Name + ex.Message);
                        }
                    }
                }
            }
        }

        private static void CleanVariablesBasedInAttribute(
          Artech.Genexus.Common.Objects.Attribute a,
          IOutputService output,
          KBObject objRef)
        {
            output.AddLine("Cleaning variables references to " + a.Name + " in " + objRef.Name);
            VariablesPart variablesPart = objRef.Parts.Get<VariablesPart>();
            if (!((KBPart<KBObject>)variablesPart != (KBPart<KBObject>)null))
                return;
            foreach (Variable variable in variablesPart.Variables)
            {
                if (!variable.IsStandard && (KBObject)variable.AttributeBasedOn != (KBObject)null && a.Name == variable.AttributeBasedOn.Name)
                {
                    output.AddLine("&" + variable.Name + " based on  " + a.Name);
                    eDBType type = variable.Type;
                    int length = variable.Length;
                    bool signed = variable.Signed;
                    string description = variable.Description;
                    int decimals = variable.Decimals;
                    variable.AttributeBasedOn = (Artech.Genexus.Common.Objects.Attribute)null;
                    variable.Type = type;
                    variable.Decimals = decimals;
                    variable.Description = description;
                    variable.Length = length;
                    variable.Signed = signed;
                }
            }
        }

        internal static bool hasModule(KBObject obj)
        {
            return !(obj.Module.Guid == Guid.Empty) && !(obj is Module) && !(obj is Folder);
        }

        internal static KBCategory MainCategory(KBModel model)
        {
            return KBCategory.Get(model, "Main Programs");
        }

        private static void CleanSDT(Artech.Genexus.Common.Objects.Attribute a, IOutputService output, KBObject objRef)
        {
            if (!(objRef is Artech.Genexus.Common.Objects.SDT))
                return;
            output.AddLine("Cleaning SDT references to " + a.Name + " in " + objRef.Name);
            foreach (IStructureItem structureItem in (IEnumerable<IStructureItem>)objRef.Parts.Get<SDTStructurePart>().Root.Items)
            {
                try
                {
                    SDTItem sdtItem = (SDTItem)structureItem;
                    EntityKey entityKey = Utility.KeyOfBasedOn_CompatibleConEvo3(sdtItem);
                    if (sdtItem.BasedOn != null && entityKey == a.Key)
                    {
                        output.AddLine("..." + sdtItem.Name + " based on  " + a.Name);
                        eDBType type = sdtItem.Type;
                        int length = sdtItem.Length;
                        bool signed = sdtItem.Signed;
                        string description = sdtItem.Description;
                        int decimals = sdtItem.Decimals;
                        sdtItem.AttributeBasedOn = (Artech.Genexus.Common.Objects.Attribute)null;
                        sdtItem.Type = type;
                        sdtItem.Decimals = decimals;
                        sdtItem.Description = description;
                        sdtItem.Length = length;
                        sdtItem.Signed = signed;
                    }
                }
                catch (Exception ex)
                {
                    output.AddErrorLine(ex.Message);
                }
            }
        }

        public static string ReturnPicture(Artech.Genexus.Common.Objects.Attribute a)
        {
            return a.Type.ToString() + "(" + a.Length.ToString() + (a.Decimals > 0 ? "." + a.Decimals.ToString() : "") + ")" + (a.Signed ? "-" : "");
        }

        public static string ReturnPictureVariable(Variable v)
        {
            return v.Type.ToString() + "(" + v.Length.ToString() + (v.Decimals > 0 ? "." + v.Decimals.ToString() : "") + ")" + (v.Signed ? "-" : "");
        }

        public static string ReturnPictureDomain(Domain d)
        {
            return d.Type.ToString() + "(" + d.Length.ToString() + (d.Decimals > 0 ? "." + d.Decimals.ToString() : "") + ")" + (d.Signed ? "-" : "");
        }

        public static void SaveObject(IOutputService output, KBObject obj)
        {
            try
            {
                obj.Save();
            }
            catch (Exception ex)
            {
                output.AddErrorLine(ex.Message + " - " + (object)ex.InnerException);
            }
        }
    }

}
