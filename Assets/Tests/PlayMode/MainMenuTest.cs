using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MainMenuTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void MainMenuTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MainMenuTestWithEnumeratorPasses()
    {
        var gameObject = new GameObject();
        var button = gameObject.FindObjectOfType<>();
        button.click();
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
