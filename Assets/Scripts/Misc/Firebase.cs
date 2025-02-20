using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class FirebaseScript : MonoBehaviour
{
    public static FirebaseScript Instance;
    FirebaseDatabase db;
    FirebaseAuth auth;

    public int playerHighscore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep FirebaseScript alive across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy any duplicate instances
        }
    }
    void Start()
    {

        // Setup for talking to Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // ✅ SET THE DATABASE URL HERE
                app.Options.DatabaseUrl = new System.Uri("https://blobber-game-firebase-default-rtdb.europe-west1.firebasedatabase.app/");

                // Initialize database reference
                db = FirebaseDatabase.DefaultInstance;

                // Set the value "World" to the key "Hello" in the database
                db.RootReference.Child("Hello").SetValueAsync("World")
                    .ContinueWithOnMainThread(dbTask =>
                    {
                        if (dbTask.IsFaulted)
                            Debug.LogError("Error writing to database: " + dbTask.Exception);
                        else
                            Debug.Log("Successfully wrote to database!");
                    });
                auth = FirebaseAuth.DefaultInstance;
                if (auth.CurrentUser == null)
                {
                    // If no user is logged in, sign in anonymously
                    SignInAnonymously();
                }
                else
                {
                    // If a user is logged in (either anonymously or via email/password), log them
                    Debug.Log("User already logged in: " + auth.CurrentUser.UserId);
                }
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
            }
        });
    }

    void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Sign in failed" + task.Exception);
            }
            else
            {
                Debug.Log("Signed in successfully as: " + auth.CurrentUser.UserId);
            }
        });
    }

    public void SavePlayerHighscore(int score)
    {
        if (auth.CurrentUser != null)
        {
        string userId = auth.CurrentUser.UserId;
        DatabaseReference userHighscoreRef = db.RootReference.Child("HighScores").Child(userId).Child("score");

        userHighscoreRef.SetValueAsync(score).ContinueWithOnMainThread(dbTask =>
        {

            if (dbTask.IsCompleted && !dbTask.IsFaulted)
            {
                Debug.Log("saved to firebase");
                userHighscoreRef.GetValueAsync().ContinueWithOnMainThread(getTask =>
                {
                    if (getTask.IsCompleted && getTask.Result.Exists)
                    {
                        playerHighscore = int.Parse(getTask.Result.Value.ToString());
                        Debug.Log("Player high score updated: " + playerHighscore);
                    }
                    else
                    {
                        Debug.LogError("Failed to retrieve updated high score: " + getTask.Exception);
                    }
                });
            }
            else
            {
                Debug.LogError("Error retrieving player high score: " + dbTask.Exception);
            }
        });
        }
        else
        {
            Debug.LogError("User is not authenticated.");
        }
    }
    public void GetPlayerHighscoreFromFirebase(Action<int> onHighscoreRetrieved)
    {
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId;
            DatabaseReference userHighscoreRef = db.RootReference.Child("HighScores").Child(userId).Child("Score");

            userHighscoreRef.GetValueAsync().ContinueWithOnMainThread(dbTask =>
            {
                if (dbTask.IsCompleted && dbTask.Result.Exists)
                {
                    int firebaseHighScore = int.Parse(dbTask.Result.Value.ToString());
                    onHighscoreRetrieved(firebaseHighScore);
                }
                else
                {
                    Debug.LogError("Failed to retrieve high score from Firebase: " + dbTask.Exception);
                    onHighscoreRetrieved(0); // Default to 0 if no score exists
                }
            });
        }
        else
        {
            Debug.LogError("User is not authenticated.");
            onHighscoreRetrieved(0);
        }
    }

    public void SaveHighScoreToFirebase(int score)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;     
        if (auth.CurrentUser != null)
        {
            string userID = auth.CurrentUser.UserId;

            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("HighScores").Child(userID).Child("Score").SetValueAsync(score).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("High score saved to Firebase!");
                }
                else
                {
                    Debug.LogError("Error saving high score: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("User is not authenticated.");
        }
    }
}
