using UnityEngine;

public class DeathZone : MonoBehaviour
{
    /*
     * Variables
    */

    /*
    * Public Methods
    */

    /*
     * Private Methods
    */
    /*
     * Collision check for trigger. If a powerup hits the trigger it gets destroyed (has to be changed to add the object to a pool)
     * if a ball hits the zone, it will be reset to the paddle. If there is no component on the ball the object gets destroyed.
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PowerUp")
            HandlePowerUp(collision);
        else if (collision.tag == "Ball")
            HandleBall(collision);
        else
            Destroy(collision.gameObject);
    }

    private void HandlePowerUp(Collider2D collision)
    {
        var powerUp = collision.GetComponent<PowerUp>();
        if (powerUp == null)
            Destroy(collision.gameObject);
        powerUp.CollisionDetected(false, Player.localPlayer.playerName, Player.localPlayer.matchID, powerUp.GetPower());
    }
    private void HandleBall(Collider2D collision)
    {
        var ball = collision.gameObject.GetComponentInParent<Ball>();
        if (ball != null)
            ball.ResetBall();
        else
            Destroy(collision.gameObject);
    }
    /*
     * Setter and Getter Methods
    */
}

