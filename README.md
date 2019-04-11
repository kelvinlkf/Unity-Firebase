# Unity-Firebase
This project consists the configuration for unity to connect with Google Firebase.
Below are the details and sample for the projects.

Google Firebase, Unity official document and configuration
https://firebase.google.com/docs/unity/setup?gclid=CjwKCAjwqLblBRBYEiwAV3pCJu4Z2W12Y3_ZQIqN997-2TXnwp4Ev8VlQGF_UTlj7mvdkytPLe_0uhoC6XkQAvD_BwE

# Content
1. Firebase Authentication (Email & Password)
2. Firebase Realtime Database (Create, Read, Update)
3. Firebase Exception Handler

# Sidenote
- Download google-service.json from your Google Firebase Project and place it inside your "Assets" folder
  (google-service.json will connect your app with the services enabled in your firebase project)
- Replace the database link in GoogleFirebase.cs/InitDatabase/Firebase.FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://FIREBASE-PROJECT.firebaseio.com/")
  (Replace FIREBASE-PROJECT with your respective firebase project
- For testing environment, it's better to set the rules to public instead of private. But make sure you change it back to private when you are launching your app into public

