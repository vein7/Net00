using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using static System.Console;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalysisApp1 {
    class Program {
        const string programText =
@"using System;
using System.Collections;
using System.Linq;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}";
        static void Main() {
            //SemanticQuickStart();
            //ConstructionCS();
            Compilation test = CreateTestCompilation();
            foreach (SyntaxTree sourceTree in test.SyntaxTrees) {
                SemanticModel model = test.GetSemanticModel(sourceTree);

                TypeInferenceRewriter rewriter = new TypeInferenceRewriter(model);

                SyntaxNode newSource = rewriter.Visit(sourceTree.GetRoot());

                if (newSource != sourceTree.GetRoot()) {
                    File.WriteAllText(sourceTree.FilePath, newSource.ToFullString());
                }
            }
        }

        private static Compilation CreateTestCompilation() {
            String programPath = @"..\..\..\Program.cs";
            String programText = File.ReadAllText(programPath);
            SyntaxTree programTree = CSharpSyntaxTree.ParseText(programText)
                                           .WithFilePath(programPath);

            String rewriterPath = @"..\..\..\TypeInferenceRewriter.cs";
            String rewriterText = File.ReadAllText(rewriterPath);
            SyntaxTree rewriterTree = CSharpSyntaxTree.ParseText(rewriterText)
                                           .WithFilePath(rewriterPath);

            SyntaxTree[] sourceTrees = { /*programTree,*/ rewriterTree };

            MetadataReference mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference codeAnalysis = MetadataReference.CreateFromFile(typeof(SyntaxTree).Assembly.Location);
            MetadataReference csharpCodeAnalysis = MetadataReference.CreateFromFile(typeof(CSharpSyntaxTree).Assembly.Location);

            MetadataReference[] references = { mscorlib, codeAnalysis, csharpCodeAnalysis };

            return CSharpCompilation.Create("TransformationCS",
                sourceTrees,
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }

        static void SyntaxTest1(string[] args) {
            var tree = CSharpSyntaxTree.ParseText(programText);
            var root = tree.GetCompilationUnitRoot();
            Console.WriteLine($"The tree is a {root.Kind()} node.");
            Console.WriteLine($"The tree has {root.Members.Count} elements in it.");
            Console.WriteLine($"The tree has {root.Usings.Count} using statements. They are:");
            foreach (UsingDirectiveSyntax element in root.Usings)
                Console.WriteLine($"\t{element.Name}");

            MemberDeclarationSyntax firstMember = root.Members[0];
            Console.WriteLine($"The first member is a {firstMember.Kind()}.");
            var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;
            Console.WriteLine($"There are {helloWorldDeclaration.Members.Count} members declared in this namespace.");
            Console.WriteLine($"The first member is a {helloWorldDeclaration.Members[0].Kind()}.");

            var programDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];
            Console.WriteLine($"There are {programDeclaration.Members.Count} members declared in the {programDeclaration.Identifier} class.");
            Console.WriteLine($"The first member is a {programDeclaration.Members[0].Kind()}.");

            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];
            Console.WriteLine($"The return type of the {mainDeclaration.Identifier} method is {mainDeclaration.ReturnType}.");
            Console.WriteLine($"The method has {mainDeclaration.ParameterList.Parameters.Count} parameters.");
            foreach (ParameterSyntax item in mainDeclaration.ParameterList.Parameters)
                Console.WriteLine($"The type of the {item.Identifier} parameter is {item.Type}.");
            Console.WriteLine($"The body text of the {mainDeclaration.Identifier} method follows:");
            Console.WriteLine(mainDeclaration.Body.ToFullString());

            var argsParameter = mainDeclaration.ParameterList.Parameters[0];
        }

        static void SemanticQuickStart() {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);

            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                .AddSyntaxTrees(tree);

        }

        static void ConstructionCS() {
            NameSyntax name = IdentifierName("System");
            WriteLine($"Created the identifier {name.ToString()}");
            name = QualifiedName(name, IdentifierName("Collections"));
            WriteLine(name.ToString());
            name = QualifiedName(name, IdentifierName("Generic"));
            WriteLine(name.ToString());
            WriteLine("\n");

            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var oldUsing = root.Usings[1];
            var newUsing = oldUsing.WithName(name);
            var newUsing2 = oldUsing.WithName(IdentifierName("System"));
            root = root.ReplaceNode(oldUsing, newUsing);
            WriteLine(root.ToString());
            WriteLine("\n");

            root.Usings.Replace(newUsing, newUsing2);
            WriteLine(root.ToString());
        }


        static async Task Main2(string[] args) {
            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances.Length == 1
                // If there is only one instance of MSBuild on this machine, set that as the one to use.
                ? visualStudioInstances[0]
                // Handle selecting the version of MSBuild you want to use.
                : SelectVisualStudioInstance(visualStudioInstances);

            Console.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            // NOTE: Be sure to register an instance with the MSBuildLocator 
            //       before calling MSBuildWorkspace.Create()
            //       otherwise, MSBuildWorkspace won't MEF compose.
            MSBuildLocator.RegisterInstance(instance);

            using (var workspace = MSBuildWorkspace.Create()) {
                // Print message for WorkspaceFailed event to help diagnosing project load failures.
                workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

                var solutionPath = args[0];
                Console.WriteLine($"Loading solution '{solutionPath}'");

                // Attach progress reporter so we print projects as they are loaded.
                var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
                Console.WriteLine($"Finished loading solution '{solutionPath}'");

                // TODO: Do analysis on the projects in the loaded solution
            }
        }

        private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances) {
            Console.WriteLine("Multiple installs of MSBuild detected please select one:");
            for (int i = 0; i < visualStudioInstances.Length; i++) {
                Console.WriteLine($"Instance {i + 1}");
                Console.WriteLine($"    Name: {visualStudioInstances[i].Name}");
                Console.WriteLine($"    Version: {visualStudioInstances[i].Version}");
                Console.WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
            }

            while (true) {
                var userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int instanceNumber) &&
                    instanceNumber > 0 &&
                    instanceNumber <= visualStudioInstances.Length) {
                    return visualStudioInstances[instanceNumber - 1];
                }
                Console.WriteLine("Input not accepted, try again.");
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress> {
            public void Report(ProjectLoadProgress loadProgress) {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null) {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }
}
