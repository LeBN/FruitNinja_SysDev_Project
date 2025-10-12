using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.XR;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    [Header("References")]
    public FruitSpawnerManager spawner;
    public GameObject startFruitPrefab;
    public Transform startFruitSpawnPoint;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI timerText;


    [Header("Game settings")]
    public float gameDuration = 60f;

    private bool gameStarted = false;
    private int score = 0;
    private float timeRemaining;

    private Coroutine timerCoroutine;

    // Called by StartFruit when it is cut
    public void StartGame()
    {
        if (gameStarted)
        {
            Debug.Log("Le jeu est déjà lancé !");
            return;
        }
        gameStarted = true;

        // Reset
        score = 0;
        UpdateScoreUI();

        timeRemaining = gameDuration;
        UpdateTimerUI();

        // Start the spawner and timer
        if (spawner != null)
            spawner.StartSpawning();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(GameTimer());
    }


    void Update()
    {
        // Clavier (nouveau Input System)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("Touche R pressée - Respawn du StartFruit");
            RespawnStartFruit();
        }

        // Manette VR (XR Input)
        UnityEngine.XR.InputDevice rightHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        bool buttonPressed = false;

        if (rightHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out buttonPressed) && buttonPressed)
        {
            Debug.Log("Bouton manette VR pressé - Respawn du StartFruit");
            RespawnStartFruit();
        }
    }

private IEnumerator GameTimer()
    {
        while (gameStarted && timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            UpdateTimerUI();
        }

        if (gameStarted)
        {
            Debug.Log("Timer terminé — appel de EndGame()");
            EndGame();
        }

        timerCoroutine = null; // libère la référence
    }



    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void EndGame()
    {
        if (!gameStarted) return;
        gameStarted = false;

        Debug.Log("Fin de partie : chrono terminé");

        // Stop spawner
        if (spawner != null)
            spawner.StopSpawning();

        // Détruit tous les fruits restants
        try
        {
            foreach (var fruit in GameObject.FindGameObjectsWithTag("Sliceable"))
                Destroy(fruit);
        }
        catch
        {
            Debug.LogWarning("Le tag 'Sliceable' n'est pas accessible pour le moment.");
        }

        try
        {
            foreach (var bomb in GameObject.FindGameObjectsWithTag("Bomb"))
                Destroy(bomb);
        }
        catch
        {
            Debug.LogWarning("Le tag 'Bomb' n'est pas accessible pour le moment.");
        }


        // Respawn du StartFruit
        if (startFruitPrefab != null)
        {
            Vector3 spawnPos = startFruitSpawnPoint != null
                ? startFruitSpawnPoint.position
                : new Vector3(0, 1.5f, 0);

            GameObject newStart = Instantiate(startFruitPrefab, spawnPos, Quaternion.identity);
            newStart.SetActive(true);

            Debug.Log("StartFruit respawned !");
        }
        else
        {
            Debug.LogWarning("StartFruitPrefab n'est pas assigné dans le GameManager !");
        }
    }

    public void RespawnStartFruit()
    {
        // Evite le double spawn
        if (GameObject.FindWithTag("StartButton") != null)
        {
            Debug.Log("StartFruit déjà présent, pas besoin de respawn.");
            return;
        }

        if (startFruitPrefab == null)
        {
            Debug.LogWarning("StartFruitPrefab manquant, impossible de respawn !");
            return;
        }

        Vector3 spawnPos = startFruitSpawnPoint != null
            ? startFruitSpawnPoint.position
            : new Vector3(0, 1.5f, 0);

        Instantiate(startFruitPrefab, spawnPos, Quaternion.identity);
        Debug.Log("StartFruit respawné manuellement !");
    }



    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = "Temps: " + Mathf.CeilToInt(timeRemaining).ToString() + "s";
    }

    // Methode appelee par SlideObject quand une bombe explose
    public void ResetFruitsOnBomb()
    {
        foreach (var fruit in GameObject.FindGameObjectsWithTag("Sliceable"))
            Destroy(fruit);
    }
}
