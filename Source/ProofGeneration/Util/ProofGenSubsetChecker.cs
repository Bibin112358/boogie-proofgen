using System;
using System.Linq;
using System.Transactions;
using Isabelle.Ast;
using Microsoft.Boogie;
using Type = Microsoft.Boogie.Type;

namespace ProofGeneration.Util
{
    /// <summary>
    /// Checks in a whether the input program is potentially supported by proof generation.
    /// </summary>
    public class ProofGenSubsetChecker : ResultReadOnlyVisitor<Term>
    {
        private object problematicNode;
        private bool result;

        protected override bool TranslatePrecondition(Absy node)
        {
            return true;
        }

        /// <summary>
        /// If false is returned, then the input is not supported.
        /// If true is returned, then the input is potentially supported.
        /// </summary>
        public bool ProofGenPotentiallySupportsSubset(Program p, out object resultNode)
        {
            problematicNode = null;
            Visit(p);
            resultNode = problematicNode;
            return problematicNode == null;
        }

        public override Implementation VisitImplementation(Implementation node)
        {
            //no procedure type parameters, free pre- and postconditions, no where clauses
            if (node.Proc.TypeParameters.Any() ||
                node.Proc.Requires.Any(req => req.Free) || 
                node.Proc.Ensures.Any(ens => ens.Free) ||
                node.Proc.InParams.Union(node.Proc.OutParams).Union(node.LocVars).Any(v => v.TypedIdent.WhereExpr != null))
            {

                if (node.Proc.TypeParameters.Any()) Console.WriteLine("| procedure type parameters |");
                if (node.Proc.Requires.Any(req => req.Free) || node.Proc.Ensures.Any(ens => ens.Free)) Console.WriteLine("| free requires |");
                if (node.Proc.InParams.Union(node.Proc.OutParams).Union(node.LocVars).Any(v => v.TypedIdent.WhereExpr != null)) Console.WriteLine("| where clauses |");
                problematicNode = node;
                //return node;
            }

            return base.VisitImplementation(node);
        }

        public override Type VisitBasicType(BasicType node)
        {
            //only support integers, booleans, and reals as basic types
            if (node.isFloat || node.IsBv || node.IsString || node.IsRMode || node.IsRegEx)
            {
                problematicNode = node;
                Console.WriteLine("| Basic Type | VisitBasicType |");
                //return node;
            }

            return base.VisitBasicType(node);
        }
        
        public override Type VisitFloatType(FloatType node)
        {
            problematicNode = node;
            Console.WriteLine("| Float | VisitFloatType |");
            return node;
        }

        #region regex
        //do not support regular expressions
        public override Sequential VisitSequential(Sequential node)
        {
            problematicNode = node;
            Console.WriteLine("| Regex |");
            return node;
        }

        public override Choice VisitChoice(Choice node)
        {
            problematicNode = node;
            Console.WriteLine("| Regex |");
            return node;
        }
        
        public override Cmd VisitRE(RE node)
        {
            problematicNode = node;
            Console.WriteLine("| Regex |");
            return node;
        }
        
        public override AtomicRE VisitAtomicRE(AtomicRE node)
        {
            problematicNode = node;
            Console.WriteLine("| Regex |");
            return node;
        }
        #endregion

        
        //do not support code block expressions
        public override Expr VisitCodeExpr(CodeExpr node)
        {
            problematicNode = node;
            Console.WriteLine("| Code block expressions |");
            return node;
        }

        #region maps 
        
        //do not support maps
        public override Expr VisitLambdaExpr(LambdaExpr node)
        {
            problematicNode = node;
            Console.WriteLine("| Maps | VisitLambdaExpr | ");
            return node;
        }

        public override MapType VisitMapType(MapType node)
        {
            problematicNode = node;
            Console.WriteLine("| Maps | VisitMapType | ");
            return node;
        }

        public override AssignLhs VisitMapAssignLhs(MapAssignLhs node)
        {
            problematicNode = node;
            Console.WriteLine("| Maps | VisitMapAssignLhs |");
            return node;
        }

        public override Type VisitMapTypeProxy(MapTypeProxy node)
        {
            problematicNode = node;
            Console.WriteLine("| Maps | VisitMapTypeProxy |");
            return node;
        }
        #endregion
        
        #region bitvectors
        
        //do not support bitvectors
        public override Type VisitBvType(BvType node)
        {
            problematicNode = node;
            Console.WriteLine("| Bitvectors | VisitBvType |");
            return node;
        }
        
        public override Expr VisitBvConcatExpr(BvConcatExpr node)
        {
            problematicNode = node;
            Console.WriteLine("| Bitvectors | VisitBvConcatExpr |");
            return node;
        }

        public override Expr VisitBvExtractExpr(BvExtractExpr node)
        {
            problematicNode = node;
            Console.WriteLine("| Bitvectors | VisitBvExtractExpr |");
            return node;
        }

        public override Type VisitBvTypeProxy(BvTypeProxy node)
        {
            problematicNode = node;
            Console.WriteLine("| Bitvectors | VisitBvTypeProxy |");
            return node;
        }
        #endregion
        
        #region civl
        
        //do not support concurrent intermediate verification language (CIVL) features

        public override Cmd VisitParCallCmd(ParCallCmd node)
        {
            problematicNode = node;
            Console.WriteLine("| CIVL |");
            return node;
        }
        #endregion
    }
}