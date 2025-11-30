using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    public void RespawnCheck()
    {
        if (currentCheckpoint == null)
        {
            if (uiManager != null) uiManager.GameOver();
            return;
        }

        playerHealth.Respawn();
        transform.position = currentCheckpoint.position;

        if (Camera.main != null && Camera.main.TryGetComponent(out CameraController camController))
        {
            camController.MoveToNewRoom(currentCheckpoint.parent);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform;

            if (SoundManager.instance != null)
                SoundManager.instance.PlaySound(checkpointSound);

            collision.enabled = false;

            if (collision.TryGetComponent(out Animator checkpointAnim))
            {
                checkpointAnim.SetTrigger("activate");
            }
        }
    }
}