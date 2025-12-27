using Acme.Packages.Menu.Core.Application.Services;
using Acme.Packages.Menu.Core.Domain.Interfaces;
using Acme.Packages.Menu.Core.Infrastructure.GeneXus;
using Acme.Packages.Menu.Core.Infrastructure.Logging;
using Acme.Packages.Menu.Core.Infrastructure.Formatters;

namespace Acme.Packages.Menu.Common.Factories
{
    public static class ServiceFactory
    {
        private static ILogger _logger;
        private static IVariableRepository _variableRepository;
        private static ITypeResolver _typeResolver;
        private static VariableService _variableService;
                private static IDocumentationService _documentationService;
                private static IDocumentationFormatter _documentationFormatter;
                        private static IVariableCleanerService _variableCleanerService;
                        private static ISmartVariableService _smartVariableService;
                
                        public static ILogger GetLogger()
                        {
                            return _logger ?? (_logger = new ConsoleLogger());
                        }
                
                        public static IVariableRepository GetVariableRepository()
                        {
                            return _variableRepository ?? (_variableRepository = new GeneXusVariableRepository());
                        }
                
                        public static ITypeResolver GetTypeResolver()
                        {
                            return _typeResolver ?? (_typeResolver = new GeneXusTypeResolver(GetLogger()));
                        }
                
                        public static VariableService GetVariableService()
                        {
                            return _variableService ?? (_variableService = new VariableService(
                                GetVariableRepository(),
                                GetTypeResolver(),
                                GetLogger()));
                        }
                
                        public static IDocumentationService GetDocumentationService()
                        {
                            return _documentationService ?? (_documentationService = new DocumentationService());
                        }
                
                        public static IDocumentationFormatter GetDocumentationFormatter()
                        {
                            return _documentationFormatter ?? (_documentationFormatter = new MarkdownDocumentationFormatter());
                        }
                
                        public static IVariableCleanerService GetVariableCleanerService()
                        {
                            return _variableCleanerService ?? (_variableCleanerService = new VariableCleanerService());
                        }
                
                        public static ISmartVariableService GetSmartVariableService()
                        {
                            return _smartVariableService ?? (_smartVariableService = new SmartVariableService());
                        }
                    }
                }
                