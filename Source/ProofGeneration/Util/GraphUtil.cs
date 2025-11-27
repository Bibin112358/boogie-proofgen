using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Boogie;
using Microsoft.Boogie.GraphUtil;

namespace ProofGeneration.Util
{
    public static class GraphUtil
    {
        public static Graph<Block> GraphFromBlocks(IList<Block> blocks, bool reverse = false)
        {
            //adusted code from VC.cs
            var dag = new Graph<Block>();
            dag.AddSource(blocks[0]);
            foreach (var b in blocks)
            {
                var gtc = b.TransferCmd as GotoCmd;
                if (gtc != null)
                {
                    Contract.Assume(gtc.LabelTargets != null);
                    foreach (var dest in gtc.LabelTargets)
                    {
                        Contract.Assert(dest != null);
                        if (reverse)
                            dag.AddEdge(dest, b);
                        else
                            dag.AddEdge(b, dest);
                    }
                }
            }

            return dag;
        }
    }
}