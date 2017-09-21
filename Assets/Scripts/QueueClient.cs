using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using UnityEngine.UI;
using System.IO;


public class QueueClient : MonoBehaviour {
	private IAmazonSQS SqsClient;
	private string queueUrl = "https://sqs.us-east-1.amazonaws.com/755552506636/BridgeInvasionQueue.fifo";
	// Use this for initialization

	void Start () {
		UnityInitializer.AttachToGameObject(this.gameObject);
		Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;

		XmlDocument doc = new XmlDocument();
		TextAsset textAsset = (TextAsset)Resources.Load("AWS", typeof(TextAsset));
		doc.LoadXml (textAsset.text);
		XmlNode access_key = doc.GetElementsByTagName ("access_key") [0];
		XmlNode secret_key = doc.GetElementsByTagName ("secret_key") [0];

		SqsClient = new AmazonSQSClient (access_key.InnerText, secret_key.InnerText, RegionEndpoint.USEast1);

		StartCoroutine (listenForMessages(.5f));
	}
		

	IEnumerator listenForMessages(float waitTime)
	{
		while (true) {
			SqsClient.ReceiveMessageAsync(queueUrl, (result) =>
				{
					if (result.Exception == null)
					{
						var messages = result.Response.Messages;
						messages.ForEach(m =>
							{
								//Process Commands Here
								Debug.Log(@"Message Id  = " + m.MessageId);
								Debug.Log(@"Message = " + m.Body);

								//Deleting Message 
								deleteMessage(m.ReceiptHandle);
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

	void deleteMessage(string receiptHandle) {
		SqsClient.DeleteMessageAsync(queueUrl, receiptHandle, (result) =>
			{
				if (result.Exception == null)
				{
					Debug.Log(@"Message ReceiptHandle  = " + receiptHandle + " deleted");
				}
				else
				{
					Debug.Log("Exception occured while deleting message = " + receiptHandle);
					Debug.LogException(result.Exception);
				}
		});
	}
	
}
