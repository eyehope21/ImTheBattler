using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class FirebaseInit : MonoBehaviour
{
    // A static reference to the Firebase Authentication instance.
    // This allows other scripts to access it.
    public static FirebaseAuth auth;

    // The Awake method is called before Start, so it's a good place for initialization.
    void Awake()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        // Check for all Firebase dependencies to ensure the SDK is set up correctly.
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Get the default Firebase Authentication instance.
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                // Log a critical error if dependencies are not met.
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
}