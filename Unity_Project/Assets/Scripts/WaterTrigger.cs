using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    // --------------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if (playerController && playerController.IsAlive())
            {
                //if player does not have the inivincibility power up
                if (playerController.GetPlayerPowerUpType() != PowerUpType.INVINCIBILITY)
                {
                    //give the other player a point
                    GameController.INSTANCE.AddToPlayerScore(playerController.GetOtherPlayerNum(), 1);

                    playerController.PlayClip(GameController.INSTANCE.GetClip("DyingWithoutPowerup"));
                }
                else
                {
                    //otherwise, just respawn player after a pop up
                    GameController.INSTANCE.CreatePopUp("Player " + playerController.GetPlayerNum() + " used their Invincibility!",
                        playerController.GetPlayerNum(), Color.magenta);

                    playerController.PlayClip(GameController.INSTANCE.GetClip("DyingWithPowerup"));
                }

                // Kill the player
                playerController.Die();
            }
        }
        else if (other.gameObject.tag == "Ball")
        {
            BallManager ballManager = other.gameObject.GetComponent<BallManager>();

            if (ballManager)
            {
                //respawn ball
                GameController.INSTANCE.AddToPlayerScore(ballManager.GetWinningPlayer(), 1);

                if (ballManager.GetWinningPlayer() == 0)
                    ballManager.PlayClip(GameController.INSTANCE.GetClip("BallDyingWithoutPlayer"));
                else
                    ballManager.PlayClip(GameController.INSTANCE.GetClip("BallDyingWithPlayer"));

                ballManager.Respawn();
            }
        }
    }
}
