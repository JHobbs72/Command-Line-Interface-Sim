using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class NewTestScript
{
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        yield return null;
    }
}
