using UnityEngine;

public class Move : MonoBehaviour
{

    /*
     * ++ Joystick ten input al
     * ++ Joystickten alýnan inputa göre movement yap
     *    ++ MoveSpeed
     *    ++CharaterController move
     *    -- Gravity hallet
     * ++ Rotasyon yap
     *      Smooth Rotasyon ekle
     * Camera Takibi
     * Dash Mekaniði
     * Fire Mekaniði
     * Animasyonlar
     * Particle Effect (Dash, Fire)
     * Sesler (Move, Ambians, Music, Fire, Dash)
     */

    [SerializeField] private Joystick joystick;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 10f;


    private void Update()
    {
        if(joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            return;
        }

        Vector3 joystickVector = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        //Debug.Log("Joystick Vector: " + joystickVector);


        // Move Vector yap
        Vector3 moveVector = joystickVector * speed * Time.deltaTime;

        characterController.Move(moveVector);

        // Rotasyon yap
        Vector3 lookVector = joystickVector.normalized;

        transform.rotation = Quaternion.LookRotation(lookVector);

        
    }

}
