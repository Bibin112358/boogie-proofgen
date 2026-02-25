using System;
using System.Collections.Generic;
using System.Linq;
using Isabelle.Ast;
using Microsoft.Boogie;
using ProofGeneration.BoogieIsaInterface.VariableTranslation;
using ProofGeneration.Util;

namespace ProofGeneration
{
    internal class ApplicableIsaVisitor : IAppliableVisitor<Term>
    {
        private readonly IList<Term> _args;
        private readonly TypeParamInstantiation _typeInst;

        private readonly TypeIsaVisitor typeIsaVisitor;

        public ApplicableIsaVisitor(TypeParamInstantiation typeInst,
            IList<Term> args,
            IVariableTranslation<TypeVariable> typeVarTranslation)
        {
            _typeInst = typeInst;
            _args = args;
            typeIsaVisitor = new TypeIsaVisitor(typeVarTranslation);
        }

        public Term Visit(UnaryOperator unaryOperator)
        {
            if (_args.Count != 1) throw new ExprArgException();

            return IsaBoogieTerm.Unop(unaryOperator.Op, _args[0]);
        }

        public Term Visit(BinaryOperator binaryOperator)
        {
            if (_args.Count != 2) throw new ExprArgException();

            return IsaBoogieTerm.Binop(binaryOperator.Op, _args[0], _args[1]);
        }

        public Term Visit(FunctionCall functionCall)
        {
            if (_args.Count != functionCall.ArgumentCount) throw new ExprArgException();

            var typeInstIsa = new List<Term>();
            foreach (var typeVar in _typeInst.FormalTypeParams)
            {
                var t = typeIsaVisitor.Translate(_typeInst[typeVar]);
                typeInstIsa.Add(t);
            }

            return IsaBoogieTerm.FunCall(functionCall.FunctionName, typeInstIsa, _args);
        }
        
        public Term Visit(TypeCoercion typeCoercion)
        {
            if(_args.Count != 1) throw new ExprArgException();

            //type coercions are only relevant for the type inference
            return _args[0];
        }

        public Term Visit(MapSelect mapSelect)
        {
            if (_args.Count != 2)
            {
                throw new NotImplementedException("Only 1-ary map are supported");
            }

            return IsaBoogieTerm.MapSelect(_args[0], _args[1]);
        }

        public Term Visit(MapStore mapStore)
        {
            if (_args.Count != 3)
            {
                throw new NotImplementedException("Only 1-ary map are supported");
            }

            return IsaBoogieTerm.MapStore(_args.First(), _args[1], _args.Last());
        }

        public Term Visit(ArithmeticCoercion arithCoercion)
        {
          if (_args.Count != 1)
          {
            throw new ExprArgException();
          }

          if(arithCoercion.Coercion == ArithmeticCoercion.CoercionType.ToReal)
          {
            return IsaBoogieTerm.IntToReal(_args[0]);
          }
          else
          {
            throw new NotImplementedException();
          }
        }

        public Term Visit(IfThenElse ifThenElse)
        {
            if (_args.Count != 3) throw new ExprArgException();

            return IsaBoogieTerm.CondExp(_args[0], _args[1], _args[2]);
        }

        public Term Visit(FieldAccess fieldAccess)
        {
          throw new NotImplementedException();
        }

        public Term Visit(FieldUpdate fieldUpdate)
        {
          throw new NotImplementedException();
        }

        public Term Visit(IsConstructor isConstructor)
        {
          throw new NotImplementedException();
        }
    }

    internal class ExprArgException : Exception
    {
    }
}