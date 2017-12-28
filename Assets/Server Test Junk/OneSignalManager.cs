using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneSignalPush.MiniJSON;



public class OneSignalManager : MonoBehaviour {

	string userId;
	private static string oneSignalDebugMessage;

	void Start () {
		// Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
		// OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);

		OneSignal.StartInit("0656bbde-65bc-4c7b-9909-338b60f23954")
			.HandleNotificationOpened(HandleNotificationOpened)
			.EndInit();

		OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

		Invoke("GetOwnData", 2.0f);

		// Call syncHashedEmail anywhere in your app if you have the user's email.
		// This improves the effectiveness of OneSignal's "best-time" notification scheduling feature.
		// OneSignal.syncHashedEmail(userEmail);
	}

	// Gets called when the player opens the notification.
	private static void HandleNotificationOpened(OSNotificationOpenedResult result) {
	}

//	public void someMethod() {
//		// Just an example userId, use your own or get it the devices by calling OneSignal.GetIdsAvailable
//		userId = "e5d2e707-d55a-42d7-b1bf-02ef7c319ab3";
//
//		var notification = new Dictionary<string, object> ();
//		notification ["contents"] = new Dictionary<string, string> () { { "en", "Test Message" } };
//
//		notification ["include_player_ids"] = new List<string> () { userId };
//		// Example of scheduling a notification in the future.
//		notification ["send_after"] = System.DateTime.Now.ToUniversalTime ().AddSeconds (5).ToString ("U");
//
//		OneSignal.PostNotification (notification, (responseSuccess) => {
//			oneSignalDebugMessage = "Notification posted successful! Delayed by about 30 secounds to give you time to press the home button to see a notification vs an in-app alert.\n" + Json.Serialize (responseSuccess);
//		}, (responseFailure) => {
//			oneSignalDebugMessage = "Notification failed to post:\n" + Json.Serialize (responseFailure);
//		});
//
//	}

	void GetOwnData (){

		var status = OneSignal.GetPermissionSubscriptionState();
		//Debug.Log (status.ToString ());
		if (status.subscriptionStatus.userId != null) {
			GetComponent<UserAccountManagerScript> ().notificationId = status.subscriptionStatus.userId.ToString ();
		}

//		Debug.Log(status.permissionStatus.hasPrompted.ToString() + "111");
//		Debug.Log(status.permissionStatus.status.ToString() + "222");
//
//		Debug.Log(status.subscriptionStatus.subscribed.ToString() + "333");
//		Debug.Log(status.subscriptionStatus.userSubscriptionSetting.ToString() + "444");
		//Debug.Log(status.subscriptionStatus.userId.ToString() + "555");
		//Debug.Log(status.subscriptionStatus.pushToken.ToString() + "666");

	}

}
