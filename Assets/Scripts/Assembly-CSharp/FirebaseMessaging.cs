using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Firebase;
//using Firebase.Messaging;
using UnityEngine;

public class FirebaseMessaging : MonoBehaviour
{
//	private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

	private bool isFirebaseInitialized;

	private void Start()
	{
	//	Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
	//	Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
		Debug.Log("Firebase Messaging Initialized");
		NotifyManager.removeAllBadgeNumber();
	}

	private void InitializeFirebase()
	{
	//	Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
	//	Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
		Debug.Log("Firebase Messaging Initialized");
	//	Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWith(delegate(Task task)
	//	{
	//		LogTaskCompletion(task, "RequestPermissionAsync");
	//	});
		isFirebaseInitialized = true;
	}
	/*
	public virtual void OnMessageReceived(object sender, MessageReceivedEventArgs e)
	{
		Debug.Log("Received a new message");
		NotifyManager.addBadgeNumber(1);
		FirebaseNotification notification = e.Message.Notification;
		if (notification != null)
		{
			Debug.Log("title: " + notification.Title);
			Debug.Log("body: " + notification.Body);
		}
		if (e.Message.From.Length > 0)
		{
			Debug.Log("from: " + e.Message.From);
		}
		if (e.Message.Link != null)
		{
			Debug.Log("link: " + e.Message.Link.ToString());
		}
		if (e.Message.Data.Count <= 0)
		{
			return;
		}
		Debug.Log("data:");
		foreach (KeyValuePair<string, string> datum in e.Message.Data)
		{
			Debug.Log("  " + datum.Key + ": " + datum.Value);
		}
	}

	public virtual void OnTokenReceived(object sender, TokenReceivedEventArgs token)
	{
		Debug.Log("Received Registration Token: " + token.Token);
	}

	public void ToggleTokenOnInit()
	{
		bool flag2 = (Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = !Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled);
		Debug.Log("Set TokenRegistrationOnInitEnabled to " + flag2);
	}

	public void OnDestroy()
	{
		Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
		Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
	}

	protected bool LogTaskCompletion(Task task, string operation)
	{
		bool result = false;
		if (task.IsCanceled)
		{
			Debug.Log(operation + " canceled.");
		}
		else if (task.IsFaulted)
		{
			Debug.Log(operation + " encounted an error.");
			foreach (Exception innerException in task.Exception.Flatten().InnerExceptions)
			{
				string text = string.Empty;
				FirebaseException ex = innerException as FirebaseException;
				if (ex != null)
				{
					text = string.Format("Error.{0}: ", ((Error)ex.ErrorCode).ToString());
				}
				Debug.Log(text + innerException.ToString());
			}
		}
		else if (task.IsCompleted)
		{
			Debug.Log(operation + " completed");
			result = true;
		}
		return result;
	}*/
}
