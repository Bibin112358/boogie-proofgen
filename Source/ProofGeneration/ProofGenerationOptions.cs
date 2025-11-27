using System.Diagnostics.Contracts;
using Microsoft.Boogie;

namespace ProofGeneration;

public interface ProofGenerationOptions : CoreOptions
{
  
  // TODO: remove static references like Dafny
  // github.com/dafny-lang/dafny/pull/3663
  public static ProofGenerationOptions Clo { get;  private set; }

  public static void Install(ProofGenerationOptions options)
  {
    Contract.Requires(options != null);
    Clo = options;
  }

  string ProofOutputDir { get; }
  bool OnlyCheckProofGenSupport { get; }
  bool DontStoreProofGenFiles { get; }

  /* If use id-based lemma naming, then whenever lemmas are specific to an entity that is represented using an id (i.e., a natural number),
    then the lemma name is uniquely determined by that id. For example, if variables are represented using natural numbers,
    then the membership lemmas for those variables are uniquely determined by the corresponding natural number. */
  bool UseIdBasedLemmaNaming { get; }

  /*
   * 0: disabled -> proofs are generated
   * 1: partially enabled -> only AST program is generated but with membership lemmas
   * 2: enabled -> only AST program is generated
   */
  int GenerateIsaProgNoProofs { get; }

  bool OnlyGenerateInitialProgramIsa()
  {
    return GenerateIsaProgNoProofs != 0;
  }

  bool DesugarMaps { get; }
}
