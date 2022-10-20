using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControllerTests
{
    private GameObject groundPrefab;
    private GameObject playerPrefab;
    private PlayerController playerController;
    private float initialXPosition;
    private float initialYPosition;
    private float updatedXPosition;
    private float updatedYPosition;

    [SetUp]
    public void SetUp()
    {
        // Create a ground prefab
        groundPrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Ground"));
        
        // Create a player prefab
        playerPrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        // Get playerController component
        playerController = playerPrefab.GetComponent<PlayerController>();
        // Substitute the playerInput
        playerController.PlayerInput = Substitute.For<IPlayerInput>();
        
        // Store the initial x-position
        initialXPosition = playerPrefab.transform.position.x;
        // Store the initial y-position
        initialYPosition = playerPrefab.transform.position.y;
    }

    [UnityTest]
    public IEnumerator Move_right_with_positive_horizontal_input()
    {
        // ASSIGN
        playerController.PlayerInput.Horizontal.Returns(1f);

        // ACT
        yield return new WaitForSeconds(0.25f);
        updatedXPosition = playerPrefab.transform.position.x;

        // ASSERT
        Assert.Greater(updatedXPosition, initialXPosition);
    }

    [UnityTest]
    public IEnumerator Move_left_with_negative_horizontal_input()
    {
        // ASSIGN
        playerController.PlayerInput.Horizontal.Returns(-1f);

        // ACT
        yield return new WaitForSeconds(0.25f);
        updatedXPosition = playerPrefab.transform.position.x;

        // ASSERT
        Assert.Less(updatedXPosition, initialXPosition);
    }

    [UnityTest]
    public IEnumerator Jump_up_when_grounded_and_jump_button_down_input()
    {
        // ASSIGN
        playerController.PlayerInput.JumpButtonDown.Returns(true);

        // ACT
        yield return new WaitForSeconds(0.25f);
        updatedYPosition = playerPrefab.transform.position.y;

        // ASSERT
        Assert.Greater(updatedYPosition, initialYPosition);
    }

    [UnityTest]
    public IEnumerator Do_not_jump_when_in_air_and_jump_button_down_input()
    {
        // ASSIGN
        playerPrefab.transform.position = new Vector3(0f, 5f, 0f);
        initialYPosition = playerPrefab.transform.position.y;
        playerController.PlayerInput.JumpButtonDown.Returns(false);

        // ACT
        yield return new WaitForSeconds(0.25f);
        playerController.PlayerInput.JumpButtonDown.Returns(true);
        updatedYPosition = playerPrefab.transform.position.y;

        // ASSERT
        Assert.Less(updatedYPosition, initialYPosition);
    }

    [TearDown]
    public void TearDown()
    {
        // Destroy the gameObjects
        Object.Destroy(groundPrefab);
        Object.Destroy(playerPrefab);
        
        // Reset the initial positions
        initialXPosition = 0f;
        initialYPosition = 0f;

        // Reset the updated positions
        updatedXPosition = 0f;
        updatedYPosition = 0f;
    }
}
