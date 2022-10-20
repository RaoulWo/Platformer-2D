using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PhysicsObjectTests
{
    private GameObject groundPrefab;
    private GameObject physicsObjectPrefab;
    private float initialYPosition;
    private float updatedYPosition;
    
    [SetUp]
    public void SetUp()
    {
        // Create a ground prefab
        groundPrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Ground"));
        
        // Create a physicsObject prefab
        physicsObjectPrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/PhysicsObject"));
        
        // Store the initial y-position
        initialYPosition = physicsObjectPrefab.transform.position.y;
    }

    [UnityTest]
    public IEnumerator Physics_object_should_be_affected_by_gravity()
    {
        // ASSIGN

        // ACT
        yield return new WaitForSeconds(0.25f);
        updatedYPosition = physicsObjectPrefab.transform.position.y;

        // ASSERT
        Assert.Less(updatedYPosition, initialYPosition);
    }

    [TearDown]
    public void TearDown()
    {
        // Destroy the gameObjects
        Object.Destroy(groundPrefab);
        Object.Destroy(physicsObjectPrefab);
        
        // Reset initial position
        initialYPosition = 0f;

        // Reset the updated position
        updatedYPosition = 0f;
    }
}
