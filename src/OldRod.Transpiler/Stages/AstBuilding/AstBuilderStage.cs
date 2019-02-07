using System;
using System.Linq;
using OldRod.Core.Ast;
using Rivers.Serialization.Dot;

namespace OldRod.Transpiler.Stages.AstBuilding
{
    public class AstBuilderStage : IStage
    {
        public const string Tag = "AstBuilder";
        
        public string Name => "IL AST builder stage";

        public void Run(DevirtualisationContext context)
        {
            var builder = new ILAstBuilder(context.TargetImage)
            {
                Logger = context.Logger
            };

            foreach (var entry in context.ControlFlowGraphs)
            {
                uint entryId = context.KoiStream.Exports.First(x => x.Value == entry.Key).Key;
                context.Logger.Debug(Tag, $"Building AST for export {entryId}...");
                var unit = builder.BuildAst(entry.Value);
                context.CompilationUnits[entry.Key] = unit;

                foreach (var node in unit.ControlFlowGraph.Nodes)
                {
                    node.UserData["rankdir"] = "LR";
                    node.UserData["label"] = node.UserData[ILAstBlock.AstBlockProperty];
                }

                var writer = new DotWriter(Console.Out, new BasicBlockSerializer());
                writer.Write(unit.ControlFlowGraph);
            }
        }
    }
}