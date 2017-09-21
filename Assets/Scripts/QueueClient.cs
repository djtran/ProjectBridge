using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using UnityEngine.UI;

public class QueueClient : MonoBehaviour {
	private IAmazonSQS SqsClient;
	// Use this for initialization
	void Start () {
		UnityInitializer.AttachToGameObject(this.gameObject);
		Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
		SqsClient = new AmazonSQSClient ("AKIAJVNEK74YSHKCDRSA", "0TpH6VNo7JwEl2Ze52oF1XPCabXnGmN9vScvZjYB", RegionEndpoint.USEast1);

		StartCoroutine (listenForMessages(2f));
	}
		

	IEnumerator listenForMessages(float waitTime)
	{
		string queueUrl = "https://sqs.us-east-1.amazonaws.com/755552506636/BridgeInvasionQueue.fifo";
		Debug.Log("Yay I made it here!");
		while (true) {
			SqsClient.ReceiveMessageAsync(queueUrl, (result) =>
				{
					if (result.Exception == null)
					{
						Debug.Log("No exception!");
						var messages = result.Response.Messages;
						messages.ForEach(m =>
							{
								Debug.Log(@"Message Id  = " + m.MessageId);
								Debug.Log(@"Mesage = " + m.Body);
							});
					}
					else
					{
						Debug.Log("An exception!");
						Debug.LogException(result.Exception);
					}
				});
			yield return new WaitForSeconds(waitTime);
		}
	}		
}
