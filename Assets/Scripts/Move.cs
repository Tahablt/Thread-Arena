using UnityEngine;

public class Move : MonoBehaviour
{
    /*
     * ++ Joystick ten input al
     * ++ Joystickten alýnan inputa göre movement yap
     * ++ MoveSpeed
     * ++CharaterController move
     * -- Gravity hallet
     * ++ Rotasyon yap
     * ++Smooth Rotasyon ekle
     * ++Camera Takibi
     * ++Dash Mekaniði
     * Fire Mekaniði
     * ++Animasyonlar (Þu an yapýyoruz!)
     * Particle Effect (Dash, Fire)
     * Sesler (Move, Ambians, Music, Fire, Dash)
     */

    [SerializeField] private Joystick joystick;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 10f;

    // Animasyonlarý kontrol etmek için Animator referansý ekledik
    [SerializeField] private Animator animator;

    private void Update()
    {
        Vector3 joystickVector = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        // Joystick'te bir hareket varsa (joystickVector'ün büyüklüðü 0'dan büyükse)
        if (joystickVector.magnitude > 0.1f)
        {
            // Yürüme animasyonunu tetikle
            animator.SetBool("isWalking", true);

            // Move Vector yap
            Vector3 moveVector = joystickVector * speed * Time.deltaTime;
            characterController.Move(moveVector);

            // Rotasyon yap
            Vector3 lookVector = joystickVector.normalized;
            transform.rotation = Quaternion.LookRotation(lookVector);
        }
        else
        {
            // Joystick býrakýldýysa ve karakter duruyorsa
            // Yürüme animasyonunu kapatýp Idle'a dön
            animator.SetBool("isWalking", false);
        }
    }
}